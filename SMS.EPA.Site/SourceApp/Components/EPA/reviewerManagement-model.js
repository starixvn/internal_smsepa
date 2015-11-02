'use strict';
/*global epaModule*/

epaModule.factory('ReviewerManagementModel',
['$modal', 'EmployeeEPAModel', 'employeeEPAFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory',
function ($modal, EmployeeEPAModel, employeeEPAFactory, utilitiesFactory, notificationFactory, errorFactory) {
    var reviewerMngModel = this;

    reviewerMngModel.epaInfo = {};
    reviewerMngModel.members = [];
    reviewerMngModel.memberEPAs = [];

    // common
    reviewerMngModel.getSelectedEmployee = function (memberEPAs) {
        var result = [];

        angular.forEach(memberEPAs, function (memberEPA) {
            if (memberEPA.Selected) {
                result.push(memberEPA);
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

    reviewerMngModel.openPopupUpdateComment = function (vm, memberEPA, comment, isEdit, isReviewer) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/EPA/Templates/modalUpdateComment.html',
            controller: "ModalUpdateCommentController as modalUpdateCommentCtrl",
            backdrop: 'static',
            resolve: {
                memberEPA: function () {
                    return angular.copy(memberEPA);
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

    reviewerMngModel.checkAdjustScoreChanged = function (memberEPAs) {
        var hasChange = false;

        for (var i = 0; i < memberEPAs.length; i++) {
            var adjustScoreOriginal = utilitiesFactory.parseFloat(memberEPAs[i].CurrentProcess.AdjustScoreOriginal),
                adjustScore = utilitiesFactory.parseFloat(memberEPAs[i].CurrentProcess.AdjustScore);

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
        if (!score || parseFloat(score) <= 0) {
            notificationFactory.error("ScoreRequire");
            return false;
        }

        return true;
    };

    reviewerMngModel.calculateScore = function (memberEPA, isReviewer) {
        var scoreAdjusted = utilitiesFactory.parseFloat(memberEPA.CurrentProcess.AdjustScore),
            averageScore = memberEPA.AverageScoreByEvaluators,
            totalScore;

        if (!isReviewer && memberEPA.ProcessByReviewers.length > 0) {
            averageScore = memberEPA.AverageScoreByReviewers;
        }

        if (memberEPA.HasDisciplineScore && memberEPA.DisciplineScore < 0) {
            memberEPA.DisciplineScore = 0;
        }

        totalScore = averageScore + scoreAdjusted;

        if (totalScore > memberEPA.CurrentProcess.MaxTotalScore) {
            scoreAdjusted = memberEPA.CurrentProcess.MaxTotalScore - averageScore;
        } else if (totalScore < 0) {
            scoreAdjusted = -averageScore;
        }

        memberEPA.CurrentProcess.TotalScore = averageScore + scoreAdjusted;
        memberEPA.FinalScore = averageScore + scoreAdjusted + memberEPA.DisciplineScore;
        memberEPA.FinalScore = EmployeeEPAModel.checkScore(memberEPA.FinalScore, 0, memberEPA.MaxFinalScore);
    };

    reviewerMngModel.calculatePerformanceScore = function (vm, isReviewer) {
        var totalScore = utilitiesFactory.parseFloat(vm.epaInfo.CurrentProcess.TotalScore),
            averageScore = vm.epaInfo.AverageScoreByEvaluators;

        if (!isReviewer && vm.epaInfo.ProcessByReviewers.length > 0) {
            averageScore = vm.epaInfo.AverageScoreByReviewers;
        }

        if (vm.epaInfo.HasDisciplineScore && vm.epaInfo.DisciplineScore < 0) {
            vm.epaInfo.DisciplineScore = 0;
        }

        averageScore -= vm.epaInfo.DisciplineScore;
        vm.finalScore = totalScore + vm.epaInfo.DisciplineScore;
        vm.finalScore = EmployeeEPAModel.checkScore(vm.finalScore, 0, vm.epaInfo.MaxFinalScore);

        vm.epaInfo.CurrentProcess.AdjustScore = totalScore - averageScore;
        vm.epaInfo.CurrentProcess.AdjustScoreOriginal = vm.epaInfo.CurrentProcess.AdjustScore;
        vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
    };

    reviewerMngModel.calculateAdjustScore = function (memberEPAs, isReviewer) {
        angular.forEach(memberEPAs, function (memberEPA) {
            var scoreAdjusted = utilitiesFactory.parseFloat(memberEPA.CurrentProcess.AdjustScore),
                averageScore = memberEPA.AverageScoreByEvaluators,
                totalScore = 0;

            if (!isReviewer && memberEPA.ProcessByReviewers.length > 0) {
                averageScore = memberEPA.AverageScoreByReviewers;
            }

            if (memberEPA.HasDisciplineScore && memberEPA.DisciplineScore < 0) {
                memberEPA.DisciplineScore = 0;
            }

            totalScore = scoreAdjusted + averageScore;

            if (totalScore > memberEPA.CurrentProcess.MaxTotalScore) {
                scoreAdjusted = memberEPA.CurrentProcess.MaxTotalScore - averageScore;
            }
            else if (totalScore < 0) {
                scoreAdjusted = -averageScore;
            }

            memberEPA.CurrentProcess.AdjustScore = scoreAdjusted;
        });
    };

    reviewerMngModel.calculateAdjustScoreMultiple = function (memberEPAs, adjust, isReviewer) {
        var scoreAdjusted, averageScore, totalScore;

        angular.forEach(memberEPAs, function (memberEPA) {
            if (memberEPA.Selected) {
                scoreAdjusted = utilitiesFactory.parseFloat(adjust.Score);
                averageScore = memberEPA.AverageScoreByEvaluators;

                if (!isReviewer && memberEPA.ProcessByReviewers.length > 0) {
                    averageScore = memberEPA.AverageScoreByReviewers;
                }

                if (memberEPA.HasDisciplineScore && memberEPA.DisciplineScore < 0) {
                    memberEPA.DisciplineScore = 0;
                }

                totalScore = scoreAdjusted + averageScore;

                if (totalScore > memberEPA.CurrentProcess.MaxTotalScore) {
                    scoreAdjusted = memberEPA.CurrentProcess.MaxTotalScore - averageScore;
                } else if (totalScore < 0) {
                    scoreAdjusted = -averageScore;
                }

                memberEPA.CurrentProcess.AdjustScore = scoreAdjusted;
                memberEPA.CurrentProcess.AdjustScoreOriginal = scoreAdjusted;
                memberEPA.CurrentProcess.TotalScore = averageScore + scoreAdjusted;
                memberEPA.CurrentProcess.TotalScoreOriginal = averageScore + scoreAdjusted;
                memberEPA.FinalScore = averageScore + scoreAdjusted + memberEPA.DisciplineScore;
                memberEPA.FinalScore = EmployeeEPAModel.checkScore(memberEPA.FinalScore, 0, memberEPA.MaxFinalScore);

                if (memberEPA.CurrentProcess.Comment && memberEPA.CurrentProcess.Comment.length > 0) {
                    memberEPA.CurrentProcess.Comment = memberEPA.CurrentProcess.Comment + '\n' + adjust.Comment;
                } else {
                    memberEPA.CurrentProcess.Comment = adjust.Comment;
                }
            }
        });
    };

    // reviewer
    reviewerMngModel.getMemberByReviewer = function (groupId) {
        return employeeEPAFactory.getMemberByReviewer(groupId)
        .success(function (members) {
            reviewerMngModel.members = members;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getEPAByReviewer = function (groupId, employeeIds) {
        return employeeEPAFactory.getEPAByReviewer(groupId, employeeIds)
        .success(function (memberEPAs) {
            reviewerMngModel.memberEPAs = memberEPAs;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getEPAInfoByReviewer = function (epaId) {
        return employeeEPAFactory.getEPAInfoByReviewer(epaId)
        .success(function (epaInfo) {
            reviewerMngModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.adjustScoreByReviewer = function (vm, memberEPAs) {
        return employeeEPAFactory.adjustScoreByReviewer(memberEPAs)
        .success(function (response) {
            if (response.IsSuccessful) {
                angular.forEach(memberEPAs, function (memberEPA) {
                    memberEPA.CurrentProcess.TotalScoreOriginal = memberEPA.CurrentProcess.TotalScore;
                    memberEPA.CurrentProcess.AdjustScoreOriginal = memberEPA.CurrentProcess.AdjustScore;
                });

                vm.dataSaved = true;

                notificationFactory.success("UpdatedSuccessfully");
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    // management
    reviewerMngModel.getMemberByManagement = function (groupId) {
        return employeeEPAFactory.getMemberByManagement(groupId)
        .success(function (members) {
            reviewerMngModel.members = members;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getEPAByManagement = function (groupId, employeeIds) {
        return employeeEPAFactory.getEPAByManagement(groupId, employeeIds)
        .success(function (memberEPAs) {
            reviewerMngModel.memberEPAs = memberEPAs;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getEPAInfoByManagement = function (epaId) {
        return employeeEPAFactory.getEPAInfoByManagement(epaId)
        .success(function (epaInfo) {
            reviewerMngModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.adjustScoreByManagement = function (vm, memberEPAs) {
        return employeeEPAFactory.adjustScoreByManagement(memberEPAs)
        .success(function (response) {
            if (response.IsSuccessful) {
                angular.forEach(memberEPAs, function (memberEPA) {
                    memberEPA.CurrentProcess.TotalScoreOriginal = memberEPA.CurrentProcess.TotalScore;
                    memberEPA.CurrentProcess.AdjustScoreOriginal = memberEPA.CurrentProcess.AdjustScore;
                });

                vm.dataSaved = true;

                notificationFactory.success("UpdatedSuccessfully");
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return reviewerMngModel;
}]);