(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('targetTemplateModule').controller('TargetTemplateController', targetTemplateController);
    targetTemplateController.$inject = ['$location', 'TargetTemplateModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService', 'baseUrl'];

    function targetTemplateController($location, TargetTemplateModel, CommonModel, utilitiesFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.targetTypes = [];
        vm.targetTemplates = [];
        vm.permission = null;
        vm.policyEnum = utilitiesFactory.policy;
        vm.filter = {
            targetTypeId: '',
            isActive: 'true'
        };
        vm.currentPage = 1;
        vm.pageSize = 20;

        utilitiesFactory.numberEntryList(vm);
        utilitiesFactory.maxLengthDescription(vm);

        vm.readMore = function (item) {
            utilitiesFactory.readMore(item);
        };

        vm.readLess = function (item) {
            utilitiesFactory.readLess(item);
        };

        function getTargetTemplate(targetTypeId, isActive) {
            lockScreenService.start();
            TargetTemplateModel.getTargetTemplate(targetTypeId, isActive).then(function () {
                vm.targetTemplates = TargetTemplateModel.targetTemplates;
                utilitiesFactory.searchOnPage(vm, vm.targetTemplates);
                lockScreenService.stop();
            });
        }

        vm.query = function (filter) {
            vm.searchKey = '';
            vm.currentPage = 1;
            getTargetTemplate(filter.targetTypeId, filter.isActive);
        };

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.create = function () {
            TargetTemplateModel.openPopupCreateTargetTemplate(vm).then(function () {
                vm.modalInstance = null;
                getTargetTemplate(vm.filter.targetTypeId, vm.filter.isActive);
            });
        };

        vm.confirmActive = function (targetTemplate) {
            TargetTemplateModel.openPopupUpdateStatusTargetTemplate(targetTemplate);
        };

        vm.update = function (targetTemplate, viewKPIs) {
            if (!viewKPIs) {
                $location.path(baseUrl + 'TargetTemplate/Update/' + targetTemplate.TargetTemplateId).search({ viewInfo: true });
            }
            else {
                $location.path(baseUrl + 'TargetTemplate/Update/' + targetTemplate.TargetTemplateId).search({ viewKPIs: true });
            }
        };

        vm.deleteTargetTemplate = function (targetTemplate) {
            TargetTemplateModel.openPopupDeleteTargetTemplate(vm, targetTemplate);
        };

        vm.detail = function (targetTemplate) {
            $location.path(baseUrl + 'TargetTemplate/Detail/' + targetTemplate.TargetTemplateId);
        };

        function initialize() {
            TargetTemplateModel.getPermissionOnTargetTemplate().then(function () {
                vm.permission = TargetTemplateModel.permission;
            });

            CommonModel.getTargetType().then(function () {
                vm.targetTypes = CommonModel.targetTypes;
            });

            getTargetTemplate(vm.filter.targetTypeId, vm.filter.isActive);
        }

        initialize();
    }
})(angular);