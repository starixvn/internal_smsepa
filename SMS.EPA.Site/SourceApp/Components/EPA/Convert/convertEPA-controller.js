'use strict';
/*global epaModule*/

epaModule.controller('ConvertEPAController',
['employeeEPAFactory', 'notificationFactory', 'errorFactory',
function (employeeEPAFactory, notificationFactory, errorFactory) {
    var vm = this;

    vm.convertEPA = function () {
        return employeeEPAFactory.convertEPA()
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

    vm.convertEPA();
}]);