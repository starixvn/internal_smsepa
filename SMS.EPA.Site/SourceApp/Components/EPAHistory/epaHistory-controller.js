'use strict'
/*global epaHistoryModule*/

epaHistoryModule.controller('EPAHistoryController',
['EPAHistoryModel', 'utilitiesFactory', 'CommonModel', function (EPAHistoryModel, utilitiesFactory, CommonModel) {
    var vm = this;
    vm.epas = EPAHistoryModel.epas;
    vm.periodOptions = CommonModel.periodOptions;

    vm.filter = {
        periodId: null
    };

    vm.currentPage = 1;
    vm.pageSize = 20;

    utilitiesFactory.numberEntryList(vm);
    utilitiesFactory.getDateFormat(vm);

    CommonModel.getAllPeriodIncludeImport().then(function () {
        vm.periodOptions = CommonModel.periodOptions;
        vm.filter.periodId = vm.periodOptions.length > 0 ? vm.periodOptions[0].PeriodId : null;
        getEPAHistory(vm.filter.periodId);
    });

    var getEPAHistory = function (periodId) {
        EPAHistoryModel.getEPAHistory(periodId).then(function () {
            vm.epas = EPAHistoryModel.epas;
            utilitiesFactory.searchOnPage(vm, vm.periods);
        });
    };

    vm.query = function (filter) {
        getEPAHistory(filter.periodId);
    };

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };
}]);