(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('JobLevelStandardTemplateController', templateCtrl);
    templateCtrl.$inject = ['$scope', '$location', 'JobLevelStandardTemplateModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService', 'baseUrl'];

    function templateCtrl($scope, $location, JLSTemplateModel, CommonModel, utilitiesFactory, lockScreenService, baseUrl) {
        var vm = this;
        vm.templates = [];
        vm.permission = null;
        vm.policyEnum = utilitiesFactory.policy;
        vm.filter = {
            isActive: 'true',
        };
        vm.currentPage = 1;
        vm.pageSize = 20;

        utilitiesFactory.numberEntryList(vm);
        utilitiesFactory.maxLengthDescription(vm);

        function getTemplate(isActive) {
            lockScreenService.start();
            JLSTemplateModel.getTemplate(isActive).then(function () {
                vm.templates = JLSTemplateModel.templates;
                utilitiesFactory.searchOnPage(vm, vm.templates);
                lockScreenService.stop();
            });
        }

        vm.query = function (filter) {
            vm.searchKey = '';
            vm.currentPage = 1;

            getTemplate(filter.isActive);
        };

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.create = function () {
            JLSTemplateModel.openPopupCreateTemplate(vm).then(function () {
                vm.modalInstance = null;
                getTemplate(vm.filter.isActive,);
            });
        };

        vm.confirmActive = function (template) {
            JLSTemplateModel.openPopupUpdateStatusTemplate(template);
        };

        vm.deleteTemplate = function (template) {
            JLSTemplateModel.openPopupDeleteTemplate(vm, template);
        };

        vm.detail = function (template, event) {
            var url = baseUrl + 'TargetTemplate/Detail/' + template.TemplateId;
            utilitiesFactory.handleOpenLink(event, url);
        };

        function initialize() {
            JLSTemplateModel.getPermissionOnTemplate().then(function () {
                vm.permission = JLSTemplateModel.permission;
            });

            getTemplate(vm.filter.isActive);
        }

        initialize();
    }
})(angular);