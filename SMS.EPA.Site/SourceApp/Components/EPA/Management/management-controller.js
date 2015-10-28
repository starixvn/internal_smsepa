'use strict';
/*global epaModule*/

epaModule.controller('ManagementController',
['$translate', '$location', 'EmployeeEPAModel', 'ReviewerManagementModel', 'CommonModel', 'utilitiesFactory', 'notificationFactory', 'baseUrl',
function ($translate, $location, EmployeeEPAModel, ReviewerManagementModel, CommonModel, utilitiesFactory, notificationFactory, baseUrl) {
    var vm = this;

    vm.memberEPAs = [];
    vm.members = [];
    vm.standardGrades = CommonModel.standardGrades;
    vm.selectedAll = false;
    vm.averageScore = 0;
    vm.isEndProcessAll = false;

    vm.filter = {
        groupId: '',
        employeeIds: ''
    };

    vm.getMember = function (groupId) {
        ReviewerManagementModel.getMemberByManagement(groupId).then(function () {
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
            vm.viewEPADetailUrl = baseUrl +'Management/Detail/' + memberEPA.EPAId;
        }
    };

    vm.viewEPAResult = function (memberEPA) {
        vm.viewEPAResultUrl = baseUrl + 'EPA/Detail/' + memberEPA.EPAId;
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
            ReviewerManagementModel.adjustScoreByManagement(vm, selectedEmployees);
        }
    };

    vm.updateComment = function (memberEPA, comment, isEdit) {
        ReviewerManagementModel.openPopupUpdateComment(vm, memberEPA, comment, isEdit, false).then(function (commentUpdate) {
            var index = vm.memberEPAs.map(function (e) { return e.EPAId; }).indexOf(memberEPA.EPAId);

            if (index !== -1) {
                vm.memberEPAs[index].CurrentProcess.TotalScore = vm.memberEPAs[index].CurrentProcess.TotalScoreOriginal;
                vm.memberEPAs[index].CurrentProcess.AdjustScore = vm.memberEPAs[index].CurrentProcess.AdjustScoreOriginal;
                vm.memberEPAs[index].CurrentProcess.Comment = commentUpdate;

                ReviewerManagementModel.calculateScore(vm.memberEPAs[index], false);
            }
        })['finally'](function () {
            vm.modalInstance = null;
        });
    };

    var getEPAByManagement = function (groupId, employeeIds) {
        vm.isEndProcessAll = false;

        ReviewerManagementModel.getEPAByManagement(groupId, employeeIds).then(function () {
            vm.memberEPAs = ReviewerManagementModel.memberEPAs;

            var averageScore = 0;
            var countEndProcess = 0;
            angular.forEach(vm.memberEPAs, function (memberEPA) {
                var totalScore,
                    disciplineScore = memberEPA.MaxDisciplineScore - memberEPA.DisciplineScore;

                if (memberEPA.HasDisciplineScore && memberEPA.DisciplineScore < 0) {
                    memberEPA.DisciplineScore = 0;
                }

                if (memberEPA.ProcessByReviewers.length > 0 && !memberEPA.CurrentProcess.TotalScore && memberEPA.ProcessByReviewers[0].IsEndProcess) {
                    memberEPA.CurrentProcess.TotalScore = memberEPA.AverageScoreByReviewers;
                } else if (memberEPA.ProcessByEvaluators.length > 0 && !memberEPA.CurrentProcess.TotalScore && memberEPA.ProcessByEvaluators[0].IsEndProcess) {
                    memberEPA.CurrentProcess.TotalScore = memberEPA.AverageScoreByEvaluators;
                }

                totalScore = utilitiesFactory.parseFloat(memberEPA.CurrentProcess.TotalScore);

                memberEPA.ProcessBySelf.TotalScore = utilitiesFactory.parseFloat(memberEPA.ProcessBySelf.TotalScore);
                memberEPA.CurrentProcess.TotalScore = totalScore;
                memberEPA.FinalScore = totalScore + memberEPA.DisciplineScore;

                if (memberEPA.FinalScore > memberEPA.MaxFinalScore) {
                    memberEPA.FinalScore = memberEPA.MaxFinalScore;
                } else if (memberEPA.FinalScore < 0) {
                    memberEPA.FinalScore = 0;
                }

                if (memberEPA.CurrentProcess.IsEndProcess) {
                    countEndProcess += 1;
                }

                if (memberEPA.ProcessByReviewers.length > 0) {
                    averageScore += memberEPA.AverageScoreByReviewers;
                    memberEPA.MaxSubtractScore = -memberEPA.AverageScoreByReviewers;
                    memberEPA.MaxAdditionScore = memberEPA.CurrentProcess.MaxTotalScore - memberEPA.AverageScoreByReviewers;
                } else {
                    averageScore += memberEPA.AverageScoreByEvaluators;
                    memberEPA.MaxSubtractScore = -memberEPA.AverageScoreByEvaluators;
                    memberEPA.MaxAdditionScore = memberEPA.CurrentProcess.MaxTotalScore - memberEPA.AverageScoreByEvaluators;
                }
            });

            if (countEndProcess === vm.memberEPAs.length) {
                vm.isEndProcessAll = true;
            }
            vm.averageScore = utilitiesFactory.parseFloat(averageScore / vm.memberEPAs.length);
        });
    };

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByManagement(filter.groupId, filter.employeeIds);
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

            getEPAByManagement(vm.filter.groupId, vm.filter.employeeIds);
        });
    };

    vm.initialize();
}]);

epaModule.controller('MemberEPAInfoOfManagentController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'ReviewerManagementModel', 'CommonModel', 'utilitiesFactory',
function ($window, $translate, $routeParams, EmployeeEPAModel, ReviewerManagementModel, CommonModel, utilitiesFactory) {
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
        var totalcore = epaInfo.CurrentProcess.TotalScore,
            comment = epaInfo.CurrentProcess.Comment;
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
            var totalScore, disciplineScore;
            vm.epaInfo = ReviewerManagementModel.epaInfo;

            if (vm.epaInfo.HasDisciplineScore && vm.epaInfo.DisciplineScore < 0) {
                vm.epaInfo.DisciplineScore = 0;
            }

            if (vm.epaInfo.AverageScoreByReviewers > 0 && !vm.epaInfo.CurrentProcess.TotalScore && vm.epaInfo.ProcessByReviewers[0].IsEndProcess) {
                vm.epaInfo.CurrentProcess.TotalScore = vm.epaInfo.AverageScoreByReviewers;
            }
            else if (vm.epaInfo.AverageScoreByEvaluators > 0 && !vm.epaInfo.CurrentProcess.TotalScore && vm.epaInfo.ProcessByEvaluators[0].IsEndProcess) {
                vm.epaInfo.CurrentProcess.TotalScore = vm.epaInfo.AverageScoreByEvaluators;
            }

            // self
            totalScore = utilitiesFactory.parseFloat(vm.epaInfo.ProcessBySelf.TotalScore);
            vm.epaInfo.ProcessBySelf.TotalScore = totalScore + vm.epaInfo.DisciplineScore;

            // evaluator
            angular.forEach(vm.epaInfo.ProcessByEvaluators, function (evaluator) {
                totalScore = utilitiesFactory.parseFloat(evaluator.TotalScore);
                evaluator.TotalScore = totalScore + vm.epaInfo.DisciplineScore;
            });
            vm.epaInfo.AverageScoreByEvaluators = vm.epaInfo.AverageScoreByEvaluators + vm.epaInfo.DisciplineScore;

            // reviewer
            angular.forEach(vm.epaInfo.ProcessByReviewers, function (reviewer) {
                totalScore = utilitiesFactory.parseFloat(reviewer.TotalScore);
                reviewer.TotalScore = totalScore + vm.epaInfo.DisciplineScore;
            });
            vm.epaInfo.AverageScoreByReviewers = vm.epaInfo.AverageScoreByReviewers + vm.epaInfo.DisciplineScore;

            // management
            vm.epaInfo.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.CurrentProcess.TotalScore);
            vm.finalScore = vm.epaInfo.CurrentProcess.TotalScore + vm.epaInfo.DisciplineScore;

            if (vm.finalScore > vm.epaInfo.MaxFinalScore) {
                vm.finalScore = vm.epaInfo.MaxFinalScore;
            }
            else if (vm.finalScore < 0) {
                vm.finalScore = 0;
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

        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;

            getEPAInfoByManagement();
        });
    };

    vm.initialize();
}]);