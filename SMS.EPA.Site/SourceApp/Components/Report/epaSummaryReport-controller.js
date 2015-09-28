'use strict';
/*global reportModule*/

reportModule.controller('EPASummaryReportController',
['$scope', 'ReportModel', 'CommonModel', 'utilitiesFactory',
function ($scope, ReportModel, CommonModel, utilitiesFactory) {
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
        ReportModel.getEPASummaryReport(filter.periodIds, filter.departmentId, filter.pageIndex, filter.itemsPerPage).then(function () {
            vm.epaSummary = ReportModel.epaSummary;
        });
    };

    vm.pageChanging = function (pageIndex, itemsPerPage) {
        vm.filter.pageIndex = pageIndex;
        vm.filter.itemsPerPage = itemsPerPage;

        getEPASummaryReport(vm.filter);
    };

    vm.query = function (filter) {
        filter.periodIds = utilitiesFactory.getSelectedData(vm.periods, 'PeriodId');

        getEPASummaryReport(filter);
    };

    vm.exportReport = function () {
        vm.exportCriteria = {
            periodIds: utilitiesFactory.getSelectedData(vm.periods, 'PeriodId'),
            departmentId: vm.filter.departmentId
        };

        $scope.$broadcast('exportEpaSummaryReport');
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