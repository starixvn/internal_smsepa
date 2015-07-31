'use strict';
/*global kpiGroupModule*/

kpiGroupModule.controller('KPIGroupController',
['KPIGroupModel', 'KPIModel', 'CommonModel', 'utilitiesFactory',
function (KPIGroupModel, KPIModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.targetTypes = CommonModel.targetTypes;
    vm.kpiGroups = KPIGroupModel.kpiGroups;
    vm.permission = KPIGroupModel.permission;

    vm.filter = {
        targetTypeId: '',
        isActive: 'true'
    };

    utilitiesFactory.numberEntryList(vm);
    vm.pageSize = 15;
    vm.currentPage = 1;

    var getKPIGroup = function (targetTypeId, isActive) {
        KPIGroupModel.getKPIGroup(targetTypeId, isActive).then(function () {
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

        CommonModel.getTargetType().then(function () {
            vm.targetTypes = CommonModel.targetTypes;
        });

        getKPIGroup(vm.filter.targetTypeId, vm.filter.isActive);
    };

    vm.initialize();

    vm.query = function (filter) {
        getKPIGroup(filter.targetTypeId, filter.isActive);
    };

    /// Create KPI Group.
    vm.createKPIGroup = function () {
        KPIGroupModel.openPopupCreateKPIGroup(vm).then()['finally'](function () {
            vm.modalInstance = '';
            getKPIGroup(vm.filter.targetTypeId, vm.filter.isActive);
        });
    };

    /// Update KPI Group.
    vm.updateKPIGroup = function (kpiGroup) {
        KPIGroupModel.openPopupUpdateKPIGroup(vm, kpiGroup).then(function (updatedKPIGroup) {
            kpiGroup.Name = updatedKPIGroup.Name;
            kpiGroup.TargetType = updatedKPIGroup.TargetType;
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
        vm.viewKPIUrl = '/KPI/Index' + '?groupId=' + kpiGroupId;
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
['$modalInstance', 'KPIGroupModel', 'CommonModel',
function ($modalInstance, KPIGroupModel, CommonModel) {
    var vm = this;

    vm.targetTypes = CommonModel.targetTypes;

    vm.kpiGroup = {
        Name: '',
        TargetTypeId: '',
        IsActive: true,
        Description: ''
    };

    vm.initialize = function () {
        CommonModel.getTargetType(true).then(function () {
            vm.targetTypes = CommonModel.targetTypes;
        });
    };

    vm.initialize();

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
['$modalInstance', 'KPIGroupModel', 'CommonModel', 'kpiGroup',
function ($modalInstance, KPIGroupModel, CommonModel, kpiGroup) {
    var vm = this;

    vm.targetTypes = CommonModel.targetTypes;

    vm.kpiGroup = {
        KPIGroupId: kpiGroup.KPIGroupId,
        Name: kpiGroup.Name,
        TargetTypeId: kpiGroup.TargetType.TargetTypeId,
        IsActive: kpiGroup.IsActive,
        Description: kpiGroup.Description,
        TargetType: {}
    };

    vm.initialize = function () {
        CommonModel.getTargetType(true).then(function () {
            vm.targetTypes = CommonModel.targetTypes;

            var index = vm.targetTypes.map(function (e) { return e.TargetTypeId; }).indexOf(kpiGroup.TargetType.TargetTypeId);
            if (index === -1) {
                vm.targetTypes.push(kpiGroup.TargetType);
            }
        });
    };

    vm.initialize();

    vm.save = function () {
        if (KPIGroupModel.validateKPIGroup(vm.kpiGroup)) {
            var index = vm.targetTypes.map(function (e) { return e.TargetTypeId; }).indexOf(vm.kpiGroup.TargetTypeId);
            if (index !== -1) {
                vm.kpiGroup.TargetType = vm.targetTypes[index];
            }
             
            KPIGroupModel.updateKPIGroup(vm.kpiGroup, $modalInstance);
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
} ]);