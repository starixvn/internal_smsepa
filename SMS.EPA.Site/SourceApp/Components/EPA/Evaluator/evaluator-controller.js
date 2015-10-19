'use strict';
/*global epaModule*/

epaModule.controller('EvaluatorController',
['EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function (EmployeeEPAModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.memberEPAs = EmployeeEPAModel.memberEPAs;
    vm.members = EmployeeEPAModel.members;
    vm.standardGrades = CommonModel.standardGrades;

    vm.filter = {
        groupId: '',
        employeeIds: ''
    };

    vm.getMember = function (groupId) {
        EmployeeEPAModel.getMemberByEvaluator(groupId).then(function () {
            vm.members = EmployeeEPAModel.members;
        });
    };

    var getEPAByEvaluator = function (groupId, employeeIds) {
        EmployeeEPAModel.getEPAByEvaluator(groupId, employeeIds).then(function () {
            vm.memberEPAs = EmployeeEPAModel.memberEPAs;

            angular.forEach(vm.memberEPAs, function (memberEPA) {
                var totalScore = memberEPA.CurrentProcess.TotalScore ? parseFloat(memberEPA.CurrentProcess.TotalScore) : null,
                    disciplineScore = memberEPA.MaxDisciplineScore - memberEPA.DisciplineScore;

                if (disciplineScore < 0) {
                    disciplineScore = 0;
                }

                memberEPA.ProcessBySelf.TotalScore = memberEPA.ProcessBySelf.TotalScore ? parseFloat(memberEPA.ProcessBySelf.TotalScore) : 0;
                memberEPA.DisciplineScore = disciplineScore;

                if (memberEPA.HasDisciplineScore) {
                    memberEPA.FinalScore = totalScore + memberEPA.DisciplineScore;
                    memberEPA.ProcessBySelf.TotalScore += memberEPA.DisciplineScore;
                }
                else {
                    memberEPA.FinalScore = totalScore;
                }

                memberEPA.Grade = EmployeeEPAModel.convertScoreToGrade(memberEPA.FinalScore, memberEPA.MaxFinalScore, vm.standardGrades);
            });
        });
    };

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByEvaluator(filter.groupId, filter.employeeIds);
    };

    vm.viewEPADetail = function (memberEPA) {
        vm.viewEPADetailUrl = '/Evaluator/Detail/' + memberEPA.EPAId;
    };

    vm.viewEPAResult = function (memberEPA) {
        vm.viewEPAResultUrl = '/EPA/Detail/' + memberEPA.EPAId;
    };

    vm.initialize = function () {
        CommonModel.getAllGroup().then(function () {
            vm.groupOptions = CommonModel.groups;
        });

        vm.getMember(vm.filter.groupId);

        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;

            vm.query(vm.filter);
        });
    };

    vm.initialize();
}]);

epaModule.controller('MemberEPAInfoOfEvaluatorController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'CommonModel',
function ($window, $translate, $routeParams, EmployeeEPAModel, CommonModel) {
    var vm = this;

    vm.epaId = parseInt($routeParams.id);

    vm.maxStar = 5;
    vm.dataSaved = false;
    vm.isReadonly = false;
    vm.evaluatedDate = null;

    vm.standardGrades = CommonModel.standardGrades;

    var calculateScore = function (epaInfo) {
        var totalScore = 0;

        angular.forEach(epaInfo.KPIGroups, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIs, function (kpi) {
                var rate = kpi.CurrentEvaluation.Rate ? kpi.CurrentEvaluation.Rate : 0;
                var score = kpi.CurrentEvaluation.Score ? parseFloat(kpi.CurrentEvaluation.Score) : 0;
                totalScore += score;

                if (score === kpi.Weight || rate === 5) {
                    kpi.CurrentEvaluation.IsMaxScore = true;
                } else {
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
        } else if (vm.finalScore < 0) {
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
                        var score = (kpi.Weight * (kpi.CurrentEvaluation.Percent / 100)).toFixed(2);

                        kpi.CurrentEvaluation.Score = score;
                    }
                    else {
                        var weight, totalKPIOptional = angular.copy(kpiGroup.NumberKPIOptional);

                        angular.forEach(kpiGroup.KPIs, function (item) {
                            if (!item.IsRequiredEvaluate && item.CurrentEvaluation.Rate > 0) {
                                totalKPIOptional -= 1;
                            }
                        });

                        weight = (kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional)).toFixed(2);

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
                if (kpi.IsRequiredEvaluate
                    && (!kpi.CurrentEvaluation.Score || parseFloat(kpi.CurrentEvaluation.Score) <= 0)) {
                    isValid = false;
                    kpi.CurrentEvaluation.EvaluateInvalid = true;
                }
                else {
                    kpi.CurrentEvaluation.EvaluateInvalid = false;
                }

                if (kpi.HasComment && (!kpi.CurrentEvaluation.Comment || kpi.CurrentEvaluation.Comment.length === 0)
                    && (kpi.IsRequiredComment || (epaInfo.RequiredCommentOnMaxScore && !kpi.IsRequiredComment && kpi.CurrentEvaluation.IsMaxScore))) {
                    isValid = false;
                    kpi.CurrentEvaluation.CommentInvalid = true;
                    kpi.ExpandedCommentByEvaluator = true;
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

            weight = (kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional)).toFixed(2);

            angular.forEach(kpiGroup.KPIs, function (item) {
                if (item.CurrentEvaluation.Rate > 0) {
                    var score = weight * (item.CurrentEvaluation.Percent / 100);

                    item.CurrentEvaluation.Score = score;
                }
            });
        }

        calculateScore(vm.epaInfo);
    };

    vm.cancel = function () {
        $window.history.back();
    };

    vm.save = function (epaInfo) {
        if (validateEPA(epaInfo)) {
            EmployeeEPAModel.updateEPAScoreByEvaluator(vm);
        }
    };

    var getEPAInfoByEvaluator = function () {
        EmployeeEPAModel.getEPAInfoByEvaluator(vm.epaId).then(function () {
            vm.epaInfo = EmployeeEPAModel.epaInfo;

            var disciplineScore = vm.epaInfo.MaxDisciplineScore - vm.epaInfo.DisciplineScore;
            vm.epaInfo.CurrentProcess.TotalScore = vm.epaInfo.CurrentProcess.TotalScore ? parseFloat(vm.epaInfo.CurrentProcess.TotalScore) : 0;
            vm.epaInfo.ProcessBySelf.TotalScore = vm.epaInfo.ProcessBySelf.TotalScore ? parseFloat(vm.epaInfo.ProcessBySelf.TotalScore) : 0;

            if (disciplineScore < 0) {
                disciplineScore = 0;
            }

            vm.epaInfo.DisciplineScore = disciplineScore;
            vm.finalScore = vm.epaInfo.CurrentProcess.TotalScore;
            vm.finalScoreOfSelf = vm.epaInfo.ProcessBySelf.TotalScore;

            if (vm.epaInfo.HasDisciplineScore) {
                vm.finalScore += vm.epaInfo.DisciplineScore;
                vm.finalScoreOfSelf += vm.epaInfo.DisciplineScore;
            }

            vm.gradeOfSelf = EmployeeEPAModel.convertScoreToGrade(vm.finalScoreOfSelf, vm.epaInfo.MaxFinalScore, vm.standardGrades)

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

            getEPAInfoByEvaluator();
        });
    };

    vm.initialize();
}]);