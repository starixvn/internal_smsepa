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

    employeeEPAModel.getEPAInfoByEmployee = function (epaId, periodId, employeeId) {
        return employeeEPAFactory.getEPAInfoByEmployee(epaId, periodId, employeeId)
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

    employeeEPAModel.getEPAInfoByEvaluator = function (epaId, periodId, employeeId) {
        return employeeEPAFactory.getEPAInfoByEvaluator(epaId, periodId, employeeId)
        .success(function (epaInfo) {
            employeeEPAModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.viewEPAInfoOfMember = function (epaId, periodId, employeeId) {
        return $modal.open({
            templateUrl: 'SourceApp/Components/EPA/Evaluator/Templates/viewEPAInfoOfMember.html',
            controller: "ViewEPAInfoOfMemberController as memberEPAInfoCtrl",
            size: 'lg',
            resolve: {
                epaId: function () {
                    return epaId;
                },
                periodId: function () {
                    return periodId;
                },
                employeeId: function () {
                    return employeeId;
                }
            },
            windowClass: "modal-confirm-center"
        });
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