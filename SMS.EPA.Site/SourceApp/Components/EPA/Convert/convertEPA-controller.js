'use strict';
/*global epaModule*/

epaModule.controller('ConvertEPAController',
['evaluationFactory', 'notificationFactory', 'errorFactory',
function (evaluationFactory, notificationFactory, errorFactory) {
    var vm = this;

    vm.convertEPA = function () {
        return evaluationFactory.convertEPA()
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