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
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function ($window, $translate, $routeParams, EmployeeEPAModel, CommonModel, utilitiesFactory) {
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
                var rate = utilitiesFactory.parseFloat(kpi.CurrentEvaluation.Rate);
                var score = utilitiesFactory.parseFloat(kpi.CurrentEvaluation.Score);
                totalScore += score;

                if (score === kpi.Weight || rate === 5) {
                    kpi.CurrentEvaluation.IsMaxScore = true;
                }
                else {
                    kpi.CurrentEvaluation.IsMaxScore = false;
                }
            });
        });

        epaInfo.CurrentProcess.TotalScore = totalScore;
        vm.finalScore = totalScore;

        if (epaInfo.HasDisciplineScore) {
            vm.finalScore = totalScore + epaInfo.DisciplineScore;
        }

        if (vm.finalScore > epaInfo.MaxFinalScore) {
            vm.finalScore = epaInfo.MaxFinalScore;
        }
        else if (vm.finalScore < 0) {
            vm.finalScore = 0;
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
                    if (kpiGroup.NumberKPIOptional === 0) {
                        var score = utilitiesFactory.parseFloat(kpi.Weight * (kpi.CurrentEvaluation.Percent / 100));

                        kpi.CurrentEvaluation.Score = score;
                    }
                    else {
                        var weight, totalKPIOptional = angular.copy(kpiGroup.NumberKPIOptional);

                        angular.forEach(kpiGroup.KPIs, function (item) {
                            if (!item.IsRequiredEvaluate && item.CurrentEvaluation.Rate > 0) {
                                totalKPIOptional -= 1;
                            }
                        });

                        weight = utilitiesFactory.parseFloat(kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional));

                        angular.forEach(kpiGroup.KPIs, function (item) {
                            if (item.CurrentEvaluation.Rate > 0) {
                                var score = weight * (item.CurrentEvaluation.Percent / 100);

                                item.CurrentEvaluation.Score = score;
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
                    kpi.CurrentEvaluation.EvaluateInvalid = true;
                } else {
                    kpi.CurrentEvaluation.EvaluateInvalid = false;
                }

                if (kpi.HasComment && (!kpi.CurrentEvaluation.Comment || kpi.CurrentEvaluation.Comment.length === 0)
                    && (kpi.IsRequiredComment || (epaInfo.RequiredCommentOnMaxScore && !kpi.IsRequiredComment && kpi.CurrentEvaluation.IsMaxScore))) {
                    isValid = false;
                    kpi.CurrentEvaluation.CommentInvalid = true;
                    kpi.ExpandedComment = true;
                } else {
                    kpi.CurrentEvaluation.CommentInvalid = false;
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

        if (kpiGroup.NumberKPIOptional > 0) {
            var weight, totalKPIOptional = angular.copy(kpiGroup.NumberKPIOptional);

            angular.forEach(kpiGroup.KPIs, function (item) {
                if (!item.IsRequiredEvaluate && item.CurrentEvaluation.Rate > 0) {
                    totalKPIOptional -= 1;
                }
            });

            weight = utilitiesFactory.parseFloat(kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional));

            angular.forEach(kpiGroup.KPIs, function (item) {
                if (item.CurrentEvaluation.Rate > 0) {
                    var score = weight * (item.CurrentEvaluation.Percent / 100);

                    item.CurrentEvaluation.Score = score;
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

    var getEPAInfoByEmployee = function () {
        EmployeeEPAModel.getEPAInfoByEmployee(vm.epaId).then(function () {
            vm.epaInfo = EmployeeEPAModel.epaInfo;
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

            vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.epaInfo.MaxFinalScore, vm.standardGrades);
            vm.evaluatedDate = vm.epaInfo.CurrentProcess.EvaluateDate ? vm.epaInfo.CurrentProcess.EvaluateDate : null;
            checkStatus(vm.epaInfo);

            angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
                if (!kpiGroup.HasKPIOptional && kpiGroup.IsSpreadKPI) {
                    var weight = (kpiGroup.Weight / kpiGroup.KPIs.length).toFixed(2);

                    angular.forEach(kpiGroup.KPIs, function (kpi) {
                        kpi.Weight = weight;
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

            getEPAInfoByEmployee();
        });
    };

    vm.initialize();
}]);