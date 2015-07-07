'use strict';
/*global kpiModule*/

kpiModule.controller('KPIController',
['KPIModel', 'CommonModel', 'utilitiesFactory',
function (KPIModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.kpis = KPIModel.kpis;

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

    getKPI(vm.filter.kpiGroupId, vm.filter.isActive);

    vm.query = function (filter) {
        getKPI(filter.kpiGroupId, filter.isActive);
    };

    /// Get index of row in table.
    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };
} ]);