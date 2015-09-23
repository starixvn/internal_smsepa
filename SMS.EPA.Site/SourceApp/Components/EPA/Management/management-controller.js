'use strict';
/*global epaModule*/

epaModule.controller('ManagementController',
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
        EmployeeEPAModel.getMemberByManagement(groupId).then(function () {
            vm.members = EmployeeEPAModel.members;
        });
    };

    vm.viewEPADetail = function (memberEPA) {
        vm.viewEPADetailUrl = '/Management/Detail/' + memberEPA.EPAId;
    };

    vm.viewEPAResult = function (memberEPA) {
        vm.viewEPAResultUrl = '/EPA/Detail/' + memberEPA.EPAId;
    };

    var getEPAByManagement = function (groupId, employeeIds) {
        EmployeeEPAModel.getEPAByManagement(groupId, employeeIds).then(function () {
            vm.memberEPAs = EmployeeEPAModel.memberEPAs;

            angular.forEach(vm.memberEPAs, function (memberEPA) {
                var performanceScore = null;

                if (memberEPA.AverageScoreByReviewers && parseFloat(memberEPA.AverageScoreByReviewers) > 0
                    && !memberEPA.ProcessByManagements[0].TotalScore) {
                    memberEPA.ProcessByManagements[0].TotalScore = memberEPA.AverageScoreByReviewers;
                }

                performanceScore = memberEPA.ProcessByManagements[0].TotalScore ? parseFloat(memberEPA.ProcessByManagements[0].TotalScore) : 0;

                if (memberEPA.HasAttendanceScore) {
                    memberEPA.FinalScore = performanceScore + memberEPA.AttendanceScore;
                }
                else {
                    memberEPA.FinalScore = performanceScore;
                }
            });
        });
    };

    var calculateAdjustScore = function (memberEPAs, adjustScore) {
        var score = adjustScore.adjust.Score ? parseFloat(adjustScore.adjust.Score) : 0;

        angular.forEach(memberEPAs, function (memberEPA) {
            if (memberEPA.Selected) {
                var totalScore = memberEPA.ProcessByManagements[0].TotalScore ? parseFloat(memberEPA.ProcessByManagements[0].TotalScore) : 0,
                    totalScoreAdjust = totalScore + score;

                if (totalScoreAdjust > memberEPA.ProcessByManagements[0].MaxScore) {
                    memberEPA.ProcessByManagements[0].TotalScore = memberEPA.ProcessByManagements[0].MaxScore;
                }
                else {
                    memberEPA.ProcessByManagements[0].TotalScore = totalScoreAdjust;
                }

                if (memberEPA.HasAttendanceScore) {
                    memberEPA.FinalScore = memberEPA.ProcessByManagements[0].TotalScore + memberEPA.AttendanceScore;
                }
                else {
                    memberEPA.FinalScore = memberEPA.ProcessByManagements[0].TotalScore;
                }

                memberEPA.ProcessByManagements[0].Comment = adjustScore.adjust.Comment;
            }
        });
    };

    var calculateFinalScore = function (memberEPA, processManagement) {
        var totalScore = processManagement.TotalScore ? parseFloat(processManagement.TotalScore) : 0;

        if (memberEPA.HasAttendanceScore) {
            memberEPA.FinalScore = totalScore + memberEPA.AttendanceScore;
        }
        else {
            memberEPA.FinalScore = totalScore;
        }
    };

    var validateAdjustScore = function (memberEPAs) {
        var isValid = true;

        angular.forEach(memberEPAs, function (memberEPA) {
            var totalScore = memberEPA.ProcessByManagements[0].TotalScore ? parseFloat(memberEPA.ProcessByManagements[0].TotalScore) : 0;
            if (totalScore <= 0) {
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

    vm.inputScore = function (memberEPA, processManagement) {
        if (!EmployeeEPAModel.validateScoreInput(processManagement.TotalScore, processManagement.MaxScore, vm.scoreInputValidMessage)) {
            processManagement.TotalScore = null;
        }

        calculateFinalScore(memberEPA, processManagement);
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
        else if (validateAdjustScore(selectedEmployees)){
            EmployeeEPAModel.adjustScoreByManagement(selectedEmployees);
        }
    };

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByManagement(filter.groupId, filter.employeeIds);
    };

    vm.updateComment = function (epaId, comment, isEdit) {
        EmployeeEPAModel.openPopupUpdateComment(vm, comment, isEdit).then(function (commentUpdate) {
            var index = vm.memberEPAs.map(function (e) { return e.EPAId; }).indexOf(epaId);

            if (index !== -1) {
                vm.memberEPAs[index].ProcessByManagements[0].Comment = commentUpdate;
            }
        })['finally'](function () {
            vm.modalInstance = null;
        });
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

epaModule.controller('MemberEPAInfoOfManagentController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'utilitiesFactory',
function ($window, $translate, $routeParams, EmployeeEPAModel, utilitiesFactory) {
    var vm = this;

    vm.epaId = parseInt($routeParams.id);

    vm.dataSaved = false;
    vm.isReadonly = false;
    vm.evaluatedDate = null;
    vm.finalScore = null;

    var checkStatus = function (epaInfo) {
        if (epaInfo.ProcessByManagements[0].IsEndProcess) {
            vm.isReadonly = true;
        }
    };

    var calculateScore = function (epaInfo) {
        var totalScore = epaInfo.ProcessByManagements[0].TotalScore ? parseFloat(epaInfo.ProcessByManagements[0].TotalScore) : 0;

        if (epaInfo.HasAttendanceScore) {
            vm.finalScore = totalScore !== 0 ? (totalScore + epaInfo.AttendanceScore) : epaInfo.AttendanceScore;
        }
        else {
            vm.finalScore = totalScore;
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
            return EmployeeEPAModel.getNotificationInputScore(processByManagement.MaxScore, vm.scoreInputValidMessage);
        }

        return '';
    };

    vm.inputScore = function (processByManagement) {
        if (!EmployeeEPAModel.validateScoreInput(processByManagement.TotalScore, processByManagement.MaxScore, vm.scoreInputValidMessage)) {
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
        var totalcore = epaInfo.ProcessByManagements[0].TotalScore,
            comment = epaInfo.ProcessByManagements[0].Comment;
        if (EmployeeEPAModel.validateEvaluationOfReviewerOrManagement(totalcore)) {
            var criteria = {
                epaId: epaInfo.EPAId,
                processId: 5,
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

        EmployeeEPAModel.getEPAInfoByManagement(vm.epaId).then(function () {
            var data = EmployeeEPAModel.epaInfo;
            var totalScoreOfManagement = null;

            if (data.AverageScoreByReviewers && parseFloat(data.AverageScoreByReviewers) > 0
                && !data.ProcessByManagements[0].TotalScore) {
                data.ProcessByManagements[0].TotalScore = data.AverageScoreByReviewers;
            }

            totalScoreOfManagement = data.ProcessByManagements[0].TotalScore ? parseFloat(data.ProcessByManagements[0].TotalScore) : 0;

            if (data.HasAttendanceScore) {
                vm.finalScore = totalScoreOfManagement !== 0 ? (totalScoreOfManagement + data.AttendanceScore) : data.AttendanceScore;
            }
            else {
                vm.finalScore = totalScoreOfManagement;
            }

            vm.evaluatedDate = data.ProcessByManagements[0].EvaluateDate ? data.ProcessByManagements[0].EvaluateDate : null;
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