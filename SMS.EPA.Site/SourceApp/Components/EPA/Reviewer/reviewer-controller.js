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
                var performanceScore = 0;

                if (memberEPA.AverageScoreByEvaluators && parseFloat(memberEPA.AverageScoreByEvaluators) > 0
                    && !memberEPA.ProcessByReviewers[0].TotalScore && memberEPA.ProcessByEvaluators[0].IsEndProcess) {
                    memberEPA.ProcessByReviewers[0].TotalScore = memberEPA.AverageScoreByEvaluators;
                }

                memberEPA.ProcessByReviewers[0].TotalScore = memberEPA.ProcessByReviewers[0].TotalScore ? parseFloat(memberEPA.ProcessByReviewers[0].TotalScore) : 0;
                performanceScore = memberEPA.ProcessByReviewers[0].TotalScore;

                if (memberEPA.HasDisciplineScore) {
                    memberEPA.FinalScore = performanceScore + memberEPA.DisciplineScore;
                }
                else {
                    memberEPA.FinalScore = performanceScore;
                }

                memberEPA.FinalScore = memberEPA.FinalScore !== 0 ? memberEPA.FinalScore : '';
            });
        });
    };

    var calculateAdjustScore = function (memberEPAs, adjustScore) {
        var score = adjustScore.adjust.Score ? parseFloat(adjustScore.adjust.Score) : 0,
            totalScore, totalScoreOriginal, totalScoreAdjust;

        angular.forEach(memberEPAs, function (memberEPA) {
            if (memberEPA.Selected) {
                totalScoreOriginal = memberEPA.ProcessByReviewers[0].TotalScoreOriginal ? parseFloat(memberEPA.ProcessByReviewers[0].TotalScoreOriginal) : 0;
                totalScore = memberEPA.ProcessByReviewers[0].TotalScore ? parseFloat(memberEPA.ProcessByReviewers[0].TotalScore) : 0;

                if (totalScore !== totalScoreOriginal) {
                    memberEPA.ProcessByReviewers[0].TotalScore = totalScoreOriginal;
                }

                totalScoreAdjust = totalScoreOriginal + score;

                if (totalScoreAdjust > memberEPA.ProcessByReviewers[0].MaxTotalScore) {
                    memberEPA.ProcessByReviewers[0].TotalScore = memberEPA.ProcessByReviewers[0].MaxTotalScore;
                }
                else if (totalScoreAdjust < 0) {
                    memberEPA.ProcessByReviewers[0].TotalScore = 0;
                }
                else {
                    memberEPA.ProcessByReviewers[0].TotalScore = totalScoreAdjust;
                }

                memberEPA.ProcessByReviewers[0].TotalScoreOriginal = memberEPA.ProcessByReviewers[0].TotalScore;

                if (memberEPA.HasDisciplineScore) {
                    memberEPA.FinalScore = memberEPA.ProcessByReviewers[0].TotalScore + memberEPA.DisciplineScore;
                }
                else {
                    memberEPA.FinalScore = memberEPA.ProcessByReviewers[0].TotalScore;
                }

                memberEPA.FinalScore = memberEPA.FinalScore !== 0 ? memberEPA.FinalScore : '';
                if (memberEPA.ProcessByReviewers[0].Comment.length > 0) {
                    memberEPA.ProcessByReviewers[0].Comment = memberEPA.ProcessByReviewers[0].Comment + '\n' + adjustScore.adjust.Comment;
                }
                else {
                    memberEPA.ProcessByReviewers[0].Comment = adjustScore.adjust.Comment;
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
            var totalScore = memberEPA.ProcessByReviewers[0].TotalScore ? parseFloat(memberEPA.ProcessByReviewers[0].TotalScore) : 0;
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
                totalScoreOriginal = vm.memberEPAs[index].ProcessByReviewers[0].TotalScoreOriginal ? parseFloat(vm.memberEPAs[index].ProcessByReviewers[0].TotalScoreOriginal) : 0;
                totalScore = vm.memberEPAs[index].ProcessByReviewers[0].TotalScore ? parseFloat(vm.memberEPAs[index].ProcessByReviewers[0].TotalScore) : 0;

                if (totalScore !== totalScoreOriginal) {
                    vm.memberEPAs[index].ProcessByReviewers[0].TotalScore = totalScoreOriginal;
                    vm.memberEPAs[index].FinalScore = totalScoreOriginal;
                }

                vm.memberEPAs[index].ProcessByReviewers[0].Comment = commentUpdate;
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

        vm.query(vm.filter);
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
        if (epaInfo.ProcessByReviewers[0].IsEndProcess) {
            vm.isReadonly = true;
        }
    };

    var calculateScore = function (epaInfo) {
        var totalScoreOfReviewer = epaInfo.ProcessByReviewers[0].TotalScore ? parseFloat(epaInfo.ProcessByReviewers[0].TotalScore) : 0;

        if (epaInfo.HasDisciplineScore) {
            vm.finalScore = totalScoreOfReviewer !== 0 ? (totalScoreOfReviewer + epaInfo.DisciplineScore) : epaInfo.DisciplineScore;
        }
        else {
            vm.finalScore = totalScoreOfReviewer;
        }

        vm.finalScore = vm.finalScore !== 0 ? vm.finalScore : '';
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
        var totalcore = epaInfo.ProcessByReviewers[0].TotalScore,
            comment = epaInfo.ProcessByReviewers[0].Comment;
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
                && !data.ProcessByReviewers[0].TotalScore && data.ProcessByEvaluators[0].IsEndProcess) {
                data.ProcessByReviewers[0].TotalScore = data.AverageScoreByEvaluators;
            }

            data.ProcessByReviewers[0].TotalScore = data.ProcessByReviewers[0].TotalScore ? parseFloat(data.ProcessByReviewers[0].TotalScore) : 0;
            totalScore = data.ProcessByReviewers[0].TotalScore;

            if (data.HasDisciplineScore) {
                vm.finalScore = totalScore + data.DisciplineScore;
            }
            else {
                vm.finalScore = totalScore;
            }

            vm.finalScore = vm.finalScore !== 0 ? vm.finalScore : '';
            data.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, data.MaxFinalScore, vm.standardGrades);
            vm.evaluatedDate = data.ProcessByReviewers[0].EvaluateDate ? data.ProcessByReviewers[0].EvaluateDate : null;
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