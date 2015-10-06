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
        vm.viewEPADetailUrl = '/Reviewer/Detail/' + memberEPA.EPAId;
    };

    vm.viewEPAResult = function (memberEPA) {
        vm.viewEPAResultUrl = '/EPA/Detail/' + memberEPA.EPAId;
    };

    var getEPAByReviewer = function (groupId, employeeIds) {
        EmployeeEPAModel.getEPAByReviewer(groupId, employeeIds).then(function () {
            vm.memberEPAs = EmployeeEPAModel.memberEPAs;

            angular.forEach(vm.memberEPAs, function (memberEPA) {
                var totalScore,
                    adjustScore = memberEPA.CurrentProcess.AdjustScore ? parseFloat(memberEPA.CurrentProcess.AdjustScore) : 0;

                if (memberEPA.ProcessByEvaluators[0].IsEndProcess && !memberEPA.CurrentProcess.TotalScore) {
                    memberEPA.CurrentProcess.TotalScore = memberEPA.AverageScoreByEvaluators;
                }

                totalScore = memberEPA.CurrentProcess.TotalScore ? parseFloat(memberEPA.CurrentProcess.TotalScore) : null;
                memberEPA.CurrentProcess.TotalScore = totalScore;

                if (memberEPA.HasDisciplineScore) {
                    memberEPA.FinalScore = totalScore + memberEPA.DisciplineScore + adjustScore;
                }
                else {
                    memberEPA.FinalScore = totalScore + adjustScore;
                }

                if (memberEPA.FinalScore > memberEPA.MaxFinalScore) {
                    memberEPA.FinalScore = memberEPA.MaxFinalScore;
                }
                else if (memberEPA.FinalScore < 0) {
                    memberEPA.FinalScore = 0;
                }
            });
        });
    };

    var calculateAdjustScore = function (memberEPAs, adjustScore) {
        var score = adjustScore.adjust.Score ? parseFloat(adjustScore.adjust.Score) : 0,
            totalScore, totalScoreOriginal, totalScoreAdjust;

        angular.forEach(memberEPAs, function (memberEPA) {
            if (memberEPA.Selected) {
                totalScoreOriginal = memberEPA.CurrentProcess.TotalScoreOriginal ? parseFloat(memberEPA.CurrentProcess.TotalScoreOriginal) : 0;
                totalScore = memberEPA.CurrentProcess.TotalScore ? parseFloat(memberEPA.CurrentProcess.TotalScore) : 0;

                if (totalScore !== totalScoreOriginal) {
                    memberEPA.CurrentProcess.TotalScore = totalScoreOriginal;
                }

                totalScoreAdjust = totalScoreOriginal + score;

                if (totalScoreAdjust > memberEPA.CurrentProcess.MaxTotalScore) {
                    memberEPA.CurrentProcess.TotalScore = memberEPA.CurrentProcess.MaxTotalScore;
                }
                else if (totalScoreAdjust < 0) {
                    memberEPA.CurrentProcess.TotalScore = 0;
                }
                else {
                    memberEPA.CurrentProcess.TotalScore = totalScoreAdjust;
                }

                memberEPA.CurrentProcess.TotalScoreOriginal = memberEPA.CurrentProcess.TotalScore;

                if (memberEPA.HasDisciplineScore) {
                    memberEPA.FinalScore = memberEPA.CurrentProcess.TotalScore + memberEPA.DisciplineScore;
                }
                else {
                    memberEPA.FinalScore = memberEPA.CurrentProcess.TotalScore;
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

    var calculateFinalScore = function (memberEPA, processByReviewer) {
        var totalScoreOfReviewer = processByReviewer.TotalScore ? parseFloat(processByReviewer.TotalScore) : 0;

        if (memberEPA.HasDisciplineScore) {
            memberEPA.FinalScore = totalScoreOfReviewer + memberEPA.DisciplineScore;
        }
        else {
            memberEPA.FinalScore = totalScoreOfReviewer;
        }

        memberEPA.FinalScore = memberEPA.FinalScore !== 0 ? memberEPA.FinalScore : '';
    };

    var validateAdjustScore = function (memberEPAs) {
        var isValid = true;

        angular.forEach(memberEPAs, function (memberEPA) {
            var totalScore = memberEPA.CurrentProcess.TotalScore ? parseFloat(memberEPA.CurrentProcess.TotalScore) : 0;
            if (totalScore <= 0) {
                isValid = false;
            }
        });

        if (!isValid) {
            EmployeeEPAModel.openPopupNotificationComment(vm, vm.adjustScoreInvalidMessage);
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
            return EmployeeEPAModel.getNotificationInputScore(processByReviewer.MaxTotalScore, vm.scoreInputValidMessage);
        }

        return '';
    };

    vm.inputScore = function (memberEPA, processByReviewer) {
        if (!EmployeeEPAModel.validateScoreInput(processByReviewer.TotalScore, processByReviewer.MaxTotalScore, vm.scoreInputValidMessage)) {
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
        else if (validateAdjustScore(selectedEmployees)) {
            EmployeeEPAModel.adjustScoreByReviewer(vm, selectedEmployees);
        }
    };

    vm.updateComment = function (memberEPA, comment, isEdit) {
        EmployeeEPAModel.openPopupUpdateComment(vm, memberEPA, comment, isEdit, true).then(function (commentUpdate) {
            var index = vm.memberEPAs.map(function (e) { return e.EPAId; }).indexOf(memberEPA.EPAId),
                totalScoreOriginal, totalScore;

            if (index !== -1) {
                totalScoreOriginal = vm.memberEPAs[index].CurrentProcess.TotalScoreOriginal ? parseFloat(vm.memberEPAs[index].CurrentProcess.TotalScoreOriginal) : 0;
                totalScore = vm.memberEPAs[index].CurrentProcess.TotalScore ? parseFloat(vm.memberEPAs[index].CurrentProcess.TotalScore) : 0;

                if (totalScore !== totalScoreOriginal) {
                    vm.memberEPAs[index].CurrentProcess.TotalScore = totalScoreOriginal;
                    vm.memberEPAs[index].FinalScore = totalScoreOriginal;
                }

                vm.memberEPAs[index].CurrentProcess.Comment = commentUpdate;
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
        var totalScoreOfReviewer = epaInfo.CurrentProcess.TotalScore ? parseFloat(epaInfo.CurrentProcess.TotalScore) : 0;

        if (epaInfo.HasDisciplineScore) {
            vm.finalScore = totalScoreOfReviewer !== 0 ? (totalScoreOfReviewer + epaInfo.DisciplineScore) : epaInfo.DisciplineScore;
        }
        else {
            vm.finalScore = totalScoreOfReviewer;
        }

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

    vm.initialize = function () {
        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });

        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;
        });

        EmployeeEPAModel.getEPAInfoByReviewer(vm.epaId, vm.employeeId, vm.evaluatorId).then(function () {
            var data = EmployeeEPAModel.epaInfo;
            var totalScore = 0;

            if (data.AverageScoreByEvaluators && parseFloat(data.AverageScoreByEvaluators) > 0
                && !data.CurrentProcess.TotalScore && data.ProcessByEvaluators[0].IsEndProcess) {
                data.CurrentProcess.TotalScore = data.AverageScoreByEvaluators;
            }

            data.CurrentProcess.TotalScore = data.CurrentProcess.TotalScore ? parseFloat(data.CurrentProcess.TotalScore) : 0;
            totalScore = data.CurrentProcess.TotalScore;

            if (data.HasDisciplineScore) {
                vm.finalScore = totalScore + data.DisciplineScore;
            }
            else {
                vm.finalScore = totalScore;
            }

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