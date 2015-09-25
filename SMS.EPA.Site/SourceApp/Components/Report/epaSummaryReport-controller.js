'use strict';
/*global reportModule*/

reportModule.controller('EPASummaryReportController',
['ReportModel', 'CommonModel', 'utilitiesFactory',
function (ReportModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.treeDepartment = CommonModel.treeDepartment;
    vm.periods = CommonModel.periods;
    vm.epaSummary = ReportModel.epaSummary;

    vm.pageOption = utilitiesFactory.pageOption;

    vm.filter = {
        periodIds: '',
        departmentId: '',
        pageIndex: utilitiesFactory.pageOption.pageIndex,
        itemsPerPage: utilitiesFactory.pageOption.itemsPerPage
    };

    var getEPASummaryReport = function (filter) {
        ReportModel.getEPASummaryReport(filter.periodIds, filter.departmentId).then(function () {
            vm.epaSummary = ReportModel.epaSummary;
        });
    };

    vm.pageChanging = function (pageIndex, itemsPerPage) {
        vm.filter.pageIndex = pageIndex;
        vm.filter.itemsPerPage = itemsPerPage;

        vm.getEPASummaryReport(vm.filter);
    };

    vm.search = function (filter) {
        getEPASummaryReport(filter);
    };

    vm.initialize = function () {
        CommonModel.getDepartment().then(function () {
            vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
        });

        CommonModel.getPeriod().then(function () {
            vm.periods = CommonModel.periods;
        });

        getEPASummaryReport(vm.filter);
    };

    vm.initialize();
}]);