'use strict';
/*global epaModule*/

epaModule.controller('ManagementController',
['$translate', '$location', 'EmployeeEPAModel', 'ReviewerManagementModel', 'CommonModel',
    'utilitiesFactory', 'notificationFactory', 'enumerationsFactory', 'baseUrl',
function ($translate, $location, EmployeeEPAModel, ReviewerManagementModel, CommonModel,
    utilitiesFactory, notificationFactory, enumerationsFactory, baseUrl) {
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

    vm.filter = {
        groupId: '',
        isLocal: '',
        employeeIds: ''
    };

    vm.getMember = function (groupId, isLocal) {
        ReviewerManagementModel.getMemberByManagement(groupId, isLocal).then(function () {
            vm.members = ReviewerManagementModel.members;
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
        }
        else {
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
            if (!memberEPA.CurrentProcess.IsEndProcess) {
                memberEPA.Selected = selectedAll;
            }
            else {
                memberEPA.Selected = false;
            }
        });
    };

    vm.getNotificationInputScore = function (memberEPA) {
        var maxSubtractScore = memberEPA.MaxSubtractScore,
            maxAdditionScore = memberEPA.MaxAdditionScore;

        return EmployeeEPAModel.getNotificationInputScore(maxSubtractScore, maxAdditionScore, vm.scoreInputValidMessage);
    };

    vm.inputScore = function (memberEPA) {
        var maxSubtractScore = memberEPA.MaxSubtractScore,
            maxAdditionScore = memberEPA.MaxAdditionScore;

        if (!EmployeeEPAModel.validateScoreInput(memberEPA.CurrentProcess.AdjustScore, maxSubtractScore, maxAdditionScore, vm.scoreInputValidMessage)) {
            memberEPA.CurrentProcess.AdjustScore = null;
        }

        ReviewerManagementModel.calculateScore(memberEPA, false);
    };

    vm.adjustScore = function () {
        var selectedEmployees = ReviewerManagementModel.getSelectedEmployee(vm.memberEPAs);

        if (selectedEmployees.length === 0) {
            notificationFactory.error("SelectEmployeeBeforeAdjustScoreWarning");
        }
        else {
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
        }
        else {
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
                vm.memberEPAs[index].CurrentProcess.TotalScore = vm.memberEPAs[index].CurrentProcess.TotalScoreOriginal;
                vm.memberEPAs[index].CurrentProcess.AdjustScore = vm.memberEPAs[index].CurrentProcess.AdjustScoreOriginal;
                vm.memberEPAs[index].CurrentProcess.Comment = commentUpdate;
                vm.memberEPAs[index].CurrentProcess.Evaluated = true;

                ReviewerManagementModel.calculateScore(vm.memberEPAs[index], false);
            }
        })['finally'](function () {
            vm.modalInstance = null;
        });
    };

    var getEPAByManagement = function (groupId, isLocal, employeeIds, selectedEmployees) {
        vm.isEndProcessAll = false;

        ReviewerManagementModel.getEPAByManagement(groupId, isLocal, employeeIds)
        .then(function () {
            vm.memberEPAs = ReviewerManagementModel.memberEPAs;

            var averageScore = 0, countEndProcess = 0;
            angular.forEach(vm.memberEPAs, function (memberEPA) {
                memberEPA.MaxPerformanceScore = memberEPA.CurrentProcess.MaxTotalScore;

                if (memberEPA.HasDisciplineScore) {
                    if (memberEPA.DisciplineScore < 0) {
                        memberEPA.DisciplineScore = 0;
                    }

                    memberEPA.MaxPerformanceScore += memberEPA.MaxDisciplineScore;
                }

                if (memberEPA.ProcessByReviewers.length > 0 && !memberEPA.CurrentProcess.TotalScore && memberEPA.ProcessByReviewers[0].IsEndProcess) {
                    memberEPA.CurrentProcess.TotalScore = memberEPA.AverageScoreByReviewers;
                } else if (!memberEPA.CurrentProcess.TotalScore && memberEPA.ProcessByEvaluators[0].IsEndProcess) {
                    if (memberEPA.HasMonthlyEPA && memberEPA.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
                        memberEPA.CurrentProcess.TotalScore = memberEPA.MonthlyEPA.AverageMonthlyEvaluatorScoreConversion;
                    } else {
                        memberEPA.CurrentProcess.TotalScore = memberEPA.AverageScoreByEvaluators;
                    }
                }

                memberEPA.ProcessBySelf.TotalScore = utilitiesFactory.parseFloat(memberEPA.ProcessBySelf.TotalScore);
                memberEPA.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(memberEPA.CurrentProcess.TotalScore);
                memberEPA.FinalScore = memberEPA.CurrentProcess.TotalScore + memberEPA.DisciplineScore;
                memberEPA.FinalScore = EmployeeEPAModel.checkScore(memberEPA.FinalScore, 0, memberEPA.MaxPerformanceScore);

                if (memberEPA.LastFinalScore.length > 0) {
                    memberEPA.LastFinalScore = utilitiesFactory.parseFloat(memberEPA.LastFinalScore);
                } else {
                    memberEPA.LastFinalScore = null;
                }

                if (memberEPA.CurrentProcess.IsEndProcess) {
                    countEndProcess += 1;
                }

                if (memberEPA.ProcessByReviewers.length > 0) {
                    averageScore += memberEPA.AverageScoreByReviewers;
                    memberEPA.MaxSubtractScore = -memberEPA.AverageScoreByReviewers;
                    memberEPA.MaxAdditionScore = utilitiesFactory.parseFloat(memberEPA.CurrentProcess.MaxTotalScore - memberEPA.AverageScoreByReviewers);
                } else {
                    averageScore += memberEPA.AverageScoreByEvaluators;

                    if (memberEPA.HasMonthlyEPA && memberEPA.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
                        memberEPA.MaxSubtractScore = -memberEPA.MonthlyEPA.AverageMonthlyEvaluatorScoreConversion;
                        memberEPA.MaxAdditionScore = utilitiesFactory.parseFloat(memberEPA.CurrentProcess.MaxTotalScore - memberEPA.MonthlyEPA.AverageMonthlyEvaluatorScoreConversion);
                    } else {
                        memberEPA.MaxSubtractScore = -memberEPA.AverageScoreByEvaluators;
                        memberEPA.MaxAdditionScore = utilitiesFactory.parseFloat(memberEPA.CurrentProcess.MaxTotalScore - memberEPA.AverageScoreByEvaluators);
                    }
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
    };

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByManagement(filter.groupId, filter.isLocal, filter.employeeIds);
    };

    vm.initialize = function () {
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
    };

    vm.initialize();
}]);

epaModule.controller('MemberEPAInfoOfManagentController',
['$window', '$timeout','$translate', '$routeParams', 'EmployeeEPAModel', 'ReviewerManagementModel', 'CommonModel',
    'utilitiesFactory', 'enumerationsFactory', 'baseUrl',
function ($window, $timeout, $translate, $routeParams, EmployeeEPAModel, ReviewerManagementModel, CommonModel,
    utilitiesFactory, enumerationsFactory, baseUrl) {
    var vm = this;

    vm.permission = 0;
    vm.epaId = parseInt($routeParams.id);
    vm.standardGrades = CommonModel.standardGrades;
    vm.policyEnum = enumerationsFactory.policy;
    vm.targetTypeEnum = enumerationsFactory.targetType;
    vm.statusEndProcessEnum = enumerationsFactory.statusEndProcess;
    vm.ratingTypeEnum = enumerationsFactory.ratingType;

    vm.dataSaved = false;
    vm.isReadonly = false;
    vm.evaluatedDate = null;
    vm.finalScore = null;

    vm.generateRatingStar = function () {
        $timeout(function () {
            $(".rating-loading").rating();
        }, 1);
    };

    var checkStatus = function (epaInfo) {
        if (epaInfo.CurrentProcess.IsEndProcess) {
            vm.isReadonly = true;
        }
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
        var totalcore = epaInfo.CurrentProcess.TotalScore;

        if (ReviewerManagementModel.validateEvaluation(totalcore)) {
            var model = new Array(epaInfo);

            ReviewerManagementModel.adjustScoreByManagement(vm, model);
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };

    var getEPAInfoByManagement = function () {
        ReviewerManagementModel.getEPAInfoByManagement(vm.epaId).then(function () {
            vm.epaInfo = ReviewerManagementModel.epaInfo;
            checkStatus(vm.epaInfo);

            vm.epaInfo.Employee.Avatar = baseUrl + vm.epaInfo.Employee.Avatar;
            vm.maxPerformanceScore = vm.epaInfo.CurrentProcess.MaxTotalScore;

            if (vm.epaInfo.HasDisciplineScore) {
                if (vm.epaInfo.DisciplineScore < 0) {
                    vm.epaInfo.DisciplineScore = 0;
                }

                vm.maxPerformanceScore += vm.epaInfo.MaxDisciplineScore;
            }

            if (vm.epaInfo.ProcessByReviewers.length > 0 && vm.epaInfo.ProcessByReviewers[0].IsEndProcess && !vm.epaInfo.CurrentProcess.TotalScore) {
                vm.epaInfo.CurrentProcess.TotalScore = vm.epaInfo.AverageScoreByReviewers;
            } else if (vm.epaInfo.ProcessByEvaluators[0].IsEndProcess && !vm.epaInfo.CurrentProcess.TotalScore) {
                if (vm.epaInfo.HasMonthlyEPA && vm.epaInfo.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
                    vm.epaInfo.CurrentProcess.TotalScore = vm.epaInfo.MonthlyEPA.AverageMonthlyEvaluatorScoreConversion;
                } else {
                    vm.epaInfo.CurrentProcess.TotalScore = vm.epaInfo.AverageScoreByEvaluators;
                }
            }

            // self
            vm.epaInfo.ProcessBySelf.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.ProcessBySelf.TotalScore);

            // evaluator
            angular.forEach(vm.epaInfo.ProcessByEvaluators, function (evaluator) {
                evaluator.TotalScore = utilitiesFactory.parseFloat(evaluator.TotalScore);
            });

            // reviewer
            angular.forEach(vm.epaInfo.ProcessByReviewers, function (reviewer) {
                reviewer.TotalScore = utilitiesFactory.parseFloat(reviewer.TotalScore);
            });

            // management
            vm.epaInfo.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.CurrentProcess.TotalScore);
            vm.finalScore = vm.epaInfo.CurrentProcess.TotalScore + vm.epaInfo.DisciplineScore;
            vm.finalScore = EmployeeEPAModel.checkScore(vm.finalScore, 0, vm.maxPerformanceScore);

            vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
            vm.evaluatedDate = vm.epaInfo.CurrentProcess.EvaluateDate ? vm.epaInfo.CurrentProcess.EvaluateDate : null;

            angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
                EmployeeEPAModel.scoreSpread(kpiGroup);
            });

            ReviewerManagementModel.getUserPermissionOnManagement().then(function () {
                vm.permission = ReviewerManagementModel.permission;

                if (vm.permission !== vm.policyEnum.Host) {
                    vm.isReadonly = true;
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

            getEPAInfoByManagement();
        });
    };

    vm.initialize();
}]);