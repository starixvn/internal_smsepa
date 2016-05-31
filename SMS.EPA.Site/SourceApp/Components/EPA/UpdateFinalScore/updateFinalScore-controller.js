'use strict';

(function (angular) {
    function UpdateFinalScoreController(CommonModel) {
        var vm = this;

        vm.departments = [];
        vm.periods = [];

        function initialize() {
            CommonModel.getDepartment().then(function () {
                vm.departments = CommonModel.departments;
            });
        }

        vm.initialize = initialize;

        vm.initialize();
    }

    UpdateFinalScoreController.$inject = ['CommonModel'];

    angular
        .module('epaModule')
        .controller('UpdateFinalScoreController', UpdateFinalScoreController);
})(angular);