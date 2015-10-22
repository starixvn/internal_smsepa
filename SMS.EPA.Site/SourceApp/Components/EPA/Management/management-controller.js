'use strict';
/*global epaModule*/

epaModule.controller('ManagementController',
['$translate', '$location', 'EmployeeEPAModel', 'CommonModel', 'utilitiesFactory', 'notificationFactory',
function ($translate, $location, EmployeeEPAModel, CommonModel, utilitiesFactory, notificationFactory) {
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
        EmployeeEPAModel.getMemberByManagement(groupId).then(function () {
            vm.members = EmployeeEPAModel.members;
        });
    };

    vm.viewEPADetail = function (memberEPA) {
        var hasChangeScore = false;

        angular.forEach(vm.memberEPAs, function (item) {
            var adjustScoreOriginal = item.CurrentProcess.AdjustScoreOriginal ? parseFloat(item.CurrentProcess.AdjustScoreOriginal) : 0,
                adjustScore = item.CurrentProcess.AdjustScore ? parseFloat(item.CurrentProcess.AdjustScore) : 0;

            if (adjustScoreOriginal !== adjustScore) {
                hasChangeScore = true;
            }
        });

        if (hasChangeScore) {
            EmployeeEPAModel.openPopupRemindSaveChange(vm).then(function () {
                vm.viewEPADetailUrl = '/Management/Detail/' + memberEPA.EPAId;
                $location.url(vm.viewEPADetailUrl);
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }
        else {
            vm.viewEPADetailUrl = '/Management/Detail/' + memberEPA.EPAId;
        }
        
    };

    vm.viewEPAResult = function (memberEPA) {
        vm.viewEPAResultUrl = '/EPA/Detail/' + memberEPA.EPAId;
    };

    var getEPAByManagement = function (groupId, employeeIds) {
        EmployeeEPAModel.getEPAByManagement(groupId, employeeIds).then(function () {
            vm.memberEPAs = EmployeeEPAModel.memberEPAs;

            var averageScore = 0;
            var countEndProcess = 0;
            angular.forEach(vm.memberEPAs, function (memberEPA) {
                var totalScore,
                    disciplineScore = memberEPA.MaxDisciplineScore - memberEPA.DisciplineScore;

                if (disciplineScore < 0) {
                    disciplineScore = 0;
                }

                if (memberEPA.ProcessByReviewers.length > 0 && !memberEPA.CurrentProcess.TotalScore && memberEPA.ProcessByReviewers[0].IsEndProcess) {
                    memberEPA.CurrentProcess.TotalScore = memberEPA.AverageScoreByReviewers;
                }
                else if (memberEPA.ProcessByEvaluators.length > 0 && !memberEPA.CurrentProcess.TotalScore && memberEPA.ProcessByEvaluators[0].IsEndProcess) {
                    memberEPA.CurrentProcess.TotalScore = memberEPA.AverageScoreByEvaluators;
                }

                totalScore = memberEPA.CurrentProcess.TotalScore ? parseFloat(memberEPA.CurrentProcess.TotalScore) : 0;

                memberEPA.DisciplineScore = disciplineScore;
                memberEPA.CurrentProcess.TotalScore = totalScore;
                memberEPA.FinalScore = totalScore;
                memberEPA.ProcessBySelf.TotalScore = parseFloat(memberEPA.ProcessBySelf.TotalScore);

                if (memberEPA.HasDisciplineScore) {
                    memberEPA.FinalScore += memberEPA.DisciplineScore;
                    memberEPA.ProcessBySelf.TotalScore += memberEPA.DisciplineScore;
                    memberEPA.AverageScoreByEvaluators += memberEPA.DisciplineScore;

                    if (memberEPA.ProcessByReviewers.length > 0) {
                        memberEPA.AverageScoreByReviewers += memberEPA.DisciplineScore;
                    }
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

                if (memberEPA.ProcessByReviewers.length > 0) {
                    averageScore += memberEPA.AverageScoreByReviewers;
                }
                else {
                    averageScore += memberEPA.AverageScoreByEvaluators;
                }
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

                if (memberEPA.ProcessByReviewers.length > 0) {
                    totalScoreAdjust = memberEPA.AverageScoreByReviewers + scoreAdjust;
                }
                else {
                    totalScoreAdjust = memberEPA.AverageScoreByEvaluators + scoreAdjust;
                }

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

    var calculateFinalScore = function (memberEPA, processManagement) {
        var adjustScore, totalScore;

        adjustScore = memberEPA.CurrentProcess.AdjustScore ? parseFloat(memberEPA.CurrentProcess.AdjustScore) : 0;
        if (memberEPA.ProcessByReviewers.length > 0) {
            totalScore = memberEPA.AverageScoreByReviewers + adjustScore;
        }
        else {
            totalScore = memberEPA.AverageScoreByEvaluators + adjustScore;
        }

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
            memberEPA.FinalScore = totalScore + memberEPA.DisciplineScore;
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
            EmployeeEPAModel.openPopupAdjustScore(vm, selectedEmployees, false).then(function (adjust) {
                calculateAdjustScore(vm.memberEPAs, adjust);
            })['finally'](function () {
                vm.modalInstance = null;
            });
        }
    };

    vm.saveAdjustScore = function () {
        var selectedEmployees = EmployeeEPAModel.getSelectedEmployee(vm.memberEPAs);

        if (selectedEmployees.length === 0) {
            notificationFactory.error("SelectEmployeeBeforeAdjustScoreWarning");
        }
        else {
            EmployeeEPAModel.adjustScoreByManagement(vm, selectedEmployees);
        }
    };

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByManagement(filter.groupId, filter.employeeIds);
    };

    vm.updateComment = function (memberEPA, comment, isEdit) {
        EmployeeEPAModel.openPopupUpdateComment(vm, memberEPA, comment, isEdit, false).then(function (commentUpdate) {
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

epaModule.controller('MemberEPAInfoOfManagentController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function ($window, $translate, $routeParams, EmployeeEPAModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.epaId = parseInt($routeParams.id);

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

            if (epaInfo.ProcessByReviewers.length > 0) {
                epaInfo.CurrentProcess.AdjustScore = totalScore - (epaInfo.AverageScoreByReviewers - epaInfo.DisciplineScore);
            }
            else {
                epaInfo.CurrentProcess.AdjustScore = totalScore - (epaInfo.AverageScoreByEvaluators - epaInfo.DisciplineScore);
            }
        }
        else {
            vm.finalScore = totalScore;

            if (epaInfo.ProcessByReviewers.length > 0) {
                epaInfo.CurrentProcess.AdjustScore = totalScore - epaInfo.AverageScoreByReviewers;
            }
            else {
                epaInfo.CurrentProcess.AdjustScore = totalScore - epaInfo.AverageScoreByEvaluators;
            }
        }

        epaInfo.CurrentProcess.AdjustScoreOriginal = epaInfo.CurrentProcess.AdjustScore;
        epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
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

    vm.getNotificationInputScore = function (processByManagement) {
        if (processByManagement) {
            return EmployeeEPAModel.getNotificationInputScore(processByManagement.MaxTotalScore, vm.scoreInputValidMessage);
        }

        return '';
    };

    vm.inputScore = function (processByManagement) {
        if (!EmployeeEPAModel.validateScoreInput(processByManagement.TotalScore, processByManagement.MaxTotalScore, vm.scoreInputValidMessage)) {
            processByManagement.TotalScore = null;
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

            EmployeeEPAModel.adjustScoreByManagement(vm, model);
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };

    var getEPAInfoByManagement = function () {
        EmployeeEPAModel.getEPAInfoByManagement(vm.epaId).then(function () {
            var totalScore, disciplineScore;

            vm.epaInfo = EmployeeEPAModel.epaInfo;

            disciplineScore = vm.epaInfo.MaxDisciplineScore - vm.epaInfo.DisciplineScore;

            if (disciplineScore < 0) {
                disciplineScore = 0;
            }

            vm.epaInfo.DisciplineScore = disciplineScore;

            if (vm.epaInfo.AverageScoreByReviewers > 0 && !vm.epaInfo.CurrentProcess.TotalScore && vm.epaInfo.ProcessByReviewers[0].IsEndProcess) {
                vm.epaInfo.CurrentProcess.TotalScore = vm.epaInfo.AverageScoreByReviewers;
            }
            else if (vm.epaInfo.AverageScoreByEvaluators > 0 && !vm.epaInfo.CurrentProcess.TotalScore && vm.epaInfo.ProcessByEvaluators[0].IsEndProcess) {
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

                angular.forEach(vm.epaInfo.ProcessByReviewers, function (reviewer) {
                    totalScore = reviewer.TotalScore ? parseFloat(reviewer.TotalScore) : 0
                    reviewer.TotalScore = totalScore + vm.epaInfo.DisciplineScore;
                });

                vm.epaInfo.AverageScoreByReviewers = vm.epaInfo.AverageScoreByReviewers + vm.epaInfo.DisciplineScore;
            }
            else {
                vm.finalScore = totalScore;
            }

            vm.finalScore = vm.finalScore !== 0 ? vm.finalScore : '';
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

        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;

            getEPAInfoByManagement();
        });
    };

    vm.initialize();
}]);