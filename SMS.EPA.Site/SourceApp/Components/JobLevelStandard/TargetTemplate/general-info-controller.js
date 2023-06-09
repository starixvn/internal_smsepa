(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('targetTemplateModule').controller('TargetTemplateGeneralInfoController', generalInfoController);
    generalInfoController.$inject = ['JobLevelStandardTemplateModel'];

    function generalInfoController(TemplateModel) {
        let vm = this;

        vm.save = function (template) {
            if (TemplateModel.validateTemplate(template)) {
                TemplateModel.updateTemplateBasicInfo(template)
            }
        };
    }
})(angular);