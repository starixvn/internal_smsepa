'use strict';
/*global epaModule*/

epaModule.factory('ReviewerManagementModel',
['$modal', 'EmployeeEPAModel', 'employeeEPAFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory',
function ($modal, EmployeeEPAModel, employeeEPAFactory, utilitiesFactory, notificationFactory, errorFactory) {
    var reviewerMngModel = this;

    reviewerMngModel.epaInfo = {};
    reviewerMngModel.groups = [];
    reviewerMngModel.members = [];
    reviewerMngModel.epaInfos = [];
    reviewerMngModel.permission = 0;

    // common
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
        if (!score || parseFloat(score) < 0) {
            notificationFactory.error("ScoreRequire");
            return false;
        }

        return true;
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
        epaInfo.FinalScore = EmployeeEPAModel.checkScore(epaInfo.FinalScore, 0, epaInfo.Process.CurrentProcess.MaxPerformanceScore);
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
        vm.finalScore = EmployeeEPAModel.checkScore(vm.finalScore, 0, vm.maxPerformanceScore);

        vm.epaInfo.Process.CurrentProcess.AdjustScore = utilitiesFactory.parseFloat(totalScore - averageScore);
        vm.epaInfo.Process.CurrentProcess.AdjustScoreOriginal = vm.epaInfo.Process.CurrentProcess.AdjustScore;
        vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
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
                epaInfo.FinalScore = EmployeeEPAModel.checkScore(epaInfo.FinalScore, 0, epaInfo.Process.CurrentProcess.MaxPerformanceScore);

                if (epaInfo.Process.CurrentProcess.Comment && epaInfo.Process.CurrentProcess.Comment.length > 0) {
                    epaInfo.Process.CurrentProcess.Comment = epaInfo.Process.CurrentProcess.Comment + '\n' + adjust.Comment;
                } else {
                    epaInfo.Process.CurrentProcess.Comment = adjust.Comment;
                }
            }
        });
    };

    // reviewer
    reviewerMngModel.getUserPermissionOnReviewer = function () {
        return employeeEPAFactory.getUserPermissionOnReviewer().success(function (permission) {
            reviewerMngModel.permission = parseInt(permission);
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getGroupByReviewer = function () {
        return employeeEPAFactory.getGroupByReviewer().success(function (groups) {
            reviewerMngModel.groups = groups;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getMemberByReviewer = function (groupId, isLocal, isMonthly) {
        return employeeEPAFactory.getMemberByReviewer(groupId, isLocal, isMonthly).success(function (members) {
            reviewerMngModel.members = members;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getEPAByReviewer = function (groupId, isLocal, isMonthly, employeeIds, statusProcess) {
        return employeeEPAFactory.getEPAByReviewer(groupId, isLocal, isMonthly, employeeIds, statusProcess).success(function (epaInfos) {
            reviewerMngModel.epaInfos = epaInfos;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getEPAInfoByReviewer = function (epaId) {
        return employeeEPAFactory.getEPAInfoByReviewer(epaId).success(function (epaInfo) {
            reviewerMngModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.adjustScoreByReviewer = function (vm, epaInfos) {
        var models = [];

        angular.forEach(epaInfos, function (epaInfo) {
            var model = {
                EPAId: epaInfo.EPAId,
                EmployeeId: epaInfo.Employee.EmployeeId,
                ProcessId: epaInfo.Process.CurrentProcess.ProcessId,
                EvaluatorId: epaInfo.Process.CurrentProcess.EvaluatorInfo.EmployeeId,
                AdjustScore: epaInfo.Process.CurrentProcess.AdjustScore,
                TotalScore: epaInfo.Process.CurrentProcess.TotalScore,
                Comment: epaInfo.Process.CurrentProcess.Comment
            };

            models.push(model);
        });

        return employeeEPAFactory.adjustScoreByReviewer(models).success(function (response) {
            if (response.IsSuccessful) {
                vm.dataSaved = true;
                notificationFactory.success("UpdatedSuccessfully");
            } else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    // management
    reviewerMngModel.getUserPermissionOnManagement = function () {
        return employeeEPAFactory.getUserPermissionOnManagement().success(function (permission) {
            reviewerMngModel.permission = parseInt(permission);
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getGroupByManagement = function () {
        return employeeEPAFactory.getGroupByManagement().success(function (groups) {
            reviewerMngModel.groups = groups;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getMemberByManagement = function (groupId, isLocal) {
        return employeeEPAFactory.getMemberByManagement(groupId, isLocal).success(function (members) {
            reviewerMngModel.members = members;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getEPAByManagement = function (groupId, isLocal, employeeIds, statusProcess) {
        return employeeEPAFactory.getEPAByManagement(groupId, isLocal, employeeIds, statusProcess).success(function (epaInfos) {
            reviewerMngModel.epaInfos = epaInfos;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.getEPAInfoByManagement = function (epaId) {
        return employeeEPAFactory.getEPAInfoByManagement(epaId).success(function (epaInfo) {
            reviewerMngModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reviewerMngModel.adjustScoreByManagement = function (vm, epaInfos) {
        var models = [];

        angular.forEach(epaInfos, function (epaInfo) {
            var model = {
                EPAId: epaInfo.EPAId,
                EmployeeId: epaInfo.Employee.EmployeeId,
                ProcessId: epaInfo.Process.CurrentProcess.ProcessId,
                EvaluatorId: epaInfo.Process.CurrentProcess.EvaluatorInfo.EmployeeId,
                AdjustScore: epaInfo.Process.CurrentProcess.AdjustScore,
                TotalScore: epaInfo.Process.CurrentProcess.TotalScore,
                Comment: epaInfo.Process.CurrentProcess.Comment
            };

            models.push(model);
        });

        return employeeEPAFactory.adjustScoreByManagement(models).success(function (response) {
            if (response.IsSuccessful) {
                vm.dataSaved = true;
                notificationFactory.success("UpdatedSuccessfully");
            } else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
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