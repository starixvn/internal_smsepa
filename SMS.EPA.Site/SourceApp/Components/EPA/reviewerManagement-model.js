'use strict';
/*global epaModule*/

epaModule.factory('ReviewerManagementModel',
['$modal', 'EmployeeEPAModel', 'employeeEPAFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory',
function ($modal, EmployeeEPAModel, employeeEPAFactory, utilitiesFactory, notificationFactory, errorFactory) {
    var reviewerMngModel = this;

    reviewerMngModel.epaInfo = {};
    reviewerMngModel.groups = [];
    reviewerMngModel.members = [];
    reviewerMngModel.memberEPAs = [];
    reviewerMngModel.permission = 0;

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
            totalScore = 0;

        if (!isReviewer && memberEPA.ProcessByReviewers.length > 0) {
            averageScore = memberEPA.AverageScoreByReviewers;
        }

        if (memberEPA.HasDisciplineScore && memberEPA.DisciplineScore < 0) {
            memberEPA.DisciplineScore = 0;
        }

        totalScore = utilitiesFactory.parseFloat(averageScore + scoreAdjusted);

        if (totalScore > memberEPA.CurrentProcess.MaxTotalScore) {
            scoreAdjusted = memberEPA.CurrentProcess.MaxTotalScore - averageScore;
        } else if (totalScore < 0) {
            scoreAdjusted = -averageScore;
        }

        memberEPA.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(averageScore + scoreAdjusted);
        memberEPA.FinalScore = averageScore + scoreAdjusted + memberEPA.DisciplineScore;
        memberEPA.FinalScore = EmployeeEPAModel.checkScore(memberEPA.FinalScore, 0, memberEPA.MaxPerformanceScore);
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

        vm.finalScore = totalScore + vm.epaInfo.DisciplineScore;
        vm.finalScore = EmployeeEPAModel.checkScore(vm.finalScore, 0, vm.maxPerformanceScore);

        vm.epaInfo.CurrentProcess.AdjustScore = utilitiesFactory.parseFloat(totalScore - averageScore);
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

            totalScore = utilitiesFactory.parseFloat(scoreAdjusted + averageScore);

            if (totalScore > memberEPA.CurrentProcess.MaxTotalScore) {
                scoreAdjusted = utilitiesFactory.parseFloat(memberEPA.CurrentProcess.MaxTotalScore - averageScore);
            } else if (totalScore < 0) {
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

                totalScore = utilitiesFactory.parseFloat(scoreAdjusted + averageScore);

                if (totalScore > memberEPA.CurrentProcess.MaxTotalScore) {
                    scoreAdjusted = utilitiesFactory.parseFloat(memberEPA.CurrentProcess.MaxTotalScore - averageScore);
                } else if (totalScore < 0) {
                    scoreAdjusted = -averageScore;
                }

                memberEPA.CurrentProcess.AdjustScore = scoreAdjusted;
                memberEPA.CurrentProcess.AdjustScoreOriginal = memberEPA.CurrentProcess.AdjustScore;
                memberEPA.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(averageScore + scoreAdjusted);
                memberEPA.CurrentProcess.TotalScoreOriginal = memberEPA.CurrentProcess.TotalScore;
                memberEPA.FinalScore = averageScore + scoreAdjusted + memberEPA.DisciplineScore;
                memberEPA.FinalScore = EmployeeEPAModel.checkScore(memberEPA.FinalScore, 0, memberEPA.MaxPerformanceScore);

                if (memberEPA.CurrentProcess.Comment && memberEPA.CurrentProcess.Comment.length > 0) {
                    memberEPA.CurrentProcess.Comment = memberEPA.CurrentProcess.Comment + '\n' + adjust.Comment;
                } else {
                    memberEPA.CurrentProcess.Comment = adjust.Comment;
                }
            }
        });
    };

    // reviewer
    reviewerMngModel.getUserPermissionOnReviewer = function () {
        return employeeEPAFactory.getUserPermissionOnReviewer()
        .success(function (permission) {
            reviewerMngModel.permission = parseInt(permission);
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getGroupByReviewer = function () {
        return employeeEPAFactory.getGroupByReviewer()
        .success(function (groups) {
            reviewerMngModel.groups = groups;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getMemberByReviewer = function (groupId, isLocal) {
        return employeeEPAFactory.getMemberByReviewer(groupId, isLocal)
        .success(function (members) {
            reviewerMngModel.members = members;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getEPAByReviewer = function (groupId, isLocal, isMonthly, employeeIds) {
        return employeeEPAFactory.getEPAByReviewer(groupId, isLocal, isMonthly, employeeIds)
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
    reviewerMngModel.getUserPermissionOnManagement = function () {
        return employeeEPAFactory.getUserPermissionOnManagement()
        .success(function (permission) {
            reviewerMngModel.permission = parseInt(permission);
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getGroupByManagement = function () {
        return employeeEPAFactory.getGroupByManagement()
        .success(function (groups) {
            reviewerMngModel.groups = groups;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getMemberByManagement = function (groupId, isLocal) {
        return employeeEPAFactory.getMemberByManagement(groupId, isLocal)
        .success(function (members) {
            reviewerMngModel.members = members;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getEPAByManagement = function (groupId, isLocal, isMonthly, employeeIds) {
        return employeeEPAFactory.getEPAByManagement(groupId, isLocal, isMonthly, employeeIds)
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

    reviewerMngModel.updateSelectedEmployees = function (memberEPAs, selectedEmployees) {
        angular.forEach(memberEPAs, function (memberEPA) {
            if (!memberEPA.CurrentProcess.IsEndProcess) {
                angular.forEach(selectedEmployees, function (selectedEmployee) {
                    if (memberEPA.EPAId === selectedEmployee.EPAId) {
                        memberEPA.Selected = true;
                    }
                });
            }
        });
    };

    return reviewerMngModel;
}]);