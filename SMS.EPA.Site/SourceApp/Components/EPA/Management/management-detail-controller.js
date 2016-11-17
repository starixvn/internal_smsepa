(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('epaModule').controller('ManagentDetailController', managentDetailController);
    managentDetailController.$inject = ['$timeout', '$translate', '$routeParams', 'EvaluationModel', 'ManagementEPAModel', 'ReviewerManagementModel',
        'CommonModel', 'utilitiesFactory', 'historyFactory', 'lockScreenService', 'baseUrl'];

    function managentDetailController($timeout, $translate, $routeParams, EvaluationModel, ManagementEPAModel, ReviewerManagementModel, CommonModel,
        utilitiesFactory, historyFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.permission = 0;
        vm.epaId = parseInt($routeParams.id);
        vm.jobGrades = [];
        vm.standardGrades = [];
        vm.policyEnum = utilitiesFactory.policy;
        vm.targetTypeEnum = utilitiesFactory.targetType;
        vm.ratingTypeEnum = utilitiesFactory.ratingType;
        vm.dataSaved = false;
        vm.evaluatedDate = null;
        vm.finalScore = null;
        vm.hasRatingByStar = false;
        vm.showRatingScoreByStar = true;

        vm.generateRatingStar = function () {
            $timeout(function () {
                $(".rating-loading").rating();
            }, 1);
        };

        vm.range = function (n) {
            return new Array(n);
        };

        vm.getNotificationInputScore = function (processByManagement) {
            if (processByManagement) {
                return EvaluationModel.getNotificationInputScore(0, processByManagement.MaxTotalScore, vm.scoreInputValidMessage);
            }

            return '';
        };

        vm.inputScore = function (processByManagement) {
            if (!EvaluationModel.validateScoreInput(processByManagement.TotalScore, 0, processByManagement.MaxTotalScore, vm.scoreInputValidMessage)) {
                processByManagement.TotalScore = null;
            }

            ReviewerManagementModel.calculatePerformanceScore(vm, false);
        };

        // Event read more / less text.
        utilitiesFactory.maxLengthDescription(vm);

        vm.readMore = function (item) {
            utilitiesFactory.readMore(item);
        };

        vm.readLess = function (item) {
            utilitiesFactory.readLess(item);
        };

        vm.save = function (epaInfo) {
            var totalcore = epaInfo.Process.CurrentProcess.TotalScore;

            if (ReviewerManagementModel.validateEvaluation(totalcore)) {
                var model = new Array(epaInfo);

                ManagementEPAModel.adjustScoreByManagement(vm, model);
            }
        };

        vm.cancel = function () {
            historyFactory.back();
        };

        function getEPAInfoByManagement() {
            ManagementEPAModel.getEPAInfoByManagement(vm.epaId).then(function () {
                vm.epaInfo = ManagementEPAModel.epaInfo;
                vm.epaInfo.Employee.Avatar = baseUrl + vm.epaInfo.Employee.Avatar;

                if (vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypeEnum.Star
                        || vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypeEnum.HalfStar
                        || (vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypeEnum.Score
                            && $.grep(vm.epaInfo.KPIGroups, function (e) { return e.NumberKPIOptional > 0; }).length > 0)) {
                    vm.hasRatingByStar = true;
                }

                if (vm.epaInfo.Process.Reviewers.length > 0 && vm.epaInfo.Process.Reviewers[0].IsEndProcess && !vm.epaInfo.Process.CurrentProcess.TotalScore) {
                    vm.epaInfo.Process.CurrentProcess.TotalScore = vm.epaInfo.Process.AverageScoreByReviewers;
                } else if (vm.epaInfo.Process.Evaluators[0].IsEndProcess && !vm.epaInfo.Process.CurrentProcess.TotalScore) {
                    if (vm.epaInfo.IsSemiHasMonthly) {
                        vm.epaInfo.Process.CurrentProcess.TotalScore = vm.epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted;
                    } else {
                        vm.epaInfo.Process.CurrentProcess.TotalScore = vm.epaInfo.Process.AverageScoreByEvaluators;
                    }
                }

                // self
                vm.epaInfo.Process.Self.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.Self.TotalScore);

                // evaluator
                angular.forEach(vm.epaInfo.Process.Evaluators, function (evaluator) {
                    evaluator.TotalScore = utilitiesFactory.parseFloat(evaluator.TotalScore);
                });

                // reviewer
                angular.forEach(vm.epaInfo.Process.Reviewers, function (reviewer) {
                    reviewer.TotalScore = utilitiesFactory.parseFloat(reviewer.TotalScore);
                });

                // management
                vm.epaInfo.Process.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.CurrentProcess.TotalScore);
                vm.finalScore = vm.epaInfo.Process.CurrentProcess.TotalScore + vm.epaInfo.DisciplineScore;
                vm.finalScore = EvaluationModel.checkScore(vm.finalScore, 0, vm.epaInfo.Process.CurrentProcess.MaxPerformanceScore);

                vm.epaInfo.Grade = EvaluationModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
                vm.evaluatedDate = vm.epaInfo.Process.CurrentProcess.EvaluateDate ? vm.epaInfo.Process.CurrentProcess.EvaluateDate : null;

                ManagementEPAModel.getUserPermissionOnManagement().then(function () {
                    vm.permission = ManagementEPAModel.permission;

                    if (vm.permission !== vm.policyEnum.Host) {
                        vm.epaInfo.IsAllowedToEdit = false;
                    }
                });

                lockScreenService.stop();
            });
        }

        function initialize() {
            lockScreenService.start();

            $translate('ScoreInputValid').then(function (message) {
                vm.scoreInputValidMessage = message;
            });

            CommonModel.getJobGrade(true).then(function () {
                vm.jobGrades = CommonModel.jobGrades;
            });

            CommonModel.getStandardGrade().then(function () {
                vm.standardGrades = CommonModel.standardGrades;

                getEPAInfoByManagement();
            });
        }

        initialize();
    }
})(angular);