(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('targetTemplateModule').controller('TargetTemplateGeneralInfoController', generalInfoController);
    generalInfoController.$inject = ['$scope', 'JobLevelStandardTemplateModel'];

    function generalInfoController($scope, JLSTemplateModel) {
        var vm = this;

        vm.save = function (template) {
            if (JLSTemplateModel.validateTemplate(template)) {
                JLSTemplateModel.updateTemplateBasicInfo(template)
            }
        };
    }
})(angular);