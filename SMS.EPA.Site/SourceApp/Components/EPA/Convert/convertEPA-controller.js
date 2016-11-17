(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('epaModule').controller('ConvertEPAController', convertEPAController);
    convertEPAController.$inject = ['evaluationFactory', 'notificationFactory', 'errorFactory'];

    function convertEPAController(evaluationFactory, notificationFactory, errorFactory) {
        var vm = this;

        vm.convertEPA = function () {
            return evaluationFactory.convertEPA().success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.convertEPA();
    }
})(angular);