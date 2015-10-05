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
                var totalScore = memberEPA.CurrentProcess.TotalScore ? parseFloat(memberEPA.CurrentProcess.TotalScore) : null;

                if (memberEPA.HasDisciplineScore) {
                    memberEPA.FinalScore = totalScore + memberEPA.DisciplineScore;
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
                if (kpi.IsRequiredEvaluate
                    && (!kpi.CurrentEvaluation.Score || parseFloat(kpi.CurrentEvaluation.Score) <= 0)) {
                    isValid = false;
                    kpi.CurrentEvaluation.EvaluateValid = true;
                }
                else {
                    kpi.CurrentEvaluation.EvaluateValid = false;
                }

                if (kpi.HasComment
                    && kpi.IsRequiredComment
                    && (!kpi.CurrentEvaluation.Comment || kpi.CurrentEvaluation.Comment.length === 0)) {
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

    vm.cancel = function () {
        $window.history.back();
    };

    vm.save = function (epaInfo) {
        if (validateEPA(epaInfo)) {
            EmployeeEPAModel.updateEPAScoreByEvaluator(vm);
        }
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
        });

        EmployeeEPAModel.getEPAInfoByEvaluator(vm.epaId, vm.employeeId, vm.evaluatorId).then(function () {
            var data = EmployeeEPAModel.epaInfo;
            var totalScoreOfEvaluator = data.CurrentProcess.TotalScore ? parseFloat(data.CurrentProcess.TotalScore) : 0,
                totalScoreOfSelf = data.ProcessBySelf.TotalScore ? parseFloat(data.ProcessBySelf.TotalScore) : 0;

            if (data.HasDisciplineScore) {
                vm.finalScore = totalScoreOfEvaluator !== 0 ? (totalScoreOfEvaluator + data.DisciplineScore) : data.DisciplineScore;
                vm.finalScoreOfSelf = totalScoreOfSelf + data.DisciplineScore;
            }
            else {
                vm.finalScore = totalScoreOfEvaluator;
                vm.finalScoreOfSelf = totalScoreOfSelf;
            }

            vm.gradeOfSelf = EmployeeEPAModel.convertScoreToGrade(vm.finalScoreOfSelf, data.MaxFinalScore, vm.standardGrades)

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

    vm.initialize();
}]);