(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('CreateJLSTemplateController', controller);
    controller.$inject = ['$uibModalInstance', 'JobLevelStandardTemplateModel'];

    function controller($uibModalInstance, JobLevelStandardTemplateModel) {
        var vm = this;

        vm.permission = 0;

        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $uibModalInstance.dismiss("cancel");
        }

        function save(data) {
            JobLevelStandardTemplateModel.createTemplate(data, $uibModalInstance);
        }
    }
})(angular);