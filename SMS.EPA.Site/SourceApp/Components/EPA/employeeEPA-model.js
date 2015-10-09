'use strict';
/*global epaModule*/

epaModule.factory('EmployeeEPAModel',
['$modal', 'employeeEPAFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory',
function ($modal, employeeEPAFactory, utilitiesFactory, notificationFactory, errorFactory) {
    var employeeEPAModel = this;

    employeeEPAModel.myEPAs = [];
    employeeEPAModel.epaInfo = {};
    employeeEPAModel.members = [];
    employeeEPAModel.memberEPAs = [];

    employeeEPAModel.getEPAByEmployee = function () {
        return employeeEPAFactory.getEPAByEmployee()
        .success(function (myEPAs) {
            employeeEPAModel.myEPAs = myEPAs;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getEPAInfoByEmployee = function (epaId) {
        return employeeEPAFactory.getEPAInfoByEmployee(epaId)
        .success(function (epaInfo) {
            employeeEPAModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getMemberByEvaluator = function (groupId) {
        return employeeEPAFactory.getMemberByEvaluator(groupId)
        .success(function (members) {
            employeeEPAModel.members = members;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getMemberByReviewer = function (groupId) {
        return employeeEPAFactory.getMemberByReviewer(groupId)
        .success(function (members) {
            employeeEPAModel.members = members;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getMemberByManagement = function (groupId) {
        return employeeEPAFactory.getMemberByManagement(groupId)
        .success(function (members) {
            employeeEPAModel.members = members;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getEPAByEvaluator = function (groupId, employeeIds) {
        return employeeEPAFactory.getEPAByEvaluator(groupId, employeeIds)
        .success(function (memberEPAs) {
            employeeEPAModel.memberEPAs = memberEPAs;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getEPAInfoByEvaluator = function (epaId) {
        return employeeEPAFactory.getEPAInfoByEvaluator(epaId)
        .success(function (epaInfo) {
            employeeEPAModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getEPAByReviewer = function (groupId, employeeIds) {
        return employeeEPAFactory.getEPAByReviewer(groupId, employeeIds)
        .success(function (memberEPAs) {
            employeeEPAModel.memberEPAs = memberEPAs;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getEPAInfoByReviewer = function (epaId) {
        return employeeEPAFactory.getEPAInfoByReviewer(epaId)
        .success(function (epaInfo) {
            employeeEPAModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getEPAByManagement = function (groupId, employeeIds) {
        return employeeEPAFactory.getEPAByManagement(groupId, employeeIds)
        .success(function (memberEPAs) {
            employeeEPAModel.memberEPAs = memberEPAs;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getEPAInfoByManagement = function (epaId) {
        return employeeEPAFactory.getEPAInfoByManagement(epaId)
        .success(function (epaInfo) {
            employeeEPAModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getEPAInfoById = function (epaId) {
        return employeeEPAFactory.getEPAInfoById(epaId)
        .success(function (epaInfo) {
            employeeEPAModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getNotificationInputScore = function (maxWeight, message) {
        return utilitiesFactory.formatString(message, maxWeight);
    };

    employeeEPAModel.validateScoreInput = function (weight, maxWeight, scoreInputValidMessage) {
        var isValid = true;

        if (parseFloat(weight) > parseFloat(maxWeight) || parseFloat(weight) < 0) {
            isValid = false;

            var message = utilitiesFactory.formatString(scoreInputValidMessage, maxWeight);
            notificationFactory.error('', message);
        }

        return isValid;
    };

    employeeEPAModel.updateEPAScoreByEmployee = function (vm) {
        return employeeEPAFactory.updateEPAScoreByEmployee(vm.epaInfo)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
                vm.dataSaved = true;
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.validateEvaluationOfReviewerOrManagement = function (score) {
        if (!score || parseFloat(score) <= 0) {
            notificationFactory.error("ScoreRequire");
            return false;
        }

        return true;
    };

    employeeEPAModel.updateEPAScoreByEvaluator = function (vm) {
        return employeeEPAFactory.updateEPAScoreByEvaluator(vm.epaInfo)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
                vm.dataSaved = true;
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getClassProgress = function (statusEndProcess) {
        if (statusEndProcess === 0) {
            return 'in-process';
        }
        else if (statusEndProcess === 1) {
            return 'end-process';
        }
        else {
            return 'open';
        }
    };

    employeeEPAModel.openPopupViewComment = function (vm, comment) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/EPA/Templates/modalViewComment.html',
            controller: "ModalViewCommentController as modalViewCommentCtrl",
            backdrop: 'static',
            resolve: {
                comment: function () {
                    return comment;
                }
            },
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    employeeEPAModel.getSelectedEmployee = function (memberEPAs) {
        var selectedEmployees = [];

        angular.forEach(memberEPAs, function (memberEPA) {
            if (memberEPA.Selected) {
                selectedEmployees.push(memberEPA);
            }
        });

        return selectedEmployees;
    };

    employeeEPAModel.openPopupAdjustScore = function (vm, selectedEmployees, isReviewer) {
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

    employeeEPAModel.adjustScoreByReviewer = function (vm, memberEPAs) {
        return employeeEPAFactory.adjustScoreByReviewer(memberEPAs)
        .success(function (response) {
            if (response.IsSuccessful) {
                angular.forEach(memberEPAs, function (memberEPA) {
                    memberEPA.CurrentProcess.TotalScoreOriginal = memberEPA.CurrentProcess.TotalScore;
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

    employeeEPAModel.adjustScoreByManagement = function (vm, memberEPAs) {
        return employeeEPAFactory.adjustScoreByManagement(memberEPAs)
        .success(function (response) {
            if (response.IsSuccessful) {
                angular.forEach(memberEPAs, function (memberEPA) {
                    memberEPA.CurrentProcess.TotalScoreOriginal = memberEPA.CurrentProcess.TotalScore;
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

    employeeEPAModel.openPopupUpdateComment = function (vm, memberEPA, comment, isEdit, isReviewer) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/EPA/Templates/modalUpdateComment.html',
            controller: "ModalUpdateCommentController as modalUpdateCommentCtrl",
            backdrop: 'static',
            resolve: {
                memberEPA: function(){
                    return angular.copy(memberEPA);
                },
                comment: function () {
                    return comment;
                },
                isEdit: function () {
                    return isEdit;
                },
                isReviewer: function() {
                    return isReviewer;
                }
            },
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    employeeEPAModel.openPopupNotificationComment = function (vm, message) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/EPA/Templates/modalNotification.html',
            controller: "ModalNotificationController as modalNotificationCtrl",
            backdrop: 'static',
            resolve: {
                message: function () {
                    return message;
                }
            },
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    employeeEPAModel.convertScoreToGrade = function (finalScore, maxFinalScore, standardGrades) {
        var score = finalScore ? parseFloat(finalScore) : 0;
        var grade;

        angular.forEach(standardGrades, function (standardGrade) {
            if (score >= standardGrade.LeftScore && score < standardGrade.RightScore) {
                grade = standardGrade.Code;
            }
        });

        return grade;
    };

    return employeeEPAModel;
}]);