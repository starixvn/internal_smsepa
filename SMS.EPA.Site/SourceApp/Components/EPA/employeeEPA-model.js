'use strict';
/*global epaModule*/

epaModule.factory('EmployeeEPAModel',
['employeeEPAFactory', 'notificationFactory', 'errorFactory',
function (employeeEPAFactory, notificationFactory, errorFactory) {
    var employeeEPAModel = this;

    employeeEPAModel.myEPAs = [];

    // #region My EPA

    employeeEPAModel.getEPAOfCurrentMember = function (status) {
        return employeeEPAFactory.getEPAOfCurrentMember(status)
        .success(function (myEPAs) {
            employeeEPAModel.myEPAs = myEPAs;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    // #endregion My EPA

    return employeeEPAModel;
}]);