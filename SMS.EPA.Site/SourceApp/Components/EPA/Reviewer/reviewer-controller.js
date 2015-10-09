'use strict';
/*global epaModule*/

epaModule.controller('ReviewerController',
['$translate', 'EmployeeEPAModel', 'CommonModel', 'utilitiesFactory', 'notificationFactory',
function ($translate, EmployeeEPAModel, CommonModel, utilitiesFactory, notificationFactory) {
    var vm = this;

    vm.memberEPAs = EmployeeEPAModel.memberEPAs;
    vm.members = EmployeeEPAModel.members;
    vm.standardGrades = CommonModel.standardGrades;
    vm.selectedAll = false;
    vm.averageScore = 0;
    vm.isEndProcessAll = false;

    vm.filter = {
        groupId: '',
        employeeIds: ''
    };

    vm.getMember = function (groupId) {
        EmployeeEPAModel.getMemberByReviewer(groupId).then(function () {
            vm.members = EmployeeEPAModel.members;
        });
    };

    vm.viewEPADetail = function (memberEPA) {
        vm.viewEPADetailUrl = '/Reviewer/Detail/' + memberEPA.EPAId + '?actorUserId=' + memberEPA.CurrentProcess.EvaluatorId;
    };

    vm.viewEPAResult = function (memberEPA) {
        vm.viewEPAResultUrl = '/EPA/Detail/' + memberEPA.EPAId;
    };

    var getEPAByReviewer = function (groupId, employeeIds) {
        EmployeeEPAModel.getEPAByReviewer(groupId, employeeIds).then(function () {
            vm.memberEPAs = EmployeeEPAModel.memberEPAs;

            var averageScore = 0;
            var countEndProcess = 0;
            angular.forEach(vm.memberEPAs, function (memberEPA) {
                var totalScore,
                    disciplineScore = memberEPA.MaxDisciplineScore - memberEPA.DisciplineScore;

                if (disciplineScore < 0) {
                    disciplineScore = 0;
                }

                if (memberEPA.ProcessByEvaluators[0].IsEndProcess && !memberEPA.CurrentProcess.TotalScore) {
                    memberEPA.CurrentProcess.TotalScore = memberEPA.AverageScoreByEvaluators;
                }

                totalScore = memberEPA.CurrentProcess.TotalScore ? parseFloat(memberEPA.CurrentProcess.TotalScore) : 0;

                memberEPA.DisciplineScore = disciplineScore;
                memberEPA.CurrentProcess.TotalScore = totalScore;
                memberEPA.ProcessBySelf.TotalScore = parseFloat(memberEPA.ProcessBySelf.TotalScore);
                memberEPA.FinalScore = totalScore;

                if (memberEPA.HasDisciplineScore) {
                    memberEPA.FinalScore += memberEPA.DisciplineScore;
                    memberEPA.ProcessBySelf.TotalScore += memberEPA.DisciplineScore;
                    memberEPA.AverageScoreByEvaluators += memberEPA.DisciplineScore;
                }

                if (memberEPA.FinalScore > memberEPA.MaxFinalScore) {
                    memberEPA.FinalScore = memberEPA.MaxFinalScore;
                }
                else if (memberEPA.FinalScore < 0) {
                    memberEPA.FinalScore = 0;
                }

                if (memberEPA.CurrentProcess.IsEndProcess) {
                    countEndProcess += 1;
                }
                averageScore += memberEPA.AverageScoreByEvaluators;
            });

            if (countEndProcess === vm.memberEPAs.length) {
                vm.isEndProcessAll = true;
            }
            vm.averageScore = (averageScore / vm.memberEPAs.length).toFixed(2);
        });
    };

    var calculateAdjustScore = function (memberEPAs, adjustScore) {
        var scoreAdjust = adjustScore.adjust.Score ? parseFloat(adjustScore.adjust.Score) : 0,
            totalScoreAdjust;

        angular.forEach(memberEPAs, function (memberEPA) {
            if (memberEPA.Selected) {
                totalScoreAdjust = memberEPA.AverageScoreByEvaluators + scoreAdjust;

                if (memberEPA.HasDisciplineScore) {
                    totalScoreAdjust -= memberEPA.DisciplineScore;
                }

                if (totalScoreAdjust > memberEPA.CurrentProcess.MaxTotalScore) {
                    totalScoreAdjust = memberEPA.CurrentProcess.MaxTotalScore;
                }
                else if (totalScoreAdjust < 0) {
                    totalScoreAdjust = 0;
                }

                memberEPA.CurrentProcess.AdjustScore = scoreAdjust;
                memberEPA.CurrentProcess.AdjustScoreOriginal = scoreAdjust;
                memberEPA.CurrentProcess.TotalScore = totalScoreAdjust;
                memberEPA.CurrentProcess.TotalScoreOriginal = totalScoreAdjust;
                memberEPA.FinalScore = totalScoreAdjust;

                if (memberEPA.HasDisciplineScore) {
                    memberEPA.FinalScore += memberEPA.DisciplineScore;
                }

                if (memberEPA.FinalScore > memberEPA.MaxFinalScore) {
                    memberEPA.FinalScore = memberEPA.MaxFinalScore;
                }
                else if (memberEPA.FinalScore < 0) {
                    memberEPA.FinalScore = 0;
                }

                if (memberEPA.CurrentProcess.Comment.length > 0) {
                    memberEPA.CurrentProcess.Comment = memberEPA.CurrentProcess.Comment + '\n' + adjustScore.adjust.Comment;
                }
                else {
                    memberEPA.CurrentProcess.Comment = adjustScore.adjust.Comment;
                }
            }
        });
    };

    var calculateFinalScore = function (memberEPA) {
        var adjustScore, totalScore;

        adjustScore = memberEPA.CurrentProcess.AdjustScore ? parseFloat(memberEPA.CurrentProcess.AdjustScore) : 0;
        totalScore = memberEPA.AverageScoreByEvaluators + adjustScore;
        if (memberEPA.HasDisciplineScore) {
            totalScore -= memberEPA.DisciplineScore;
        }

        if (totalScore > memberEPA.CurrentProcess.MaxTotalScore) {
            totalScore = memberEPA.CurrentProcess.MaxTotalScore;
        }
        else if (totalScore < 0) {
            totalScore = 0;
        }

        memberEPA.CurrentProcess.TotalScore = totalScore;
        memberEPA.FinalScore = totalScore;

        if (memberEPA.HasDisciplineScore) {
            memberEPA.FinalScore += memberEPA.DisciplineScore;
        }

        if (memberEPA.FinalScore > memberEPA.MaxFinalScore) {
            memberEPA.FinalScore = memberEPA.MaxFinalScore;
        }
        else if (memberEPA.FinalScore < 0) {
            memberEPA.FinalScore = 0;
        }
    };

    vm.checkAll = function (selectedAll) {
        angular.forEach(vm.memberEPAs, function (memberEPA) {
            if (!memberEPA.CurrentProcess.IsEndProcess) {
                memberEPA.Selected = selectedAll;
            }
            else {
                memberEPA.Selected = false;
            }
        });
    };

    vm.getNotificationInputScore = function (processByReviewer) {
        if (processByReviewer) {
            return EmployeeEPAModel.getNotificationInputScore(processByReviewer.MaxTotalScore, vm.scoreInputValidMessage);
        }

        return '';
    };

    vm.inputScore = function (memberEPA) {
        calculateFinalScore(memberEPA);
    };

    vm.adjustScore = function () {
        var selectedEmployees = EmployeeEPAModel.getSelectedEmployee(vm.memberEPAs);

        if (selectedEmployees.length === 0) {
            notificationFactory.error("SelectEmployeeBeforeAdjustScoreWarning");
        }
        else {
            EmployeeEPAModel.openPopupAdjustScore(vm, selectedEmployees, true).then(function (adjust) {
                calculateAdjustScore(vm.memberEPAs, adjust);
            })['finally'](function () {
                vm.modalInstance = null;
            });
        }
    };

    vm.saveAdjustScore = function () {
        var selectedEmployees = EmployeeEPAModel.getSelectedEmployee(vm.memberEPAs);

        if (selectedEmployees.length === 0) {
            notificationFactory.error("SelectEmployeeBeforeSaveWarning");
        }
        else {
            EmployeeEPAModel.adjustScoreByReviewer(vm, selectedEmployees);
        }
    };

    vm.updateComment = function (memberEPA, comment, isEdit) {
        EmployeeEPAModel.openPopupUpdateComment(vm, memberEPA, comment, isEdit, true).then(function (commentUpdate) {
            var index = vm.memberEPAs.map(function (e) { return e.EPAId; }).indexOf(memberEPA.EPAId);

            if (index !== -1) {
                vm.memberEPAs[index].CurrentProcess.TotalScore = vm.memberEPAs[index].CurrentProcess.TotalScoreOriginal;
                vm.memberEPAs[index].CurrentProcess.AdjustScore = vm.memberEPAs[index].CurrentProcess.AdjustScoreOriginal;
                vm.memberEPAs[index].CurrentProcess.Comment = commentUpdate;

                calculateFinalScore(vm.memberEPAs[index]);
            }
        })['finally'](function () {
            vm.modalInstance = null;
        });
    };

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByReviewer(filter.groupId, filter.employeeIds);
    };

    vm.initialize = function () {
        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });

        $translate('AdjustScoreInvalid').then(function (message) {
            vm.adjustScoreInvalidMessage = message;
        });

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

epaModule.controller('MemberEPAInfoOfReviewerController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function ($window, $translate, $routeParams, EmployeeEPAModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.epaId = parseInt($routeParams.id);
    vm.actorUserId = parseInt($routeParams.actorUserId);

    vm.dataSaved = false;
    vm.isReadonly = false;
    vm.evaluatedDate = null;
    vm.finalScore = null;

    vm.standardGrades = CommonModel.standardGrades;

    var checkStatus = function (epaInfo) {
        if (epaInfo.CurrentProcess.IsEndProcess) {
            vm.isReadonly = true;
        }
    };

    var calculateScore = function (epaInfo) {
        var totalScore = epaInfo.CurrentProcess.TotalScore ? parseFloat(epaInfo.CurrentProcess.TotalScore) : 0;

        if (epaInfo.HasDisciplineScore) {
            vm.finalScore = totalScore + epaInfo.DisciplineScore;
            epaInfo.CurrentProcess.AdjustScore = totalScore - (epaInfo.AverageScoreByEvaluators - epaInfo.DisciplineScore);
        }
        else {
            vm.finalScore = totalScore;
            epaInfo.CurrentProcess.AdjustScore = totalScore - epaInfo.AverageScoreByEvaluators;
        }

        epaInfo.CurrentProcess.AdjustScoreOriginal = epaInfo.CurrentProcess.AdjustScore;
        epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, epaInfo.MaxFinalScore, vm.standardGrades);
    };

    vm.openPopupViewComment = function (comment) {
        EmployeeEPAModel.openPopupViewComment(vm, comment);
    };

    vm.range = function (n) {
        return new Array(n);
    };

    vm.getClassProgress = function (statusEndProcess) {
        return EmployeeEPAModel.getClassProgress(statusEndProcess);
    };

    vm.getNotificationInputScore = function (processByReviewer) {
        if (processByReviewer) {
            return EmployeeEPAModel.getNotificationInputScore(processByReviewer.MaxTotalScore, vm.scoreInputValidMessage);
        }

        return '';
    };

    vm.inputScore = function (processByReviewer) {
        if (!EmployeeEPAModel.validateScoreInput(processByReviewer.TotalScore, processByReviewer.MaxTotalScore, vm.scoreInputValidMessage)) {
            processByReviewer.TotalScore = null;
        }

        calculateScore(vm.epaInfo);
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
        var totalcore = epaInfo.CurrentProcess.TotalScore,
            comment = epaInfo.CurrentProcess.Comment;
        if (EmployeeEPAModel.validateEvaluationOfReviewerOrManagement(totalcore)) {
            var model = new Array(epaInfo);

            EmployeeEPAModel.adjustScoreByReviewer(vm, model);
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };

    var getEPAInfoByReviewer = function () {
        EmployeeEPAModel.getEPAInfoByReviewer(vm.epaId, vm.actorUserId).then(function () {
            var totalScore, disciplineScore;

            vm.epaInfo = EmployeeEPAModel.epaInfo;

            disciplineScore = vm.epaInfo.MaxDisciplineScore - vm.epaInfo.DisciplineScore;

            if (disciplineScore < 0) {
                disciplineScore = 0;
            }

            vm.epaInfo.DisciplineScore = disciplineScore;

            if (vm.epaInfo.AverageScoreByEvaluators > 0 && !vm.epaInfo.CurrentProcess.TotalScore && vm.epaInfo.ProcessByEvaluators[0].IsEndProcess) {
                vm.epaInfo.CurrentProcess.TotalScore = vm.epaInfo.AverageScoreByEvaluators;
            }

            vm.epaInfo.CurrentProcess.TotalScore = vm.epaInfo.CurrentProcess.TotalScore ? parseFloat(vm.epaInfo.CurrentProcess.TotalScore) : 0;
            totalScore = vm.epaInfo.CurrentProcess.TotalScore;

            if (vm.epaInfo.HasDisciplineScore) {
                vm.finalScore = totalScore + vm.epaInfo.DisciplineScore;

                totalScore = vm.epaInfo.ProcessBySelf.TotalScore ? parseFloat(vm.epaInfo.ProcessBySelf.TotalScore) : 0
                vm.epaInfo.ProcessBySelf.TotalScore = totalScore + vm.epaInfo.DisciplineScore;

                angular.forEach(vm.epaInfo.ProcessByEvaluators, function (evaluator) {
                    totalScore = evaluator.TotalScore ? parseFloat(evaluator.TotalScore) : 0
                    evaluator.TotalScore = totalScore + vm.epaInfo.DisciplineScore;
                });

                vm.epaInfo.AverageScoreByEvaluators = vm.epaInfo.AverageScoreByEvaluators + vm.epaInfo.DisciplineScore;
            }
            else {
                vm.finalScore = totalScore;
            }

            vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.epaInfo.MaxFinalScore, vm.standardGrades);
            vm.evaluatedDate = vm.epaInfo.CurrentProcess.EvaluateDate ? vm.epaInfo.CurrentProcess.EvaluateDate : null;
            vm.epaInfo = vm.epaInfo;
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

        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;

            getEPAInfoByReviewer();
        });
    };

    vm.initialize();
}]);