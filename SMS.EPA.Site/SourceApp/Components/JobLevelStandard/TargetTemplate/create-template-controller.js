(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('CreateJLSTemplateController', controller);
    controller.$inject = ['$uibModalInstance', 'JobLevelStandardTemplateModel'];

    function controller($uibModalInstance, TemplateModel) {
        let vm = this;
        vm.template = {
            Name: ''
        };

        vm.cancel = function cancel() {
            $uibModalInstance.dismiss("cancel");
        };

        vm.save = function save(data) {
            if (TemplateModel.validateTemplate(data)) {
                TemplateModel.createTemplate(data, $uibModalInstance);
            }
        };
    }
})(angular);