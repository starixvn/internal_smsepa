'use strict';
/*global kpiGroupModule*/

kpiGroupModule.controller('KPIGroupController',
['KPIGroupModel', 'KPIModel', 'CommonModel', 'utilitiesFactory', 'baseUrl',
function (KPIGroupModel, KPIModel, CommonModel, utilitiesFactory, baseUrl) {
    var vm = this;

    vm.kpiGroups = KPIGroupModel.kpiGroups;
    vm.permission = KPIGroupModel.permission;

    vm.filter = {
        isActive: 'true'
    };

    utilitiesFactory.numberEntryList(vm);
    vm.pageSize = 15;
    vm.currentPage = 1;

    var getKPIGroup = function (isActive) {
        KPIGroupModel.getKPIGroup(isActive).then(function () {
            vm.kpiGroups = KPIGroupModel.kpiGroups;
            utilitiesFactory.searchOnPage(vm, vm.kpiGroups);
        });
    };

    vm.initialize = function () {
        KPIModel.getUserPermissionOnKPI().then(function () {
            vm.permissionOnKPI = KPIModel.permission;
        });

        KPIGroupModel.getUserPermissionOnKPIGroup().then(function () {
            vm.permission = KPIGroupModel.permission;
        });

        getKPIGroup(vm.filter.isActive);
    };

    vm.initialize();

    vm.query = function (filter) {
        vm.searchKey = '';
        vm.currentPage = 1;
        getKPIGroup(filter.isActive);
    };

    /// Create KPI Group.
    vm.createKPIGroup = function () {
        KPIGroupModel.openPopupCreateKPIGroup(vm).then()['finally'](function () {
            vm.modalInstance = '';
            getKPIGroup(vm.filter.isActive);
        });
    };

    /// Update KPI Group.
    vm.updateKPIGroup = function (kpiGroup) {
        KPIGroupModel.openPopupUpdateKPIGroup(vm, kpiGroup).then(function (updatedKPIGroup) {
            kpiGroup.Name = updatedKPIGroup.Name;
            kpiGroup.IsActive = updatedKPIGroup.IsActive;
            kpiGroup.Description = updatedKPIGroup.Description;
        })['finally'](function () {
            vm.modalInstance = '';
        });
    };

    /// Update status KPI Group.
    vm.updateStatusKPIGroup = function (kpiGroup) {
        KPIGroupModel.openPopupUpdateStatusKPIGroup(kpiGroup);
    };

    /// Delete KPI Group.
    vm.deleteKPIGroup = function (kpiGroup) {
        KPIGroupModel.openPopupDeleteKPIGroup(vm, kpiGroup);
    };

    /// Add KPI.
    vm.addKPI = function (kpiGroup) {
        KPIModel.openPopupCreateKPI(vm, kpiGroup).then()['finally'](function () {
            vm.modalInstance = '';
        });
    };

    vm.viewKPI = function (kpiGroupId) {
        vm.viewKPIUrl = baseUrl + 'KPI/Index' + '?groupId=' + kpiGroupId;
    };

    /// Event read more / less text.
    utilitiesFactory.maxLengthDescription(vm);

    vm.readMore = function (item) {
        utilitiesFactory.readMore(item);
    };

    vm.readLess = function (item) {
        utilitiesFactory.readLess(item);
    };

    /// Get index of row in table.
    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };
} ]);

kpiGroupModule.controller('CreateKPIGroupController',
['$modalInstance', 'KPIGroupModel',
function ($modalInstance, KPIGroupModel) {
    var vm = this;

    vm.kpiGroup = {
        Name: '',
        IsActive: true,
        Description: ''
    };

    vm.save = function () {
        if (KPIGroupModel.validateKPIGroup(vm.kpiGroup)) {
            KPIGroupModel.createKPIGroup(vm.kpiGroup, $modalInstance);
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
} ]);

kpiGroupModule.controller('UpdateKPIGroupController',
['$modalInstance', 'KPIGroupModel', 'kpiGroup',
function ($modalInstance, KPIGroupModel, kpiGroup) {
    var vm = this;

    vm.kpiGroup = {
        KPIGroupId: kpiGroup.KPIGroupId,
        Name: kpiGroup.Name,
        IsActive: kpiGroup.IsActive,
        Description: kpiGroup.Description
    };

    vm.save = function () {
        if (KPIGroupModel.validateKPIGroup(vm.kpiGroup)) {
            KPIGroupModel.updateKPIGroup(vm.kpiGroup, $modalInstance);
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
} ]);