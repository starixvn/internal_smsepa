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
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'SelfEvalutorModel', 'CommonModel', 'utilitiesFactory',
function ($window, $translate, $routeParams, EmployeeEPAModel, SelfEvalutorModel, CommonModel, utilitiesFactory) {
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

                    SelfEvalutorModel.calculateScore(vm);
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

        SelfEvalutorModel.calculateScore(vm);
    };

    vm.getNotificationInputScore = function (kpi) {
        return EmployeeEPAModel.getNotificationInputScore(0, kpi.Weight, vm.scoreInputValidMessage);
    };

    vm.inputScore = function (kpi) {
        kpi.CurrentEvaluation.Rate = 0;

        if (!EmployeeEPAModel.validateScoreInput(kpi.CurrentEvaluation.Score, 0, kpi.Weight, vm.scoreInputValidMessage)) {
            kpi.CurrentEvaluation.Score = null;
        }
        else {
            SelfEvalutorModel.calculateScore(vm);
        }
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
            var totalScore = utilitiesFactory.parseFloat(vm.epaInfo.CurrentProcess.TotalScore),
                disciplineScore = vm.epaInfo.MaxDisciplineScore - vm.epaInfo.DisciplineScore;

            if (disciplineScore < 0) {
                disciplineScore = 0;
            }

            vm.epaInfo.CurrentProcess.TotalScore = totalScore;
            vm.epaInfo.DisciplineScore = disciplineScore;
            vm.finalScore = totalScore;

            if (vm.epaInfo.HasDisciplineScore) {
                vm.finalScore += vm.epaInfo.DisciplineScore;
            }

            vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
            vm.evaluatedDate = vm.epaInfo.CurrentProcess.EvaluateDate ? vm.epaInfo.CurrentProcess.EvaluateDate : null;
            checkStatus(vm.epaInfo);

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