(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').controller('ResetScoreController', resetScoreController);
    resetScoreController.$inject = ['$routeParams', 'evaluationFactory', 'notificationFactory', 'errorFactory'];

    function resetScoreController($routeParams, evaluationFactory, notificationFactory, errorFactory) {
        var vm = this;

        vm.epaId = parseInt($routeParams.id);

        vm.resetScore = function () {
            return evaluationFactory.resetScore(vm.epaId).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.resetScore();
    }
})(angular);