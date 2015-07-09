'use strict';
/*global kpiModule*/

kpiModule.controller('KPIGroupController',
['KPIGroupModel', 'CommonModel', 'utilitiesFactory',
function (KPIGroupModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.targetTypes = CommonModel.targetTypes;
    vm.kpiGroups = KPIGroupModel.kpiGroups;

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

    /// Get index of row in table.
    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };
} ]);

kpiModule.controller('CreateKPIGroupController',
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
            KPIGroupModel.insertKPIGroup(vm.kpiGroup, $modalInstance);
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
} ]);