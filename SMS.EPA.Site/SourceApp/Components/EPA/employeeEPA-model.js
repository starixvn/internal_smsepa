'use strict';
/*global epaModule*/

epaModule.factory('EmployeeEPAModel',
['employeeEPAFactory', 'notificationFactory', 'errorFactory',
function (employeeEPAFactory, notificationFactory, errorFactory) {
    var employeeEPAModel = this;

    employeeEPAModel.listEPA = [];
    employeeEPAModel.epaInfo = {};

    employeeEPAModel.getEPAOfCurrentMember = function (status) {
        return employeeEPAFactory.getEPAOfCurrentMember(status)
        .success(function (listEPA) {
            employeeEPAModel.listEPA = listEPA;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getEPAInformation = function (epaId, periodId, employeeId) {
        return employeeEPAFactory.getEPAInformation(epaId, periodId, employeeId)
        .success(function (epaInfo) {
            employeeEPAModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.calculateScore = function (vm, epaInfo) {
        var finalScore = 0;

        angular.forEach(epaInfo.KPIGroups, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIs, function (kpi) {
                if (kpi.Score && parseFloat(kpi.Score) > 0) {
                    finalScore += parseFloat(kpi.Score);
                }
            });
        });

        epaInfo.FinalScore = finalScore.toFixed(2);
    };

    employeeEPAModel.validateEPA = function (epaInfo) {
        var isValid = true;

        if (!epaInfo.Comment || epaInfo.Comment.length === 0) {
            isValid = false;
            epaInfo.CommentValid = true;
        }

        angular.forEach(epaInfo.KPIGroups, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIs, function (kpi) {
                if (kpi.IsRequiredEvaluate && kpi.Score === 0) {
                    isValid = false;
                    kpi.EvaluateValid = true;
                }

                if (kpi.HasComment && kpi.IsRequiredComment && (!kpi.Comment || kpi.Comment.length === 0)) {
                    isValid = false;
                    kpi.CommentValid = true;
                }
            });
        });

        return isValid;
    };

    employeeEPAModel.updateEmployeeEPA = function (epaInfo) {
        return employeeEPAFactory.updateEmployeeEPA(epaInfo)
        .success(function (response) {
            if (response.IsSuccessful) {
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