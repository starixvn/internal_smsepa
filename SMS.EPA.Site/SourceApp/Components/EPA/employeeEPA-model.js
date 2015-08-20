'use strict';
/*global epaModule*/

epaModule.factory('EmployeeEPAModel',
['$modal', 'employeeEPAFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory',
function ($modal, employeeEPAFactory, utilitiesFactory, notificationFactory, errorFactory) {
    var employeeEPAModel = this;

    employeeEPAModel.epaStatus = [{ Id: 0, Name: 'Open' }, { Id: 1, Name: 'Closed' }];

    employeeEPAModel.myEPAs = [];
    employeeEPAModel.epaInfo = {};
    employeeEPAModel.members = [];
    employeeEPAModel.memberEPAs = [];

    employeeEPAModel.getEPAOfCurrentMember = function (targetTypeId, status) {
        return employeeEPAFactory.getEPAOfCurrentMember(targetTypeId, status)
        .success(function (myEPAs) {
            employeeEPAModel.myEPAs = myEPAs;
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

    employeeEPAModel.getMember = function (processId, groupId) {
        return employeeEPAFactory.getMember(processId, groupId)
        .success(function (members) {
            employeeEPAModel.members = members;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getEPAByEvaluator = function (fromDate, toDate, targetTypeId, groupId, employeeIds, status) {
        return employeeEPAFactory.getEPAByEvaluator(fromDate, toDate, targetTypeId, groupId, employeeIds, status)
        .success(function (memberEPAs) {
            employeeEPAModel.memberEPAs = memberEPAs;
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

    employeeEPAModel.validateWeightInput = function (weight, maxWeight, weightInputValidMessage) {
        var isValid = true;

        if (weight > maxWeight) {
            isValid = false;

            var message = utilitiesFactory.formatString(weightInputValidMessage, maxWeight);
            notificationFactory.error('', message);
        }

        return isValid;
    }

    employeeEPAModel.validateEPA = function (epaInfo) {
        var isValid = true;

        if (!epaInfo.Comment || epaInfo.Comment.length === 0) {
            isValid = false;
            epaInfo.CommentValid = true;
        }

        angular.forEach(epaInfo.KPIGroups, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIs, function (kpi) {
                if (kpi.IsRequiredEvaluate && (!kpi.Score || kpi.Score === 0)) {
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

    employeeEPAModel.updateEmployeeEPA = function (dataSaved, epaInfo) {
        return employeeEPAFactory.updateEmployeeEPA(epaInfo)
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

    employeeEPAModel.openPopupUpdateStatusEPA = function (epa, epaId, periodId, processId, employeeId, status) {
        var criteria = {
            epaId: epaId,
            periodId: periodId,
            processId: processId,
            employeeId: employeeId,
            status: status.Id
        };

        var modalInstance = $modal.open({
            templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return '';
                },
                action: function () {
                    return "change status";
                }
            },
            windowClass: "modal-confirm-center"
        });

        modalInstance.result.then(function () {
            employeeEPAFactory.updateStatusEPA(criteria)
            .success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                    epa.Status = status;
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            })
            .error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        })['finally'](function () {
            modalInstance = '';
        });
    };

    return employeeEPAModel;
}]);