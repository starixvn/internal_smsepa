'use strict';
/*global epaModule*/

epaModule.controller('ReviewerController',
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
        ReviewerManagementModel.getMemberByReviewer(groupId).then(function () {
            vm.members = ReviewerManagementModel.members;
        });
    };

    vm.viewEPADetail = function (memberEPA) {
        if (ReviewerManagementModel.checkAdjustScoreChanged(vm.memberEPAs)) {
            ReviewerManagementModel.openPopupRemindSaveChange(vm).then(function () {
                vm.viewEPADetailUrl = baseUrl + 'Reviewer/Detail/' + memberEPA.EPAId;
                $location.url(vm.viewEPADetailUrl);
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }
        else {
            vm.viewEPADetailUrl = baseUrl + 'Reviewer/Detail/' + memberEPA.EPAId;
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
        if (memberEPA) {
            var minSubtractScore = -memberEPA.ProcessByEvaluators[0].MaxTotalScore,
                maxAdditionScore = memberEPA.CurrentProcess.MaxTotalScore;

            return EmployeeEPAModel.getNotificationInputScore(minSubtractScore, maxAdditionScore, vm.scoreInputValidMessage);
        }

        return '';
    };

    vm.inputScore = function (memberEPA) {
        var minSubtractScore = -memberEPA.ProcessByEvaluators[0].MaxTotalScore,
            maxAdditionScore = memberEPA.CurrentProcess.MaxTotalScore;

        if (!EmployeeEPAModel.validateScoreInput(memberEPA.CurrentProcess.AdjustScore, minSubtractScore, maxAdditionScore, vm.scoreInputValidMessage)) {
            memberEPA.CurrentProcess.AdjustScore = null;
        }

        ReviewerManagementModel.calculateScore(memberEPA, true);
    };

    vm.adjustScore = function () {
        var selectedEmployees = ReviewerManagementModel.getSelectedEmployee(vm.memberEPAs);

        if (selectedEmployees.length === 0) {
            notificationFactory.error("SelectEmployeeBeforeAdjustScoreWarning");
        }
        else {
            ReviewerManagementModel.openPopupAdjustScore(vm, selectedEmployees, true).then(function (adjust) {
                ReviewerManagementModel.calculateAdjustScoreMultiple(selectedEmployees, adjust, true);
            })['finally'](function () {
                vm.modalInstance = null;
            });
        }
    };

    vm.saveAdjustScore = function () {
        var selectedEmployees = ReviewerManagementModel.getSelectedEmployee(vm.memberEPAs);

        if (selectedEmployees.length === 0) {
            notificationFactory.error("SelectEmployeeBeforeSaveWarning");
        }
        else {
            ReviewerManagementModel.calculateAdjustScore(selectedEmployees, true);
            ReviewerManagementModel.adjustScoreByReviewer(vm, selectedEmployees);
        }
    };

    vm.updateComment = function (memberEPA, comment, isEdit) {
        ReviewerManagementModel.openPopupUpdateComment(vm, memberEPA, comment, isEdit, true).then(function (commentUpdate) {
            var index = vm.memberEPAs.map(function (e) { return e.EPAId; }).indexOf(memberEPA.EPAId);

            if (index !== -1) {
                vm.memberEPAs[index].CurrentProcess.TotalScore = vm.memberEPAs[index].CurrentProcess.TotalScoreOriginal;
                vm.memberEPAs[index].CurrentProcess.AdjustScore = vm.memberEPAs[index].CurrentProcess.AdjustScoreOriginal;
                vm.memberEPAs[index].CurrentProcess.Comment = commentUpdate;

                ReviewerManagementModel.calculateScore(vm.memberEPAs[index], true);
            }
        })['finally'](function () {
            vm.modalInstance = null;
        });
    };

    var getEPAByReviewer = function (groupId, employeeIds) {
        vm.isEndProcessAll = false;

        ReviewerManagementModel.getEPAByReviewer(groupId, employeeIds).then(function () {
            vm.memberEPAs = ReviewerManagementModel.memberEPAs;

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

                totalScore = utilitiesFactory.parseFloat(memberEPA.CurrentProcess.TotalScore);

                memberEPA.DisciplineScore = disciplineScore;
                memberEPA.CurrentProcess.TotalScore = totalScore;
                memberEPA.ProcessBySelf.TotalScore = utilitiesFactory.parseFloat(memberEPA.ProcessBySelf.TotalScore);
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
            vm.averageScore = utilitiesFactory.parseFloat(averageScore / vm.memberEPAs.length);
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

            getEPAByReviewer(vm.filter.groupId, vm.filter.employeeIds);
        });
    };

    vm.initialize();
}]);

epaModule.controller('MemberEPAInfoOfReviewerController',
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

    vm.getNotificationInputScore = function (processByReviewer) {
        if (processByReviewer) {
            return EmployeeEPAModel.getNotificationInputScore(0, processByReviewer.MaxTotalScore, vm.scoreInputValidMessage);
        }

        return '';
    };

    vm.inputScore = function (processByReviewer) {
        if (!EmployeeEPAModel.validateScoreInput(processByReviewer.TotalScore, 0, processByReviewer.MaxTotalScore, vm.scoreInputValidMessage)) {
            processByReviewer.TotalScore = null;
        }

        ReviewerManagementModel.calculatePerformanceScore(vm, true);
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
        if (ReviewerManagementModel.validateEvaluationOfReviewerOrManagement(totalcore)) {
            var model = new Array(epaInfo);

            ReviewerManagementModel.adjustScoreByReviewer(vm, model);
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };

    var getEPAInfoByReviewer = function () {
        ReviewerManagementModel.getEPAInfoByReviewer(vm.epaId).then(function () {
            var totalScore, disciplineScore;
            vm.epaInfo = ReviewerManagementModel.epaInfo;
            disciplineScore = vm.epaInfo.MaxDisciplineScore - vm.epaInfo.DisciplineScore;

            if (disciplineScore < 0) {
                disciplineScore = 0;
            }

            vm.epaInfo.DisciplineScore = disciplineScore;

            if (vm.epaInfo.AverageScoreByEvaluators > 0 && !vm.epaInfo.CurrentProcess.TotalScore && vm.epaInfo.ProcessByEvaluators[0].IsEndProcess) {
                vm.epaInfo.CurrentProcess.TotalScore = vm.epaInfo.AverageScoreByEvaluators;
            }

            vm.epaInfo.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.CurrentProcess.TotalScore);
            totalScore = vm.epaInfo.CurrentProcess.TotalScore;

            if (vm.epaInfo.HasDisciplineScore) {
                vm.finalScore = totalScore + vm.epaInfo.DisciplineScore;

                totalScore = utilitiesFactory.parseFloat(vm.epaInfo.ProcessBySelf.TotalScore);
                vm.epaInfo.ProcessBySelf.TotalScore = totalScore + vm.epaInfo.DisciplineScore;

                angular.forEach(vm.epaInfo.ProcessByEvaluators, function (evaluator) {
                    totalScore = utilitiesFactory.parseFloat(evaluator.TotalScore);
                    evaluator.TotalScore = totalScore + vm.epaInfo.DisciplineScore;
                });

                vm.epaInfo.AverageScoreByEvaluators = vm.epaInfo.AverageScoreByEvaluators + vm.epaInfo.DisciplineScore;
            }
            else {
                vm.finalScore = totalScore;
            }

            vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
            vm.evaluatedDate = vm.epaInfo.CurrentProcess.EvaluateDate ? vm.epaInfo.CurrentProcess.EvaluateDate : null;
            vm.epaInfo = vm.epaInfo;
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

            getEPAInfoByReviewer();
        });
    };

    vm.initialize();
}]);