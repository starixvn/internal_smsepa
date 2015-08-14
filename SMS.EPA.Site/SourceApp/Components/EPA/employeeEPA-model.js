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

    employeeEPAModel.getEPAInformation = function (employeeId) {
        return employeeEPAFactory.getEPAInformation(employeeId)
        .success(function (epaInfo) {
            employeeEPAModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return employeeEPAModel;
}]);