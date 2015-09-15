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

    employeeEPAModel.validateScoreInput = function (weight, maxWeight, scoreInputValidMessage) {
        var isValid = true;

        if (parseFloat(weight) > parseFloat(maxWeight)) {
            isValid = false;

            var message = utilitiesFactory.formatString(scoreInputValidMessage, maxWeight);
            notificationFactory.error('', message);
        }

        return isValid;
    };

    employeeEPAModel.updateEPAScore = function (dataSaved, epaInfo) {
        return employeeEPAFactory.updateEPAScore(epaInfo)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
                dataSaved = true;
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.updateScoreByReviewerOrManagement = function (dataSaved, criteria) {
        return employeeEPAFactory.updateScoreByReviewerOrManagement(criteria)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
                dataSaved = true;
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.validateEvaluationOfReviewerOrManagement = function (score) {
        if (!score) {
            notificationFactory.error("ScoreRequire");
            return false;
        }

        return true;
    };

    employeeEPAModel.updateEPAScoreByEvaluator = function (dataSaved, epaInfo) {
        return employeeEPAFactory.updateEPAScoreByEvaluator(epaInfo)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
                dataSaved = true;
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

    employeeEPAModel.getClassProgress = function (isEndProcess) {
        if (!isEndProcess) {
            return 'progress-striped active';
        }
        else {
            return '';
        }
    };

    return employeeEPAModel;
}]);