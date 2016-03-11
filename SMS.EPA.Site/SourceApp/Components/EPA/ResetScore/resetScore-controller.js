'use strict';
/*global epaModule*/

epaModule.controller('ResetScoreController',
['$routeParams', 'employeeEPAFactory', 'notificationFactory', 'errorFactory',
function ($routeParams, employeeEPAFactory, notificationFactory, errorFactory) {
    var vm = this;

    vm.epaId = parseInt($routeParams.id);

    vm.resetScore = function () {
        var criteria = {
            EPAId: vm.epaId
        };

        return employeeEPAFactory.resetScore(criteria)
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

    vm.resetScore();
}]);