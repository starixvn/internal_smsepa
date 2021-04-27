(function () {
    'use strict';

    angular.module('jobDescriptionModule').controller('CreateDescriptionGroupItemController', createDescriptionGroupItemController);
    createDescriptionGroupItemController.$inject = ['JobDescriptionModel', 'lockScreenService'];

    function createDescriptionGroupItemController($routeParams, JobDescriptionModel, lockScreenService) {
        var vm = this;

        vm.name = '';
    }
})();