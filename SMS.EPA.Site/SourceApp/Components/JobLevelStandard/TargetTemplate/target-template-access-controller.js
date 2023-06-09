(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('JobLevelStandardTemplateController', templateCtrl);
    templateCtrl.$inject = ['JobLevelStandardTemplateModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService', 'baseUrl'];

    function templateCtrl(TemplateModel, CommonModel, utilitiesFactory, lockScreenService, baseUrl) {
        let vm = this;
        vm.templates = [];
        vm.permission = null;
        vm.policyEnum = utilitiesFactory.policy;
        vm.filter = {
            isActive: 'true'
        };
        vm.currentPage = 1;
        vm.pageSize = 20;

        utilitiesFactory.numberEntryList(vm);

        function getTemplate(isActive) {
            lockScreenService.start();
            TemplateModel.getTemplate(isActive).then(function () {
                vm.templates = TemplateModel.templates;
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
            TemplateModel.openPopupCreateTemplate(vm).then(function () {
                vm.modalInstance = null;
                getTemplate(vm.filter.isActive);
            });
        };

        vm.confirmActive = function (template) {
            TemplateModel.openPopupUpdateStatusTemplate(template);
        };

        vm.deleteTemplate = function (template) {
            TemplateModel.openPopupDeleteTemplate(vm, template);
        };

        vm.detail = function (template, event) {
            const url = baseUrl + 'JobLevelStandard/Template/Detail/' + template.TemplateId;
            utilitiesFactory.handleOpenLink(event, url);
        };

        function initialize() {
            TemplateModel.getPermissionOnTemplate().then(function () {
                vm.permission = TemplateModel.permission;
            });

            getTemplate(vm.filter.isActive);
        }

        initialize();
    }
})(angular);