'use strict';
/*global epaModule*/

epaModule.controller('ManagementController',
['$translate', '$location', 'EmployeeEPAModel', 'ReviewerManagementModel', 'CommonModel',
    'utilitiesFactory', 'notificationFactory', 'enumerationsFactory', 'historyFactory', 'baseUrl',
function ($translate, $location, EmployeeEPAModel, ReviewerManagementModel, CommonModel,
    utilitiesFactory, notificationFactory, enumerationsFactory, historyFactory, baseUrl) {
    var vm = this;

    vm.permission = 0;
    vm.memberEPAs = [];
    vm.members = [];
    vm.standardGrades = CommonModel.standardGrades;
    vm.selectedAll = false;
    vm.averageScore = 0;
    vm.isEndProcessAll = false;
    vm.policyEnum = enumerationsFactory.policy;
    vm.targetTypeEnum = enumerationsFactory.targetType;

    vm.filter = {};

    function initFilter() {
        var filter = historyFactory.getState();
        return filter ? filter : {
            groupId: '',
            isLocal: '',
            employeeIds: ''
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
            }
        };
    }

    function sortBySelfScore(epaInfo1, epaInfo2) {
        var tempa = epaInfo1.Process.Self.TotalScore ? parseFloat(epaInfo1.Process.Self.TotalScore) : 0,
            tempb = epaInfo2.Process.Self.TotalScore ? parseFloat(epaInfo2.Process.Self.TotalScore) : 0;

        return tempa - tempb;
    }

    function sortByEvaluatorScore(epaInfo1, epaInfo2) {
        return epaInfo1.Process.AverageScoreByEvaluators - epaInfo2.Process.AverageScoreByEvaluators;
    }

    function sortByReviewerScore(epaInfo1, epaInfo2) {
        return epaInfo1.Process.AverageScoreByReviewers - epaInfo2.Process.AverageScoreByReviewers;
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
        }

        return cssClass;
    };

    vm.changeSorting = function (field) {
        switch (field) {
            case 'SelfScore':
                vm.sortBy.evaluatorScore = {
                    ascending: false,
                    descending: false
                };
                vm.sortBy.reviewerScore = {
                    ascending: false,
                    descending: false
                };
                if (!vm.sortBy.selfScore.ascending) {
                    vm.sortBy.selfScore.ascending = true;
                    vm.sortBy.selfScore.descending = false;
                    vm.memberEPAs.sort(sortBySelfScore);
                } else {
                    vm.sortBy.selfScore.ascending = false;
                    vm.sortBy.selfScore.descending = true;
                    vm.memberEPAs.reverse();
                }
                break;
            case 'EvaluatorScore':
                vm.sortBy.selfScore = {
                    ascending: false,
                    descending: false
                };
                vm.sortBy.reviewerScore = {
                    ascending: false,
                    descending: false
                };
                if (!vm.sortBy.evaluatorScore.ascending) {
                    vm.sortBy.evaluatorScore.ascending = true;
                    vm.sortBy.evaluatorScore.descending = false;
                    vm.memberEPAs.sort(sortByEvaluatorScore);
                } else {
                    vm.sortBy.evaluatorScore.ascending = false;
                    vm.sortBy.evaluatorScore.descending = true;
                    vm.memberEPAs.reverse();
                }
                break;
            case 'ReviewerScore':
                vm.sortBy.selfScore = {
                    ascending: false,
                    descending: false
                };
                vm.sortBy.evaluatorScore = {
                    ascending: false,
                    descending: false
                };
                if (!vm.sortBy.reviewerScore.ascending) {
                    vm.sortBy.reviewerScore.ascending = true;
                    vm.sortBy.reviewerScore.descending = false;
                    vm.memberEPAs.sort(sortByReviewerScore);
                } else {
                    vm.sortBy.reviewerScore.ascending = false;
                    vm.sortBy.reviewerScore.descending = true;
                    vm.memberEPAs.reverse();
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

    vm.viewEPADetail = function (memberEPA) {
        if (ReviewerManagementModel.checkAdjustScoreChanged(vm.memberEPAs)) {
            ReviewerManagementModel.openPopupRemindSaveChange(vm).then(function () {
                vm.viewEPADetailUrl = baseUrl + 'Management/Detail/' + memberEPA.EPAId;
                $location.url(vm.viewEPADetailUrl);
            })['finally'](function () {
                vm.modalInstance = '';
            });
        } else {
            if (memberEPA.HasMonthlyEPA && memberEPA.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
                vm.viewEPADetailUrl = baseUrl + 'Management/SemiDetail/' + memberEPA.EPAId;
            } else {
                vm.viewEPADetailUrl = baseUrl + 'Management/Detail/' + memberEPA.EPAId;
            }
        }
    };

    vm.viewEPAResult = function (memberEPA) {
        if (memberEPA.HasMonthlyEPA && memberEPA.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
            vm.viewEPAResultUrl = baseUrl + 'EPA/SemiDetail/' + memberEPA.EPAId;
        } else {
            vm.viewEPAResultUrl = baseUrl + 'EPA/Detail/' + memberEPA.EPAId;
        }
    };

    vm.viewOtherViolationDetail = function (memberEPA) {
        CommonModel.openPopupViewOtherViolationDetail(memberEPA.EPAId);
    };

    vm.checkAll = function (selectedAll) {
        angular.forEach(vm.memberEPAs, function (memberEPA) {
            if (!memberEPA.Process.CurrentProcess.IsEndProcess) {
                memberEPA.Selected = selectedAll;
            } else {
                memberEPA.Selected = false;
            }
        });
    };

    vm.getNotificationInputScore = function (memberEPA) {
        var maxSubtractScore = memberEPA.Process.CurrentProcess.MaxSubtractScore,
            maxAdditionScore = memberEPA.Process.CurrentProcess.MaxAdditionScore;

        return EmployeeEPAModel.getNotificationInputScore(maxSubtractScore, maxAdditionScore, vm.scoreInputValidMessage);
    };

    vm.inputScore = function (memberEPA) {
        var maxSubtractScore = memberEPA.Process.CurrentProcess.MaxSubtractScore,
            maxAdditionScore = memberEPA.Process.CurrentProcess.MaxAdditionScore;

        if (!EmployeeEPAModel.validateScoreInput(memberEPA.Process.CurrentProcess.AdjustScore, maxSubtractScore, maxAdditionScore, vm.scoreInputValidMessage)) {
            memberEPA.Process.CurrentProcess.AdjustScore = null;
        }

        ReviewerManagementModel.calculateScore(memberEPA, false);
    };

    vm.adjustScore = function () {
        var selectedEmployees = ReviewerManagementModel.getSelectedEmployee(vm.memberEPAs);

        if (selectedEmployees.length === 0) {
            notificationFactory.error("SelectEmployeeBeforeAdjustScoreWarning");
        } else {
            ReviewerManagementModel.openPopupAdjustScore(vm, selectedEmployees, false).then(function (adjust) {
                ReviewerManagementModel.calculateAdjustScoreMultiple(selectedEmployees, adjust, false);
            })['finally'](function () {
                vm.modalInstance = null;
            });
        }
    };

    vm.saveAdjustScore = function () {
        var selectedEmployees = ReviewerManagementModel.getSelectedEmployee(vm.memberEPAs);

        if (selectedEmployees.length === 0) {
            notificationFactory.error("SelectEmployeeBeforeAdjustScoreWarning");
        } else {
            ReviewerManagementModel.calculateAdjustScore(selectedEmployees, false);
            ReviewerManagementModel.adjustScoreByManagement(vm, selectedEmployees)['finally'](function () {
                getEPAByManagement(vm.filter.groupId, vm.filter.isLocal, vm.filter.employeeIds, selectedEmployees);
            });
        }
    };

    vm.updateComment = function (memberEPA, comment, isEdit) {
        ReviewerManagementModel.openPopupUpdateComment(vm, memberEPA, comment, isEdit, false)
        .then(function (commentUpdate) {
            var index = vm.memberEPAs.map(function (e) { return e.EPAId; }).indexOf(memberEPA.EPAId);

            if (index !== -1) {
                vm.memberEPAs[index].Process.CurrentProcess.TotalScore = vm.memberEPAs[index].Process.CurrentProcess.TotalScoreOriginal;
                vm.memberEPAs[index].Process.CurrentProcess.AdjustScore = vm.memberEPAs[index].Process.CurrentProcess.AdjustScoreOriginal;
                vm.memberEPAs[index].Process.CurrentProcess.Comment = commentUpdate;
                vm.memberEPAs[index].Process.CurrentProcess.Evaluated = true;

                ReviewerManagementModel.calculateScore(vm.memberEPAs[index], false);
            }
        })['finally'](function () {
            vm.modalInstance = null;
        });
    };

    function getEPAByManagement(groupId, isLocal, employeeIds, selectedEmployees) {
        vm.isEndProcessAll = false;
        resetSorting();

        ReviewerManagementModel.getEPAByManagement(groupId, isLocal, employeeIds)
        .then(function () {
            vm.memberEPAs = ReviewerManagementModel.memberEPAs;

            var averageScore = 0, countEndProcess = 0;
            angular.forEach(vm.memberEPAs, function (memberEPA) {
                if (memberEPA.Process.Reviewers.length > 0
                    && !memberEPA.Process.CurrentProcess.TotalScore
                    && memberEPA.Process.Reviewers[0].IsEndProcess) {
                    memberEPA.Process.CurrentProcess.TotalScore = memberEPA.Process.AverageScoreByReviewers;
                } else if (!memberEPA.Process.CurrentProcess.TotalScore && memberEPA.Process.Evaluators[0].IsEndProcess) {
                    if (memberEPA.HasMonthlyEPA && memberEPA.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
                        memberEPA.Process.CurrentProcess.TotalScore = memberEPA.MonthlyEPA.AverageEvaluatorScoreConverted;
                    } else {
                        memberEPA.Process.CurrentProcess.TotalScore = memberEPA.Process.AverageScoreByEvaluators;
                    }
                }

                memberEPA.Process.Self.TotalScore = utilitiesFactory.parseFloat(memberEPA.Process.Self.TotalScore);
                memberEPA.Process.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(memberEPA.Process.CurrentProcess.TotalScore);
                memberEPA.FinalScore = memberEPA.Process.CurrentProcess.TotalScore + memberEPA.DisciplineScore;
                memberEPA.FinalScore = EmployeeEPAModel.checkScore(memberEPA.FinalScore, 0, memberEPA.Process.CurrentProcess.MaxPerformanceScore);

                if (memberEPA.LastFinalScore.length > 0) {
                    memberEPA.LastFinalScore = utilitiesFactory.parseFloat(memberEPA.LastFinalScore);
                } else {
                    memberEPA.LastFinalScore = null;
                }

                if (memberEPA.Process.CurrentProcess.IsEndProcess) {
                    countEndProcess += 1;
                }
            });

            if (countEndProcess === vm.memberEPAs.length) {
                vm.isEndProcessAll = true;
            }
            vm.averageScore = utilitiesFactory.parseFloat(averageScore / vm.memberEPAs.length);

            if (selectedEmployees && selectedEmployees.length > 0) {
                ReviewerManagementModel.updateSelectedEmployees(vm.memberEPAs, selectedEmployees);
            }
        });

        historyFactory.saveState(vm.filter);
    }

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByManagement(filter.groupId, filter.isLocal, filter.employeeIds);
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

            getEPAByManagement(vm.filter.groupId, vm.filter.isLocal, vm.filter.isMonthly, vm.filter.employeeIds);
        });
    }

    initialize();
}]);

epaModule.controller('MemberEPAInfoOfManagentController',
['$window', '$timeout', '$translate', '$routeParams', 'EmployeeEPAModel', 'ReviewerManagementModel', 'CommonModel',
    'utilitiesFactory', 'enumerationsFactory', 'historyFactory', 'baseUrl',
function ($window, $timeout, $translate, $routeParams, EmployeeEPAModel, ReviewerManagementModel, CommonModel,
    utilitiesFactory, enumerationsFactory, historyFactory, baseUrl) {
    var vm = this;

    vm.permission = 0;
    vm.epaId = parseInt($routeParams.id);
    vm.standardGrades = CommonModel.standardGrades;
    vm.policyEnum = enumerationsFactory.policy;
    vm.targetTypeEnum = enumerationsFactory.targetType;
    vm.statusEndProcessEnum = enumerationsFactory.statusEndProcess;
    vm.ratingTypeEnum = enumerationsFactory.ratingType;
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
            return EmployeeEPAModel.getNotificationInputScore(0, processByManagement.MaxTotalScore, vm.scoreInputValidMessage);
        }

        return '';
    };

    vm.inputScore = function (processByManagement) {
        if (!EmployeeEPAModel.validateScoreInput(processByManagement.TotalScore, 0, processByManagement.MaxTotalScore, vm.scoreInputValidMessage)) {
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

            ReviewerManagementModel.adjustScoreByManagement(vm, model);
        }
    };

    vm.cancel = function () {
        historyFactory.back();
    };

    function getEPAInfoByManagement() {
        ReviewerManagementModel.getEPAInfoByManagement(vm.epaId).then(function () {
            vm.epaInfo = ReviewerManagementModel.epaInfo;
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
                if (vm.epaInfo.HasMonthlyEPA && vm.epaInfo.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
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
            vm.finalScore = EmployeeEPAModel.checkScore(vm.finalScore, 0, vm.epaInfo.Process.CurrentProcess.MaxPerformanceScore);

            vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
            vm.evaluatedDate = vm.epaInfo.Process.CurrentProcess.EvaluateDate ? vm.epaInfo.Process.CurrentProcess.EvaluateDate : null;

            ReviewerManagementModel.getUserPermissionOnManagement().then(function () {
                vm.permission = ReviewerManagementModel.permission;

                if (vm.permission !== vm.policyEnum.Host) {
                    vm.epaInfo.IsAllowedToEdit = false;
                }
            });
        });
    }

    function initialize() {
        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });

        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;

            getEPAInfoByManagement();
        });
    }

    initialize();
}]);