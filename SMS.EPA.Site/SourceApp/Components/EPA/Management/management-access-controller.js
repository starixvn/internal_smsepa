'use strict';
/*global epaModule*/

epaModule.controller('ManagementController',
['$translate', '$location', '$routeParams', 'EmployeeEPAModel', 'ReviewerManagementModel', 'CommonModel',
    'utilitiesFactory', 'notificationFactory', 'historyFactory', 'lockScreenService', 'baseUrl',
function ($translate, $location, $routeParams, EmployeeEPAModel, ReviewerManagementModel, CommonModel,
    utilitiesFactory, notificationFactory, historyFactory, lockScreenService, baseUrl) {
    var vm = this;

    vm.permission = 0;
    vm.epaInfos = [];
    vm.members = [];
    vm.standardGrades = CommonModel.standardGrades;
    vm.selectedAll = false;
    vm.averageScore = 0;
    vm.isEndProcessAll = false;
    vm.policyEnum = utilitiesFactory.policy;
    vm.targetTypeEnum = utilitiesFactory.targetType;

    vm.filter = {};

    function initFilter() {
        var filter = historyFactory.getState();
        return filter ? filter : {
            groupId: '',
            isLocal: '',
            employeeIds: '',
            statusProcess: $routeParams.statusProcess ? $routeParams.statusProcess : ''
        };
    }

    function resetSorting() {
        vm.sortBy = {
            selfScore: {
                ascending: false, descending: false
            },
            evaluatorScore: {
                ascending: false, descending: false
            },
            reviewerScore: {
                ascending: false, descending: false
            },
            disciplineScore: {
                ascending: false, descending: false
            },
            totalScore: {
                ascending: false, descending: false
            }
        };
    }

    function sortBySelfScore(epaInfo1, epaInfo2) {
        var tempa = epaInfo1.Process.Self.TotalScore ? parseFloat(epaInfo1.Process.Self.TotalScore) : 0,
            tempb = epaInfo2.Process.Self.TotalScore ? parseFloat(epaInfo2.Process.Self.TotalScore) : 0;

        if (epaInfo1.HasMonthlyEPA && epaInfo1.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
            tempa = epaInfo1.MonthlyEPA.AverageSelfScoreConverted;
        }

        if (epaInfo2.HasMonthlyEPA && epaInfo2.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
            tempb = epaInfo2.MonthlyEPA.AverageSelfScoreConverted;
        }

        return tempa - tempb;
    }

    function sortByEvaluatorScore(epaInfo1, epaInfo2) {
        var tempa = epaInfo1.Process.AverageScoreByEvaluators,
            tempb = epaInfo2.Process.AverageScoreByEvaluators;

        if (epaInfo1.HasMonthlyEPA && epaInfo1.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
            tempa = epaInfo1.MonthlyEPA.AverageEvaluatorScoreConverted;
        }

        if (epaInfo2.HasMonthlyEPA && epaInfo2.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
            tempb = epaInfo2.MonthlyEPA.AverageEvaluatorScoreConverted;
        }

        return tempa - tempb;
    }

    function sortByReviewerScore(epaInfo1, epaInfo2) {
        return epaInfo1.Process.AverageScoreByReviewers - epaInfo2.Process.AverageScoreByReviewers;
    }

    function sortByDisciplineScore(epaInfo1, epaInfo2) {
        return epaInfo1.DisciplineScore - epaInfo2.DisciplineScore;
    }

    function sortByTotalScore(epaInfo1, epaInfo2) {
        return epaInfo1.FinalScore - epaInfo2.FinalScore;
    }

    vm.selectedCls = function (field) {
        var cssClass = '';

        switch (field) {
            case 'SelfScore':
                if (vm.sortBy.selfScore.ascending) {
                    cssClass = 'sorting-asc';
                } else if (vm.sortBy.selfScore.descending) {
                    cssClass = 'sorting-desc';
                }
                break;
            case 'EvaluatorScore':
                if (vm.sortBy.evaluatorScore.ascending) {
                    cssClass = 'sorting-asc';
                } else if (vm.sortBy.evaluatorScore.descending) {
                    cssClass = 'sorting-desc';
                }
                break;
            case 'ReviewerScore':
                if (vm.sortBy.reviewerScore.ascending) {
                    cssClass = 'sorting-asc';
                } else if (vm.sortBy.reviewerScore.descending) {
                    cssClass = 'sorting-desc';
                }
                break;
            case 'DisciplineScore':
                if (vm.sortBy.disciplineScore.ascending) {
                    cssClass = 'sorting-asc';
                } else if (vm.sortBy.disciplineScore.descending) {
                    cssClass = 'sorting-desc';
                }
                break;
            case 'TotalScore':
                if (vm.sortBy.totalScore.ascending) {
                    cssClass = 'sorting-asc';
                } else if (vm.sortBy.totalScore.descending) {
                    cssClass = 'sorting-desc';
                }
                break;
        }

        return cssClass;
    };

    vm.changeSorting = function (field) {
        switch (field) {
            case 'SelfScore':
                vm.sortBy.evaluatorScore = { ascending: false, descending: false };
                vm.sortBy.reviewerScore = { ascending: false, descending: false };
                vm.sortBy.disciplineScore = { ascending: false, descending: false };
                vm.sortBy.totalScore = { ascending: false, descending: false };

                if (!vm.sortBy.selfScore.ascending) {
                    vm.sortBy.selfScore = { ascending: true, descending: false };
                    vm.epaInfos.sort(sortBySelfScore);
                } else {
                    vm.sortBy.selfScore = { ascending: false, descending: true };
                    vm.epaInfos.reverse();
                }
                break;
            case 'EvaluatorScore':
                vm.sortBy.selfScore = { ascending: false, descending: false };
                vm.sortBy.reviewerScore = { ascending: false, descending: false };
                vm.sortBy.disciplineScore = { ascending: false, descending: false };
                vm.sortBy.totalScore = { ascending: false, descending: false };

                if (!vm.sortBy.evaluatorScore.ascending) {
                    vm.sortBy.evaluatorScore = { ascending: true, descending: false };
                    vm.epaInfos.sort(sortByEvaluatorScore);
                } else {
                    vm.sortBy.evaluatorScore = { ascending: false, descending: true };
                    vm.epaInfos.reverse();
                }
                break;
            case 'ReviewerScore':
                vm.sortBy.selfScore = { ascending: false, descending: false };
                vm.sortBy.evaluatorScore = { ascending: false, descending: false };
                vm.sortBy.disciplineScore = { ascending: false, descending: false };
                vm.sortBy.totalScore = { ascending: false, descending: false };

                if (!vm.sortBy.reviewerScore.ascending) {
                    vm.sortBy.reviewerScore = { ascending: true, descending: false };
                    vm.epaInfos.sort(sortByReviewerScore);
                } else {
                    vm.sortBy.reviewerScore = { ascending: false, descending: true };
                    vm.epaInfos.reverse();
                }
                break;
            case 'DisciplineScore':
                vm.sortBy.selfScore = { ascending: false, descending: false };
                vm.sortBy.evaluatorScore = { ascending: false, descending: false };
                vm.sortBy.reviewerScore = { ascending: false, descending: false };
                vm.sortBy.totalScore = { ascending: false, descending: false };

                if (!vm.sortBy.disciplineScore.ascending) {
                    vm.sortBy.disciplineScore = { ascending: true, descending: false };
                    vm.epaInfos.sort(sortByDisciplineScore);
                } else {
                    vm.sortBy.disciplineScore = { ascending: false, descending: true };
                    vm.epaInfos.reverse();
                }
                break;
            case 'TotalScore':
                vm.sortBy.selfScore = { ascending: false, descending: false };
                vm.sortBy.evaluatorScore = { ascending: false, descending: false };
                vm.sortBy.reviewerScore = { ascending: false, descending: false };
                vm.sortBy.disciplineScore = { ascending: false, descending: false };

                if (!vm.sortBy.totalScore.ascending) {
                    vm.sortBy.totalScore = { ascending: true, descending: false };
                    vm.epaInfos.sort(sortByTotalScore);
                } else {
                    vm.sortBy.totalScore = { ascending: false, descending: true };
                    vm.epaInfos.reverse();
                }
                break;
        }
    };

    vm.getMember = function (groupId, isLocal) {
        ReviewerManagementModel.getMemberByManagement(groupId, isLocal).then(function () {
            vm.members = ReviewerManagementModel.members;
            utilitiesFactory.setSelectedItem(vm.members, "EmployeeId", vm.filter.employeeIds);
        });
    };

    vm.viewEPADetail = function (epaInfo) {
        if (ReviewerManagementModel.checkAdjustScoreChanged(vm.epaInfos)) {
            ReviewerManagementModel.openPopupRemindSaveChange(vm).then(function () {
                vm.viewEPADetailUrl = baseUrl + 'Management/Detail/' + epaInfo.EPAId;
                $location.url(vm.viewEPADetailUrl);
            })['finally'](function () {
                vm.modalInstance = '';
            });
        } else {
            vm.viewEPADetailUrl = baseUrl + 'Management/Detail/' + epaInfo.EPAId;
        }
    };

    vm.viewEPAResult = function (epaInfo) {
        vm.viewEPAResultUrl = baseUrl + 'EPA/Detail/' + epaInfo.EPAId;
    };

    vm.viewOtherViolationDetail = function (epaInfo) {
        CommonModel.openPopupViewOtherViolationDetail(epaInfo.EPAId);
    };

    vm.checkAll = function (selectedAll) {
        angular.forEach(vm.epaInfos, function (epaInfo) {
            if (!epaInfo.Process.CurrentProcess.IsEndProcess) {
                epaInfo.Selected = selectedAll;
            } else {
                epaInfo.Selected = false;
            }
        });
    };

    vm.getNotificationInputScore = function (epaInfo) {
        var maxSubtractScore = epaInfo.Process.CurrentProcess.MaxSubtractScore,
            maxAdditionScore = epaInfo.Process.CurrentProcess.MaxAdditionScore;

        return EmployeeEPAModel.getNotificationInputScore(maxSubtractScore, maxAdditionScore, vm.scoreInputValidMessage);
    };

    vm.inputScore = function (epaInfo) {
        var maxSubtractScore = epaInfo.Process.CurrentProcess.MaxSubtractScore,
            maxAdditionScore = epaInfo.Process.CurrentProcess.MaxAdditionScore;

        if (!EmployeeEPAModel.validateScoreInput(epaInfo.Process.CurrentProcess.AdjustScore, maxSubtractScore, maxAdditionScore, vm.scoreInputValidMessage)) {
            epaInfo.Process.CurrentProcess.AdjustScore = null;
        }

        ReviewerManagementModel.calculateScore(epaInfo, false);
        epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(epaInfo.FinalScore, vm.standardGrades);
    };

    vm.adjustScore = function () {
        var epaInfos = ReviewerManagementModel.getEPASelected(vm.epaInfos);

        if (epaInfos.length === 0) {
            notificationFactory.error("SelectEmployeeBeforeAdjustScoreWarning");
        } else {
            ReviewerManagementModel.openPopupAdjustScore(vm, epaInfos, false).then(function (adjust) {
                ReviewerManagementModel.calculateAdjustScoreMultiple(epaInfos, adjust, false);
                angular.forEach(epaInfos, function (epaInfo) {
                    epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(epaInfo.FinalScore, vm.standardGrades);
                });
            })['finally'](function () {
                vm.modalInstance = null;
            });
        }
    };

    vm.saveAdjustScore = function () {
        var epaInfos = ReviewerManagementModel.getEPASelected(vm.epaInfos);

        if (epaInfos.length === 0) {
            notificationFactory.error("SelectEmployeeBeforeAdjustScoreWarning");
        } else {
            ReviewerManagementModel.calculateAdjustScore(epaInfos, false);
            ReviewerManagementModel.adjustScoreByManagement(vm, epaInfos)['finally'](function () {
                getEPAByManagement(vm.filter.groupId, vm.filter.isLocal, vm.filter.employeeIds, epaInfos, vm.filter.statusProcess);
            });
        }
    };

    vm.updateComment = function (epaInfo, comment, isEdit) {
        ReviewerManagementModel.openPopupUpdateComment(vm, epaInfo, comment, isEdit, false).then(function (commentUpdate) {
            var index = vm.epaInfos.map(function (e) { return e.EPAId; }).indexOf(epaInfo.EPAId);

            if (index !== -1) {
                vm.epaInfos[index].Process.CurrentProcess.TotalScore = vm.epaInfos[index].Process.CurrentProcess.TotalScoreOriginal;
                vm.epaInfos[index].Process.CurrentProcess.AdjustScore = vm.epaInfos[index].Process.CurrentProcess.AdjustScoreOriginal;
                vm.epaInfos[index].Process.CurrentProcess.Comment = commentUpdate;
                vm.epaInfos[index].Process.CurrentProcess.Evaluated = true;

                ReviewerManagementModel.calculateScore(vm.epaInfos[index], false);
                vm.epaInfos[index].Grade = EmployeeEPAModel.convertScoreToGrade(vm.epaInfos[index].FinalScore, vm.standardGrades);
            }
        })['finally'](function () {
            vm.modalInstance = null;
        });
    };

    function getEPAByManagement(groupId, isLocal, employeeIds, selectedEmployees, statusProcess) {
        lockScreenService.start();
        vm.isEndProcessAll = false;
        resetSorting();

        ReviewerManagementModel.getEPAByManagement(groupId, isLocal, employeeIds, statusProcess).then(function () {
            vm.epaInfos = ReviewerManagementModel.epaInfos;

            var averageScore = 0, countEndProcess = 0;
            angular.forEach(vm.epaInfos, function (epaInfo) {
                if (!epaInfo.Process.CurrentProcess.TotalScore && epaInfo.Process.Reviewers[0].IsEndProcess) {
                    epaInfo.Process.CurrentProcess.TotalScore = epaInfo.Process.AverageScoreByReviewers;
                }

                epaInfo.Process.Self.TotalScore = utilitiesFactory.parseFloat(epaInfo.Process.Self.TotalScore);
                epaInfo.Process.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(epaInfo.Process.CurrentProcess.TotalScore);
                epaInfo.FinalScore = epaInfo.Process.CurrentProcess.TotalScore + epaInfo.DisciplineScore;
                epaInfo.FinalScore = EmployeeEPAModel.checkScore(epaInfo.FinalScore, 0, epaInfo.Process.CurrentProcess.MaxPerformanceScore);
                epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(epaInfo.FinalScore, vm.standardGrades);

                if (epaInfo.LastFinalScore.length > 0) {
                    epaInfo.LastFinalScore = utilitiesFactory.parseFloat(epaInfo.LastFinalScore);
                } else {
                    epaInfo.LastFinalScore = null;
                }

                if (epaInfo.Process.CurrentProcess.IsEndProcess) {
                    countEndProcess += 1;
                }

                averageScore += epaInfo.Process.AverageScoreByReviewers;
            });

            if (countEndProcess === vm.epaInfos.length) {
                vm.isEndProcessAll = true;
            }
            vm.averageScore = utilitiesFactory.parseFloat(averageScore / vm.epaInfos.length);

            if (selectedEmployees && selectedEmployees.length > 0) {
                ReviewerManagementModel.updateSelectedEmployees(vm.epaInfos, selectedEmployees);
            }

            lockScreenService.stop();
        });

        historyFactory.saveState(vm.filter);
    }

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByManagement(filter.groupId, filter.isLocal, filter.employeeIds, null, filter.statusProcess);
    };

    function initialize() {
        vm.filter = initFilter();
        resetSorting();

        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });

        $translate('AdjustScoreInvalid').then(function (message) {
            vm.adjustScoreInvalidMessage = message;
        });

        ReviewerManagementModel.getUserPermissionOnManagement().then(function () {
            vm.permission = ReviewerManagementModel.permission;
        });

        ReviewerManagementModel.getGroupByManagement().then(function () {
            vm.groupOptions = ReviewerManagementModel.groups;
        });

        vm.getMember(vm.filter.groupId, vm.filter.isLocal);

        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;

            getEPAByManagement(vm.filter.groupId, vm.filter.isLocal, vm.filter.employeeIds, vm.filter.statusProcess);
        });
    }

    initialize();
}]);