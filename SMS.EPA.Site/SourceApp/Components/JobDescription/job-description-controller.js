(function (angular) {
    'use strict';

    angular.module('jobDescriptionModule').controller('JobDescriptionController', jobDescriptionController);
    jobDescriptionController.$inject = ['JobDescriptionModel', 'utilitiesFactory'];

    function jobDescriptionController(JobDescriptionModel, utilitiesFactory) {
        let vm = this;

        vm.dataTab = 1;
        vm.permission = 0;
        vm.policy = utilitiesFactory.policy;

        function initialize() {
            JobDescriptionModel.getUserPermission().then(function () {
                vm.permission = JobDescriptionModel.permission;
            });
        }

        initialize();
    }
})(angular);