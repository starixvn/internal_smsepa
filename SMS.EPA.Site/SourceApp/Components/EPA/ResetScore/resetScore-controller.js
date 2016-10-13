'use strict';
/*global epaModule*/

epaModule.controller('ResetScoreController',
['$routeParams', 'evaluationFactory', 'notificationFactory', 'errorFactory',
function ($routeParams, evaluationFactory, notificationFactory, errorFactory) {
    var vm = this;

    vm.epaId = parseInt($routeParams.id);

    vm.resetScore = function () {
        return evaluationFactory.resetScore(vm.epaId).success(function (response) {
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