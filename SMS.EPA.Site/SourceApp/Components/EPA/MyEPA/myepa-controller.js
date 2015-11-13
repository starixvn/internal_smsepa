'use strict';
/*global epaModule*/

epaModule.controller('EmployeeEPAController',
['SelfEvalutorModel', 'baseUrl',
function (SelfEvalutorModel, baseUrl) {
    var vm = this;

    vm.myEPAs = SelfEvalutorModel.myEPAs;

    vm.viewEPADetail = function (epa) {
        vm.viewEPADetailUrl = baseUrl + 'SelfEvaluation/Detail/' + epa.EPAId;
    };

    vm.viewEPAResult = function (epa) {
        vm.viewEPAResultUrl = baseUrl + 'EPA/Detail/' + epa.EPAId;
    };

    vm.initialize = function () {
        SelfEvalutorModel.getEPAByEmployee().then(function () {
            vm.myEPAs = SelfEvalutorModel.myEPAs;
        });
    };

    vm.initialize();
}]);

epaModule.controller('EmployeeEPAInfoController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'SelfEvalutorModel', 'CommonModel', 'utilitiesFactory', 'baseUrl',
function ($window, $translate, $routeParams, EmployeeEPAModel, SelfEvalutorModel, CommonModel, utilitiesFactory, baseUrl) {
    var vm = this;

    vm.epaId = parseInt($routeParams.id);

    vm.maxStar = 5;
    vm.dataSaved = false;
    vm.isReadonly = false;
    vm.evaluatedDate = null;

    vm.epaInfo = SelfEvalutorModel.epaInfo;
    vm.standardGrades = CommonModel.standardGrades;

    var checkStatus = function (epaInfo) {
        if (epaInfo.CurrentProcess.IsEndProcess) {
            vm.isReadonly = true;
            vm.hoveringLeave = null;
        }
        else {
            vm.hoveringLeave = function (kpiGroup, kpi) {
                if (kpi.CurrentEvaluation.Rate > 0) {
                    if (!kpiGroup.HasKPIOptional) {
                        var score = utilitiesFactory.parseFloat(kpi.Weight * (kpi.CurrentEvaluation.Rate / vm.maxStar));
                        kpi.CurrentEvaluation.Score = score;
                    }
                    else {
                        SelfEvalutorModel.scoreSpreadForKPIGroupHasKPIOptional(kpiGroup, vm.maxStar);
                    }

                    SelfEvalutorModel.calculateScore(vm, true);
                }
            };
        }
    };

    vm.getClassProgress = function (statusEndProcess) {
        return EmployeeEPAModel.getClassProgress(statusEndProcess);
    };

    vm.cancelRating = function (kpiGroup, kpi) {
        kpi.CurrentEvaluation.Rate = 0;
        kpi.CurrentEvaluation.Score = null;

        if (kpiGroup.HasKPIOptional) {
            SelfEvalutorModel.scoreSpreadForKPIGroupHasKPIOptional(kpiGroup, vm.maxStar);
        }

        SelfEvalutorModel.calculateScore(vm, true);
    };

    vm.getNotificationInputScore = function (kpi) {
        return EmployeeEPAModel.getNotificationInputScore(0, kpi.Weight, vm.scoreInputValidMessage);
    };

    vm.inputScore = function (kpi) {
        kpi.CurrentEvaluation.Rate = 0;

        if (!EmployeeEPAModel.validateScoreInput(kpi.CurrentEvaluation.Score, 0, kpi.Weight, vm.scoreInputValidMessage)) {
            kpi.CurrentEvaluation.Score = null;
        }

        SelfEvalutorModel.calculateScore(vm, true);
    };

    vm.save = function (epaInfo) {
        if (SelfEvalutorModel.validateEPA(epaInfo)) {
            SelfEvalutorModel.updateEPAScoreByEmployee(vm);
        }
        else {
            SelfEvalutorModel.openPopupNotificationInvalidComment(vm, vm.evaluateEPAInvalidMessage);
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };

    var getEPAInfoByEmployee = function () {
        SelfEvalutorModel.getEPAInfoByEmployee(vm.epaId).then(function () {
            vm.epaInfo = SelfEvalutorModel.epaInfo;
            checkStatus(vm.epaInfo);

            vm.epaInfo.Employee.Avatar = baseUrl + vm.epaInfo.Employee.Avatar;

            vm.maxPerformanceScore = vm.epaInfo.CurrentProcess.MaxTotalScore;

            if (vm.epaInfo.HasDisciplineScore) {
                if (vm.epaInfo.DisciplineScore < 0) {
                    vm.epaInfo.DisciplineScore = 0;
                }

                vm.maxPerformanceScore += vm.epaInfo.MaxDisciplineScore;
            }

            vm.epaInfo.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.CurrentProcess.TotalScore);
            vm.finalScore = vm.epaInfo.CurrentProcess.TotalScore + vm.epaInfo.DisciplineScore;
            vm.finalScore = EmployeeEPAModel.checkScore(vm.finalScore, 0, vm.maxPerformanceScore);

            if (vm.epaInfo.LastFinalScore.length > 0) {
                vm.epaInfo.LastFinalScore = utilitiesFactory.parseFloat(vm.epaInfo.LastFinalScore);
            } else {
                vm.epaInfo.LastFinalScore = null;
            }

            vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
            vm.evaluatedDate = vm.epaInfo.CurrentProcess.EvaluateDate ? vm.epaInfo.CurrentProcess.EvaluateDate : null;

            angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
                EmployeeEPAModel.scoreSpread(kpiGroup);
            });
        });
    };

    vm.initialize = function () {
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
    };

    vm.initialize();
}]);