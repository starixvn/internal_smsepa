'use strict';
/*global reportModule*/

reportModule.controller('EPASummaryReportController',
['ReportModel', 'CommonModel',
function (ReportModel, CommonModel) {
    var vm = this;

    vm.treeDepartment = CommonModel.treeDepartment;
    vm.periods = CommonModel.periods;
    vm.epaSummaries = ReportModel.epaSummaries;

    vm.filter = {
        periodIds: '',
        departmentId: ''
    };

    var getEPASummaryReport = function (filter) {
        ReportModel.getEPASummaryReport(filter.periodIds, filter.departmentId).then(function () {
            vm.epaSummaries = ReportModel.epaSummaries;
        });
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