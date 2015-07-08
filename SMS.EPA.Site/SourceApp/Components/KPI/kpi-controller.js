'use strict';
/*global kpiModule*/

kpiModule.controller('KPIController',
['KPIModel', 'KPIGroupModel', 'CommonModel', 'utilitiesFactory',
function (KPIModel, KPIGroupModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.kpis = KPIModel.kpis;
    vm.kpiGroups = KPIGroupModel.kpiGroups;

    vm.filter = {
        kpiGroupId: '',
        isActive: 'true'
    };

    utilitiesFactory.numberEntryList(vm);
    vm.pageSize = 15;
    vm.currentPage = 1;

    var getKPI = function (kpiGroupId, isActive) {
        KPIModel.getKPI(kpiGroupId, isActive).then(function () {
            vm.kpis = KPIModel.kpis;
            utilitiesFactory.searchOnPage(vm, vm.kpis);
        });
    };

    vm.initialize = function () {
        KPIGroupModel.getKPIGroup().then(function () {
            vm.kpiGroups = KPIGroupModel.kpiGroups;
        });

        getKPI(vm.filter.kpiGroupId, vm.filter.isActive);
    };

    vm.initialize();

    vm.query = function (filter) {
        getKPI(filter.kpiGroupId, filter.isActive);
    };

    /// Create KPI.
    vm.createKPI = function () {
        KPIModel.openPopupCreateKPI(vm).then()['finally'](function () {
            vm.modalInstance = '';
            getKPI(vm.filter.kpiGroupId, vm.filter.isActive);
        });
    };

    /// Get index of row in table.
    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };
} ]);

kpiModule.controller('CreateKPIController',
['$modalInstance', 'KPIModel', 'KPIGroupModel',
function ($modalInstance, KPIModel, KPIGroupModel) {
    var vm = this;

    vm.currentChosenKPIGroups = [];
    vm.kpiGroups = KPIGroupModel.kpiGroups;

    vm.kpi = {
        Name: '',
        KPIGroupIds: '',
        HasComment: false,
        IsRequiredComment: false,
        IsRequiredEvaluate: false,
        Description: ''
    };

    vm.initialize = function () {
        KPIGroupModel.getKPIGroup(true).then(function () {
            vm.kpiGroups = KPIGroupModel.kpiGroups;
        });
    };

    vm.initialize();

    vm.changeHasComment = function () {
        if (vm.kpi.HasComment === false) {
            vm.kpi.IsRequiredComment = false;
        }
    };

    vm.save = function (isCreateAndContinued) {
        if (KPIModel.validateKPI(vm.kpi, vm.currentChosenKPIGroups)) {
            vm.kpi.KPIGroupIds = vm.currentChosenKPIGroups.join(',');
            KPIModel.insertKPI(vm, isCreateAndContinued, $modalInstance);
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
} ]);