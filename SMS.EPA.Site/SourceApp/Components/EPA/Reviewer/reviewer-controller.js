'use strict';
/*global epaModule*/

epaModule.controller('ReviewerController',
['$translate', 'EmployeeEPAModel', 'CommonModel', 'utilitiesFactory', 'notificationFactory',
function ($translate, EmployeeEPAModel, CommonModel, utilitiesFactory, notificationFactory) {
    var vm = this;

    vm.memberEPAs = EmployeeEPAModel.memberEPAs;
    vm.members = EmployeeEPAModel.members;
    vm.selectedAll = false;

    vm.filter = {
        groupId: '',
        employeeIds: ''
    };

    vm.getMember = function (groupId) {
        EmployeeEPAModel.getMember(4, groupId).then(function () {
            vm.members = EmployeeEPAModel.members;
        });
    };

    vm.viewEPADetail = function (memberEPA) {
        vm.viewEPADetailUrl = '/Reviewer/Detail?epaId=' + memberEPA.EPAId;
    };

    vm.viewEPAResult = function (memberEPA) {
        vm.viewEPAResultUrl = '/EPA/Detail/' + memberEPA.EPAId;
    };

    var getEPAByReviewer = function (groupId, employeeIds) {
        EmployeeEPAModel.getEPAByReviewer(groupId, employeeIds).then(function () {
            vm.memberEPAs = EmployeeEPAModel.memberEPAs;

            angular.forEach(vm.memberEPAs, function (memberEPA) {
                var performanceScore = memberEPA.ProcessByReviewers[0].TotalScore ? parseFloat(memberEPA.ProcessByReviewers[0].TotalScore) : 0;

                if (memberEPA.HasAttendanceScore) {
                    memberEPA.FinalScore = performanceScore + memberEPA.AttendanceScore;
                }
                else {
                    memberEPA.FinalScore = memberEPA.AttendanceScore;
                }
            });
        });
    };

    var calculateAdjustScore = function (memberEPAs, adjustScore) {
        var score = parseFloat(adjustScore.adjust.Score);

        angular.forEach(memberEPAs, function (memberEPA) {
            if (memberEPA.Selected) {
                var totalScore = memberEPA.ProcessByReviewers[0].TotalScore ? parseFloat(memberEPA.ProcessByReviewers[0].TotalScore) : 0,
                    totalScoreAdjust = totalScore + score;

                if (totalScoreAdjust > memberEPA.ProcessByReviewers[0].MaxScore) {
                    memberEPA.ProcessByReviewers[0].TotalScore = memberEPA.ProcessByReviewers[0].MaxScore;
                }
                else {
                    memberEPA.ProcessByReviewers[0].TotalScore = totalScoreAdjust;
                }

                memberEPA.ProcessByReviewers[0].Comment = adjustScore.adjust.Comment;
            }
        });
    };

    var calculateFinalScore = function (memberEPA, processByReviewer) {
        var totalScoreOfReviewer = processByReviewer.TotalScore ? parseFloat(processByReviewer.TotalScore) : 0;

        if (memberEPA.HasAttendanceScore) {
            memberEPA.FinalScore = totalScoreOfReviewer + memberEPA.AttendanceScore;
        }
        else {
            memberEPA.FinalScore = totalScoreOfReviewer;
        }
    };

    var validateAdjustScore = function (memberEPAs) {
        var isValid = true;

        angular.forEach(memberEPAs, function (memberEPA) {
            var totalScore = memberEPA.ProcessByReviewers[0].TotalScore ? parseFloat(memberEPA.ProcessByReviewers[0].TotalScore) : 0;
            if (totalScore === 0) {
                isValid = false;
            }
        });

        if (!isValid) {
            notificationFactory.error("AdjustScoreInvalid");
        }

        return isValid;
    };

    vm.checkAll = function (selectedAll) {
        angular.forEach(vm.memberEPAs, function (memberEPA) {
            memberEPA.Selected = selectedAll;
        });
    };

    vm.getNotificationInputScore = function (processByReviewer) {
        if (processByReviewer) {
            return EmployeeEPAModel.getNotificationInputScore(processByReviewer.MaxScore, vm.scoreInputValidMessage);
        }

        return '';
    };

    vm.inputScore = function (memberEPA, processByReviewer) {
        if (!EmployeeEPAModel.validateScoreInput(processByReviewer.TotalScore, processByReviewer.MaxScore, vm.scoreInputValidMessage)) {
            processByReviewer.TotalScore = null;
        }

        calculateFinalScore(memberEPA, processByReviewer);
    };

    vm.adjustScore = function () {
        var selectedEmployees = EmployeeEPAModel.getSelectedEmployee(vm.memberEPAs);

        if (selectedEmployees.length === 0) {
            notificationFactory.error("SelectEmployeeBeforeAdjustScoreWarning");
        }
        else {
            EmployeeEPAModel.openPopupAdjustScore(vm, selectedEmployees).then(function (adjust) {
                calculateAdjustScore(vm.memberEPAs, adjust);
            })['finally'](function () {
                modalInstance = null;
            });
        }
    };

    vm.saveAdjustScore = function () {
        var selectedEmployees = EmployeeEPAModel.getSelectedEmployee(vm.memberEPAs);

        if (selectedEmployees.length === 0) {
            notificationFactory.error("SelectEmployeeBeforeSaveWarning");
        }
        else if (validateAdjustScore(selectedEmployees)) {
            EmployeeEPAModel.adjustScoreByReviewer(selectedEmployees);
        }
    };

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByReviewer(filter.groupId, filter.employeeIds);
    };

    vm.initialize = function () {
        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });

        CommonModel.getAllGroup().then(function () {
            vm.groupOptions = CommonModel.groups;
        });

        vm.getMember(vm.filter.groupId);

        vm.query(vm.filter);
    };

    vm.initialize();
}]);

epaModule.controller('MemberEPAInfoOfReviewerController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'utilitiesFactory',
function ($window, $translate, $routeParams, EmployeeEPAModel, utilitiesFactory) {
    var vm = this;

    vm.epaId = parseInt($routeParams.epaId);

    vm.dataSaved = false;
    vm.isReadonly = false;
    vm.evaluatedDate = null;
    vm.finalScore = null;

    var checkStatus = function (epaInfo) {
        if (epaInfo.ProcessByReviewers[0].IsEndProcess) {
            vm.isReadonly = true;
        }
    };

    var calculateScore = function (epaInfo) {
        var totalScoreOfReviewer = epaInfo.ProcessByReviewers[0].TotalScore ? parseFloat(epaInfo.ProcessByReviewers[0].TotalScore) : 0;

        if (epaInfo.HasAttendanceScore) {
            vm.finalScore = totalScoreOfReviewer !== 0 ? (totalScoreOfReviewer + epaInfo.AttendanceScore) : epaInfo.AttendanceScore;
        }
        else {
            vm.finalScore = totalScoreOfReviewer;
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
            return EmployeeEPAModel.getNotificationInputScore(processByReviewer.MaxScore, vm.scoreInputValidMessage);
        }

        return '';
    };

    vm.inputScore = function (processByReviewer) {
        if (!EmployeeEPAModel.validateScoreInput(processByReviewer.TotalScore, processByReviewer.MaxScore, vm.scoreInputValidMessage)) {
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
        var totalcore = epaInfo.ProcessByReviewers[0].TotalScore,
            comment = epaInfo.ProcessByReviewers[0].Comment;
        if (EmployeeEPAModel.validateEvaluationOfReviewerOrManagement(totalcore)) {
            var criteria = {
                epaId: epaInfo.EPAId,
                processId: 4,
                employeeId: epaInfo.Employee.EmployeeId,
                score: totalcore,
                comment: comment
            };

            EmployeeEPAModel.updateScoreByReviewerOrManagement(vm, criteria);
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };

    vm.initialize = function () {
        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });

        EmployeeEPAModel.getEPAInfoByReviewer(vm.epaId, vm.employeeId, vm.evaluatorId).then(function () {
            var data = EmployeeEPAModel.epaInfo;
            var totalScoreOfReviewer = null;

            if (data.AverageScoreByEvaluators
                && parseFloat(data.AverageScoreByEvaluators) > 0
                && !data.ProcessByReviewers[0].TotalScore) {
                data.ProcessByReviewers[0].TotalScore = data.AverageScoreByEvaluators;
            }

            totalScoreOfReviewer = data.ProcessByReviewers[0].TotalScore ? parseFloat(data.ProcessByReviewers[0].TotalScore) : 0;

            if (data.HasAttendanceScore) {
                vm.finalScore = totalScoreOfReviewer !== 0 ? (totalScoreOfReviewer + data.AttendanceScore) : data.AttendanceScore;
            }
            else {
                vm.finalScore = totalScoreOfReviewer;
            }

            vm.evaluatedDate = data.ProcessByReviewers[0].EvaluateDate ? data.ProcessByReviewers[0].EvaluateDate : null;
            vm.epaInfo = data;
            checkStatus(vm.epaInfo);

            angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
                if (!EmployeeEPAModel.checkKPIOptional(kpiGroup) && kpiGroup.IsSpreadKPI) {
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