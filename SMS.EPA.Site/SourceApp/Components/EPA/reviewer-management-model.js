'use strict';
/*global epaModule*/

epaModule.factory('ReviewerManagementModel',
['$modal', 'EvaluationModel', 'evaluationFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory',
function ($modal, EvaluationModel, evaluationFactory, utilitiesFactory, notificationFactory, errorFactory) {
    var reviewerMngModel = this;

    reviewerMngModel.getEPASelected = function (epaInfos) {
        var result = [];

        angular.forEach(epaInfos, function (epaInfo) {
            if (epaInfo.Selected) {
                result.push(epaInfo);
            }
        });

        return result;
    };

    reviewerMngModel.openPopupAdjustScore = function (vm, selectedEmployees, isReviewer) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/EPA/Templates/modalAdjustScore.html',
            controller: "ModalAdjustScoreController as modalAdjustScoreCtrl",
            backdrop: 'static',
            resolve: {
                selectedEmployees: function () {
                    return angular.copy(selectedEmployees);
                },
                isReviewer: function () {
                    return isReviewer;
                }
            },
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    reviewerMngModel.openPopupUpdateComment = function (vm, epaInfo, comment, isEdit, isReviewer) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/EPA/Templates/modalUpdateComment.html',
            controller: "ModalUpdateCommentController as modalUpdateCommentCtrl",
            backdrop: 'static',
            resolve: {
                epaInfo: function () {
                    return angular.copy(epaInfo);
                },
                comment: function () {
                    return comment;
                },
                isEdit: function () {
                    return isEdit;
                },
                isReviewer: function () {
                    return isReviewer;
                }
            },
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    reviewerMngModel.checkAdjustScoreChanged = function (epaInfos) {
        var hasChange = false;

        for (var i = 0; i < epaInfos.length; i++) {
            var adjustScoreOriginal = utilitiesFactory.parseFloat(epaInfos[i].Process.CurrentProcess.AdjustScoreOriginal),
                adjustScore = utilitiesFactory.parseFloat(epaInfos[i].Process.CurrentProcess.AdjustScore);

            if (adjustScoreOriginal !== adjustScore) {
                hasChange = true;

                break;
            }
        }

        return hasChange;
    };

    reviewerMngModel.openPopupRemindSaveChange = function (vm) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/EPA/Templates/modalRemindSaveChange.html',
            controller: "ModalRemindController as modalRemindCtrl",
            backdrop: 'static',
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    reviewerMngModel.validateEvaluation = function (score) {
        var isValid = true;

        if (score === null || parseFloat(score) < 0) {
            notificationFactory.error("ScoreRequire");
            isValid = false;
        }

        return isValid;
    };

    reviewerMngModel.calculateScore = function (epaInfo, isReviewer) {
        var scoreAdjusted = utilitiesFactory.parseFloat(epaInfo.Process.CurrentProcess.AdjustScore),
            averageScore = epaInfo.Process.AverageScoreByEvaluators,
            totalScore = 0;

        if (epaInfo.HasMonthlyEPA) {
            averageScore = epaInfo.MonthlyEPA.AverageReviewerScoreConverted;
        }

        if (!isReviewer && epaInfo.Process.Reviewers.length > 0) {
            averageScore = epaInfo.Process.AverageScoreByReviewers;
        }

        totalScore = utilitiesFactory.parseFloat(averageScore + scoreAdjusted);

        if (totalScore > epaInfo.Process.CurrentProcess.MaxTotalScore) {
            scoreAdjusted = epaInfo.Process.CurrentProcess.MaxTotalScore - averageScore;
        } else if (totalScore < 0) {
            scoreAdjusted = -averageScore;
        }

        epaInfo.Process.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(averageScore + scoreAdjusted);
        epaInfo.FinalScore = averageScore + scoreAdjusted + epaInfo.DisciplineScore;
        epaInfo.FinalScore = EvaluationModel.checkScore(epaInfo.FinalScore, 0, epaInfo.Process.CurrentProcess.MaxPerformanceScore);
    };

    reviewerMngModel.calculatePerformanceScore = function (vm, isReviewer) {
        var totalScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.CurrentProcess.TotalScore),
            averageScore = vm.epaInfo.Process.AverageScoreByEvaluators;

        if (vm.epaInfo.HasMonthlyEPA) {
            averageScore = vm.epaInfo.MonthlyEPA.AverageReviewerScoreConverted;
        }

        if (!isReviewer && vm.epaInfo.Process.Reviewers.length > 0) {
            averageScore = vm.epaInfo.Process.AverageScoreByReviewers;
        }

        vm.finalScore = totalScore + vm.epaInfo.DisciplineScore;
        vm.finalScore = EvaluationModel.checkScore(vm.finalScore, 0, vm.maxPerformanceScore);

        vm.epaInfo.Process.CurrentProcess.AdjustScore = utilitiesFactory.parseFloat(totalScore - averageScore);
        vm.epaInfo.Process.CurrentProcess.AdjustScoreOriginal = vm.epaInfo.Process.CurrentProcess.AdjustScore;
        vm.epaInfo.Grade = EvaluationModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
    };

    reviewerMngModel.calculateAdjustScore = function (epaInfos, isReviewer) {
        angular.forEach(epaInfos, function (epaInfo) {
            var scoreAdjusted = utilitiesFactory.parseFloat(epaInfo.Process.CurrentProcess.AdjustScore),
                averageScore = epaInfo.Process.AverageScoreByEvaluators,
                totalScore = 0;

            if (!isReviewer && epaInfo.Process.Reviewers.length > 0) {
                averageScore = epaInfo.Process.AverageScoreByReviewers;
            }

            totalScore = utilitiesFactory.parseFloat(scoreAdjusted + averageScore);

            if (totalScore > epaInfo.Process.CurrentProcess.MaxTotalScore) {
                scoreAdjusted = utilitiesFactory.parseFloat(epaInfo.Process.CurrentProcess.MaxTotalScore - averageScore);
            } else if (totalScore < 0) {
                scoreAdjusted = -averageScore;
            }

            epaInfo.Process.CurrentProcess.AdjustScore = scoreAdjusted;
        });
    };

    reviewerMngModel.calculateAdjustScoreMultiple = function (epaInfos, adjust, isReviewer) {
        var scoreAdjusted, averageScore, totalScore;

        angular.forEach(epaInfos, function (epaInfo) {
            if (epaInfo.Selected) {
                scoreAdjusted = utilitiesFactory.parseFloat(adjust.Score);
                averageScore = epaInfo.Process.AverageScoreByEvaluators;

                if (!isReviewer && epaInfo.Process.Reviewers.length > 0) {
                    averageScore = epaInfo.Process.AverageScoreByReviewers;
                }

                totalScore = utilitiesFactory.parseFloat(scoreAdjusted + averageScore);

                if (totalScore > epaInfo.Process.CurrentProcess.MaxTotalScore) {
                    scoreAdjusted = utilitiesFactory.parseFloat(epaInfo.Process.CurrentProcess.MaxTotalScore - averageScore);
                } else if (totalScore < 0) {
                    scoreAdjusted = -averageScore;
                }

                epaInfo.Process.CurrentProcess.AdjustScore = scoreAdjusted;
                epaInfo.Process.CurrentProcess.AdjustScoreOriginal = epaInfo.Process.CurrentProcess.AdjustScore;
                epaInfo.Process.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(averageScore + scoreAdjusted);
                epaInfo.Process.CurrentProcess.TotalScoreOriginal = epaInfo.Process.CurrentProcess.TotalScore;
                epaInfo.FinalScore = averageScore + scoreAdjusted + epaInfo.DisciplineScore;
                epaInfo.FinalScore = EvaluationModel.checkScore(epaInfo.FinalScore, 0, epaInfo.Process.CurrentProcess.MaxPerformanceScore);

                if (epaInfo.Process.CurrentProcess.Comment && epaInfo.Process.CurrentProcess.Comment.length > 0) {
                    epaInfo.Process.CurrentProcess.Comment = epaInfo.Process.CurrentProcess.Comment + '\n' + adjust.Comment;
                } else {
                    epaInfo.Process.CurrentProcess.Comment = adjust.Comment;
                }
            }
        });
    };

    reviewerMngModel.updateSelectedEmployees = function (epaInfos, selectedEmployees) {
        angular.forEach(epaInfos, function (epaInfo) {
            if (!epaInfo.Process.CurrentProcess.IsEndProcess) {
                angular.forEach(selectedEmployees, function (selectedEmployee) {
                    if (epaInfo.EPAId === selectedEmployee.EPAId) {
                        epaInfo.Selected = true;
                    }
                });
            }
        });
    };

    return reviewerMngModel;
}]);