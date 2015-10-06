'use strict';
/*global epaModule*/

epaModule.controller('EmployeeEPAController',
['EmployeeEPAModel',
function (EmployeeEPAModel) {
    var vm = this;

    vm.myEPAs = EmployeeEPAModel.myEPAs;

    vm.viewEPADetail = function (epa) {
        vm.viewEPADetailUrl = '/SelfEvaluation/Detail/' + epa.EPAId;
    };

    vm.viewEPAResult = function (epa) {
        vm.viewEPAResultUrl = '/EPA/Detail/' + epa.EPAId;
    };

    vm.initialize = function () {
        EmployeeEPAModel.getEPAByEmployee().then(function () {
            vm.myEPAs = EmployeeEPAModel.myEPAs;
        });
    };

    vm.initialize();
}]);

epaModule.controller('EmployeeEPAInfoController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'CommonModel',
function ($window, $translate, $routeParams, EmployeeEPAModel, CommonModel) {
    var vm = this;

    vm.epaId = parseInt($routeParams.id);

    vm.maxStar = 5;
    vm.dataSaved = false;
    vm.isReadonly = false;
    vm.evaluatedDate = null;

    vm.epaInfo = EmployeeEPAModel.epaInfo;
    vm.standardGrades = CommonModel.standardGrades;

    var calculateScore = function (epaInfo) {
        var totalScore = 0;

        angular.forEach(epaInfo.KPIGroups, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIs, function (kpi) {
                if (kpi.CurrentEvaluation.Score && parseFloat(kpi.CurrentEvaluation.Score) > 0) {
                    totalScore += parseFloat(kpi.CurrentEvaluation.Score);
                }
            });
        });

        epaInfo.CurrentProcess.TotalScore = totalScore.toFixed(2);

        if (epaInfo.HasDisciplineScore) {
            vm.finalScore = totalScore + epaInfo.DisciplineScore;
        }
        else {
            vm.finalScore = totalScore;
        }

        epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, epaInfo.MaxFinalScore, vm.standardGrades);
    };

    var checkStatus = function (epaInfo) {
        if (epaInfo.CurrentProcess.IsEndProcess) {
            vm.isReadonly = true;
            vm.hoveringOver = null;
            vm.hoveringLeave = null;
        }
        else {
            vm.hoveringOver = function (value, kpi) {
                kpi.CurrentEvaluation.Percent = (100 * value) / vm.maxStar;
            };

            vm.hoveringLeave = function (kpiGroup, kpi) {
                kpi.CurrentEvaluation.Percent = (100 * kpi.CurrentEvaluation.Rate) / vm.maxStar;

                if (kpi.CurrentEvaluation.Percent > 0) {
                    if (kpiGroup.TotalKPIOptional === 0) {
                        var score = kpi.Weight * (kpi.CurrentEvaluation.Percent / 100);

                        kpi.CurrentEvaluation.Score = (score % 1) === 0 ? score : score.toFixed(2);
                    }
                    else {
                        var weight, totalKPIOptional = angular.copy(kpiGroup.TotalKPIOptional);

                        angular.forEach(kpiGroup.KPIs, function (item) {
                            if (!item.IsRequiredEvaluate && item.CurrentEvaluation.Rate > 0) {
                                totalKPIOptional -= 1;
                            }
                        });

                        weight = kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional);

                        angular.forEach(kpiGroup.KPIs, function (item) {
                            if (item.CurrentEvaluation.Rate > 0) {
                                var score = weight * (item.CurrentEvaluation.Percent / 100);

                                item.CurrentEvaluation.Score = (score % 1) === 0 ? score : score.toFixed(2);
                            }
                        });
                    }

                    calculateScore(vm.epaInfo);
                }
            };
        }
    };

    var validateEPA = function (epaInfo) {
        var isValid = true;

        angular.forEach(epaInfo.KPIGroups, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIs, function (kpi) {
                if (kpi.IsRequiredEvaluate && (!kpi.CurrentEvaluation.Score || parseFloat(kpi.CurrentEvaluation.Score) <= 0)) {
                    isValid = false;
                    kpi.CurrentEvaluation.EvaluateValid = true;
                }
                else {
                    kpi.CurrentEvaluation.EvaluateValid = false;
                }

                if (kpi.HasComment && kpi.IsRequiredComment && (!kpi.CurrentEvaluation.Comment || kpi.CurrentEvaluation.Comment.length === 0)) {
                    isValid = false;
                    kpi.CurrentEvaluation.CommentValid = true;
                }
                else {
                    kpi.CurrentEvaluation.CommentValid = false;
                }
            });
        });

        if (!isValid) {
            EmployeeEPAModel.openPopupNotificationComment(vm, vm.evaluateEPAInvalidMessage);
        }

        return isValid;
    };

    vm.getClassProgress = function (statusEndProcess) {
        return EmployeeEPAModel.getClassProgress(statusEndProcess);
    };

    vm.cancelRating = function (kpiGroup, kpi) {
        kpi.CurrentEvaluation.Rate = 0;
        kpi.CurrentEvaluation.Percent = 0;
        kpi.CurrentEvaluation.Score = null;

        if (kpiGroup.TotalKPIOptional > 0) {
            var weight, totalKPIOptional = angular.copy(kpiGroup.TotalKPIOptional);

            angular.forEach(kpiGroup.KPIs, function (item) {
                if (!item.IsRequiredEvaluate && item.CurrentEvaluation.Rate > 0) {
                    totalKPIOptional -= 1;
                }
            });

            weight = kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional);

            angular.forEach(kpiGroup.KPIs, function (item) {
                if (item.CurrentEvaluation.Rate > 0) {
                    var score = weight * (item.CurrentEvaluation.Percent / 100);

                    item.CurrentEvaluation.Score = (score % 1) === 0 ? score : score.toFixed(2);
                }
            });
        }

        calculateScore(vm.epaInfo);
    };

    vm.getNotificationInputScore = function (kpi) {
        return EmployeeEPAModel.getNotificationInputScore(kpi.Weight, vm.scoreInputValidMessage);
    };

    vm.inputScore = function (kpi) {
        kpi.CurrentEvaluation.Rate = 0;
        kpi.CurrentEvaluation.Percent = null;

        if (!EmployeeEPAModel.validateScoreInput(kpi.CurrentEvaluation.Score, kpi.Weight, vm.scoreInputValidMessage)) {
            kpi.CurrentEvaluation.Score = null;
        }
        else {
            calculateScore(vm.epaInfo);
        }
    };

    vm.save = function (epaInfo) {
        if (validateEPA(epaInfo)) {
            EmployeeEPAModel.updateEPAScoreByEmployee(vm);
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };

    var getEPAInfoByEmployee = function (epaId) {
        EmployeeEPAModel.getEPAInfoByEmployee(epaId).then(function () {
            var data = EmployeeEPAModel.epaInfo;
            var totalScoreOfSelf = data.CurrentProcess.TotalScore ? parseFloat(data.CurrentProcess.TotalScore) : 0;

            if (data.HasDisciplineScore) {
                vm.finalScore = totalScoreOfSelf ? (totalScoreOfSelf + data.DisciplineScore) : data.DisciplineScore;
            }
            else {
                vm.finalScore = totalScoreOfSelf;
            }

            data.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, data.MaxFinalScore, vm.standardGrades);
            vm.evaluatedDate = data.CurrentProcess.EvaluateDate ? data.CurrentProcess.EvaluateDate : null;
            vm.epaInfo = data;
            checkStatus(vm.epaInfo);

            angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
                if (!kpiGroup.HasKPIOptional && kpiGroup.IsSpreadKPI) {
                    var weight = kpiGroup.Weight / kpiGroup.KPIs.length;

                    angular.forEach(kpiGroup.KPIs, function (kpi) {
                        if ((weight % 1) === 0) {
                            kpi.Weight = weight;
                        }
                        else {
                            kpi.Weight = weight.toFixed(2);
                        }
                    });
                }
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

            getEPAInfoByEmployee(vm.epaId);
        });
    };

    vm.initialize();
}]);