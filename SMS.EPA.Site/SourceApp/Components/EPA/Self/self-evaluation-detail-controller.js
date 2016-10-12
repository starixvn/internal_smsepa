(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('epaModule').controller('EmployeeEPAInfoController', employeeEPAInfoController);
    employeeEPAInfoController.$inject = ['$timeout', '$translate', '$routeParams', 'EmployeeEPAModel', 'SelfEvalutorModel',
        'SelfEvalutionModel', 'CommonModel', 'utilitiesFactory', 'historyFactory', 'baseUrl'];

    function employeeEPAInfoController($timeout, $translate, $routeParams, EmployeeEPAModel, SelfEvalutorModel,
        SelfEvalutionModel, CommonModel, utilitiesFactory, historyFactory, baseUrl) {
        var vm = this;

        vm.dataSaved = false;
        vm.epaId = parseInt($routeParams.id);
        vm.epaInfo = {};
        vm.evaluatedDate = null;
        vm.maxStar = utilitiesFactory.maxStar;
        vm.policyEnum = utilitiesFactory.policy;
        vm.ratingTypeEnum = utilitiesFactory.ratingType;
        vm.standardGrades = [];
        vm.statusEndProcessEnum = utilitiesFactory.statusEndProcess;
        vm.targetTypeEnum = utilitiesFactory.targetType;

        vm.cancel = cancel;
        vm.cancelRating = cancelRating;
        vm.generateRatingStar = generateRatingStar;
        vm.getClassProgress = getClassProgress;
        vm.getNotificationInputScore = getNotificationInputScore;
        vm.hoveringLeave = null;
        vm.inputScore = inputScore;
        vm.save = save;

        initialize();

        function cancel() {
            historyFactory.back();
        }

        function cancelRating(kpiGroup, kpi) {
            kpi.CurrentEvaluation.Rate = 0;
            $("#star_" + kpi.KPIId).rating('update', kpi.CurrentEvaluation.Rate);
            kpi.CurrentEvaluation.Score = null;

            if (kpiGroup.HasKPIOptional) {
                SelfEvalutorModel.spreadScoreForKPIGroupHasKPIOptional(kpiGroup, vm.maxStar);
            }

            SelfEvalutorModel.calculateScore(vm);
        }

        function checkStatus(epaInfo) {
            if (epaInfo.IsAllowedToEdit) {
                vm.hoveringLeave = hoveringLeave;
            }
        }

        function generateRatingStar() {
            $timeout(function () {
                $(".rating-loading").rating();
            }, 1);
        }

        function getClassProgress(statusEndProcess) {
            return EmployeeEPAModel.getClassProgress(statusEndProcess);
        }

        function getEPAInfoByEmployee() {
            SelfEvalutionModel.getEPAInfoByEmployee(vm.epaId).then(function () {
                vm.epaInfo = SelfEvalutionModel.epaInfo;
                checkStatus(vm.epaInfo);

                vm.epaInfo.Employee.Avatar = baseUrl + vm.epaInfo.Employee.Avatar;
                vm.epaInfo.Process.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.CurrentProcess.TotalScore);
                vm.finalScore = vm.epaInfo.Process.CurrentProcess.TotalScore + vm.epaInfo.DisciplineScore;
                vm.finalScore = EmployeeEPAModel.checkScore(vm.finalScore, 0, vm.epaInfo.Process.CurrentProcess.MaxPerformanceScore);

                if (vm.epaInfo.LastFinalScore.length > 0) {
                    vm.epaInfo.LastFinalScore = utilitiesFactory.parseFloat(vm.epaInfo.LastFinalScore);
                } else {
                    vm.epaInfo.LastFinalScore = null;
                }

                vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
                vm.evaluatedDate = vm.epaInfo.Process.CurrentProcess.EvaluateDate ? vm.epaInfo.Process.CurrentProcess.EvaluateDate : null;

                SelfEvalutionModel.getUserPermissionOnEmployee().then(function () {
                    var permission = SelfEvalutionModel.permission;

                    if (permission !== vm.policyEnum.Host) {
                        vm.epaInfo.IsAllowedToEdit = false;
                        vm.hoveringLeave = null;
                    }
                });
            });
        }

        function getNotificationInputScore(kpi) {
            var weight = utilitiesFactory.splitNumber(kpi.Weight, 2);

            return EmployeeEPAModel.getNotificationInputScore(0, weight, vm.scoreInputValidMessage);
        }

        function hoveringLeave(kpiGroup, kpi) {
            if (kpi.CurrentEvaluation.Rate > 0) {
                if (!kpiGroup.HasKPIOptional) {
                    var score = (kpi.Weight * kpi.CurrentEvaluation.Rate) / vm.maxStar;
                    score = utilitiesFactory.splitNumber(score, 4);
                    kpi.CurrentEvaluation.Score = score;
                } else {
                    SelfEvalutorModel.spreadScoreForKPIGroupHasKPIOptional(kpiGroup, vm.maxStar);
                }

                SelfEvalutorModel.calculateScore(vm);
            }
        }

        function initialize() {
            $translate('ScoreInputValid').then(function (message) {
                vm.scoreInputValidMessage = message;
            });

            $translate('EvaluateEPAInvalid').then(function (message) {
                vm.evaluateEPAInvalidMessage = message;
            });

            CommonModel.getStandardGrade().then(function () {
                vm.standardGrades = CommonModel.standardGrades;

                getEPAInfoByEmployee();
            });
        }

        function inputScore(kpi) {
            var weight = utilitiesFactory.splitNumber(kpi.Weight, 2),
                score = kpi.CurrentEvaluation.Score ? parseFloat(kpi.CurrentEvaluation.Score) : 0;

            score = utilitiesFactory.splitNumber(score, 2);
            kpi.CurrentEvaluation.Rate = 0;

            if (!EmployeeEPAModel.validateScoreInput(score, 0, weight, vm.scoreInputValidMessage)) {
                kpi.CurrentEvaluation.Score = null;
            }

            SelfEvalutorModel.calculateScore(vm);
        }

        function save(epaInfo) {
            if (SelfEvalutionModel.validateEPA(epaInfo)) {
                SelfEvalutionModel.updateEPAScoreByEmployee(vm);
            } else {
                SelfEvalutorModel.openPopupNotificationInvalidComment(vm, vm.evaluateEPAInvalidMessage);
            }
        }
    }
})(angular);