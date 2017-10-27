(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('kpiGroupModule').controller('KPIGroupController', KPIGroupController);
    KPIGroupController.$inject = ['$location', 'KPIGroupModel', 'KPIModel', 'CommonModel',
        'utilitiesFactory', 'historyFactory', 'lockScreenService', 'baseUrl'];

    function KPIGroupController($location, KPIGroupModel, KPIModel, CommonModel, utilitiesFactory, historyFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.currentPage = 1;
        vm.filter = {};
        vm.kpiGroups = [];
        vm.pageSize = 15;
        vm.permission = 0;
        vm.policyEnum = utilitiesFactory.policy;

        vm.addKPI = addKPI;
        vm.createKPIGroup = createKPIGroup;
        vm.deleteKPIGroup = deleteKPIGroup;
        vm.getIndex = getIndex;
        vm.query = query;
        vm.readLess = readLess;
        vm.readMore = readMore;
        vm.updateKPIGroup = updateKPIGroup;
        vm.updateStatusKPIGroup = updateStatusKPIGroup;
        vm.viewKPI = viewKPI;

        utilitiesFactory.numberEntryList(vm);
        utilitiesFactory.maxLengthDescription(vm);
        initialize();

        function addKPI(kpiGroup) {
            KPIModel.openPopupCreateKPI(vm, kpiGroup).then()['finally'](function () {
                vm.modalInstance = '';
            });
        }

        function createKPIGroup() {
            KPIGroupModel.openPopupCreateKPIGroup(vm).then()['finally'](function () {
                vm.modalInstance = '';
                getKPIGroup(vm.filter.isActive);
            });
        }

        function deleteKPIGroup(kpiGroup) {
            KPIGroupModel.openPopupDeleteKPIGroup(vm, kpiGroup);
        }

        function getIndex(index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        function initFilter() {
            var filter = historyFactory.getState();

            return filter ? filter : { isActive: 'true' };
        }

        function initialize() {
            vm.filter = initFilter();
            KPIModel.getUserPermissionOnKPI().then(function () {
                vm.permissionOnKPI = KPIModel.permission;
            });

            KPIGroupModel.getUserPermissionOnKPIGroup().then(function () {
                vm.permission = KPIGroupModel.permission;
            });

            getKPIGroup(vm.filter.isActive);
        }

        function getKPIGroup(isActive) {
            lockScreenService.start();
            KPIGroupModel.getKPIGroup(isActive).then(function () {
                vm.kpiGroups = KPIGroupModel.kpiGroups;
                utilitiesFactory.searchOnPage(vm, vm.kpiGroups);
                lockScreenService.stop();
            });
            historyFactory.saveState(vm.filter);
        }

        function query(filter) {
            vm.searchKey = '';
            vm.currentPage = 1;
            getKPIGroup(filter.isActive);
        }

        function readMore(item) {
            utilitiesFactory.readMore(item);
        }

        function readLess(item) {
            utilitiesFactory.readLess(item);
        }

        function updateKPIGroup(kpiGroup) {
            KPIGroupModel.openPopupUpdateKPIGroup(vm, kpiGroup).then(function (updatedKPIGroup) {
                kpiGroup.Name = updatedKPIGroup.Name;
                kpiGroup.IsActive = updatedKPIGroup.IsActive;
                kpiGroup.Description = updatedKPIGroup.Description;
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }

        function updateStatusKPIGroup(kpiGroup) {
            KPIGroupModel.openPopupUpdateStatusKPIGroup(kpiGroup);
        }

        function viewKPI(kpiGroupId) {
            $location.path(baseUrl + 'KPI/Index' + '?groupId=' + kpiGroupId);
        }
    }
})(angular);