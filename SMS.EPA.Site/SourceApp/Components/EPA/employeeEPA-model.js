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

    employeeEPAModel.getEPAOfCurrentMember = function () {
        return employeeEPAFactory.getEPAOfCurrentMember()
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

    employeeEPAModel.getMember = function (processId, groupId) {
        return employeeEPAFactory.getMember(processId, groupId)
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

        if (parseFloat(weight) > parseFloat(maxWeight)) {
            isValid = false;

            var message = utilitiesFactory.formatString(scoreInputValidMessage, maxWeight);
            notificationFactory.error('', message);
        }

        return isValid;
    };

    employeeEPAModel.notificationEvaluateEPAInValid = function () {
        notificationFactory.error("EvaluateEPAInvalid");
    }

    employeeEPAModel.updateEPAScore = function (vm) {
        return employeeEPAFactory.updateEPAScore(vm.epaInfo)
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

    employeeEPAModel.updateScoreByReviewerOrManagement = function (vm, criteria) {
        return employeeEPAFactory.updateScoreByReviewerOrManagement(criteria)
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

    employeeEPAModel.checkKPIOptional = function (kpiGroup) {
        var hasKPIOptional = false, totalKPIOptional = 0;

        angular.forEach(kpiGroup.KPIs, function (kpi) {
            if (!kpi.IsRequiredEvaluate) {
                hasKPIOptional = true;
                totalKPIOptional += 1;
            }
        });

        kpiGroup.TotalKPIOptional = totalKPIOptional;

        return hasKPIOptional;
    };

    employeeEPAModel.getClassProgress = function (statusEndProcess) {
        if (statusEndProcess === 0) {
            return 'progress-striped active';
        }
        else {
            return '';
        }
    };

    employeeEPAModel.openPopupViewComment = function (vm, comment) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/EPA/Templates/modalViewComment.html',
            controller: "ViewCommentController as modalViewCommentCtrl",
            backdrop: 'static',
            size: 'sm',
            resolve: {
                comment: function () {
                    return comment;
                }
            },
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    employeeEPAModel.getClassProgress = function (isEndProcess) {
        if (!isEndProcess) {
            return 'progress-striped active';
        }
        else {
            return '';
        }
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

    employeeEPAModel.openPopupAdjustScore = function (vm, selectedEmployees) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/EPA/Templates/modalAdjustScore.html',
            controller: "ModalAdjustScoreController as modalAdjustScoreCtrl",
            backdrop: 'static',
            resolve: {
                selectedEmployees: function () {
                    return selectedEmployees;
                }
            },
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    employeeEPAModel.adjustScoreByReviewer = function (memberEPAs) {
        return employeeEPAFactory.adjustScoreByReviewer(memberEPAs)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.adjustScoreByManagement = function (memberEPAs) {
        return employeeEPAFactory.adjustScoreByManagement(memberEPAs)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return employeeEPAModel;
}]);