'use strict';

(function (angular) {
    angular.module('kpiGroupModule').controller('KPIGroupController', KPIGroupController);
    KPIGroupController.$inject = ['KPIGroupModel', 'KPIModel', 'CommonModel', 'utilitiesFactory', 'historyFactory', 'baseUrl'];

    angular.module('kpiGroupModule').controller('CreateKPIGroupController', CreateKPIGroupController);
    CreateKPIGroupController.$inject = ['$modalInstance', 'KPIGroupModel'];

    angular.module('kpiGroupModule').controller('UpdateKPIGroupController', UpdateKPIGroupController);
    UpdateKPIGroupController.$inject = ['$modalInstance', 'KPIGroupModel', 'kpiGroup'];

    function KPIGroupController(KPIGroupModel, KPIModel, CommonModel, utilitiesFactory, historyFactory, baseUrl) {
        var vm = this;

        vm.addKPI = addKPI;
        vm.createKPIGroup = createKPIGroup;
        vm.currentPage = 1;
        vm.deleteKPIGroup = deleteKPIGroup;
        vm.filter = {};
        vm.getIndex = getIndex;
        vm.kpiGroups = [];
        vm.pageSize = 15;
        vm.permission = 0;
        vm.policyEnum = utilitiesFactory.policy;
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
            KPIGroupModel.getKPIGroup(isActive).then(function () {
                vm.kpiGroups = KPIGroupModel.kpiGroups;
                utilitiesFactory.searchOnPage(vm, vm.kpiGroups);
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
            vm.viewKPIUrl = baseUrl + 'KPI/Index' + '?groupId=' + kpiGroupId;
        }
    }

    function CreateKPIGroupController($modalInstance, KPIGroupModel) {
        var vm = this;
        vm.cancel = cancel;
        vm.kpiGroup = {
            Name: '',
            IsActive: true,
            Description: ''
        };
        vm.save = save;

        function cancel() {
            $modalInstance.dismiss("cancel");
        }

        function save() {
            if (KPIGroupModel.validateKPIGroup(vm.kpiGroup)) {
                KPIGroupModel.createKPIGroup(vm.kpiGroup, $modalInstance);
            }
        }
    }

    function UpdateKPIGroupController($modalInstance, KPIGroupModel, kpiGroup) {
        var vm = this;

        vm.cancel = cancel;
        vm.kpiGroup = {
            KPIGroupId: kpiGroup.KPIGroupId,
            Name: kpiGroup.Name,
            IsActive: kpiGroup.IsActive,
            Description: kpiGroup.Description
        };
        vm.save = save;

        function cancel() {
            $modalInstance.dismiss("cancel");
        }

        function save() {
            if (KPIGroupModel.validateKPIGroup(vm.kpiGroup)) {
                KPIGroupModel.updateKPIGroup(vm.kpiGroup, $modalInstance);
            }
        }
    }
})(angular);