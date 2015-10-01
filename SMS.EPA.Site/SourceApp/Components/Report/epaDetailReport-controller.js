'use strict';
/*global reportModule*/

reportModule.controller('EPADetailReportController',
['$scope', 'ReportModel', 'CommonModel', 'utilitiesFactory',
function ($scope, ReportModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.treeDepartment = CommonModel.treeDepartment;
    vm.periods = CommonModel.periods;
    vm.epaDetail = ReportModel.epaDetail;
    vm.scoredBy = [
        { ColumnId: 7, ProcessId: 2, Name: 'Self' },
        { ColumnId: 8, ProcessId: 3, Name: 'Evaluator' },
        { ColumnId: 9, ProcessId: 4, Name: 'Reviewer' },
        { ColumnId: 10, ProcessId: 5, Name: 'Management' }
    ];

    vm.hideColumns = [];
    vm.showColumns = [7, 8, 9, 10];
    vm.showColumnsOriginal = [7, 8, 9, 10];

    vm.pageOption = utilitiesFactory.pageOption;

    vm.filter = {
        periodId: '',
        departmentId: '',
        pageIndex: utilitiesFactory.pageOption.pageIndex,
        itemsPerPage: utilitiesFactory.pageOption.itemsPerPage
    };

    var getShowHideColumns = function () {
        vm.hideColumns = [], vm.showColumns = [];

        angular.forEach(vm.scoredBy, function (item) {
            if (item.ticked) {
                vm.showColumns.push(item.ColumnId);
            }
            else {
                vm.hideColumns.push(item.ColumnId);
            }
        });

        if (vm.hideColumns.length === vm.showColumnsOriginal.length) {
            vm.hideColumns = [];
            vm.showColumns = angular.copy(vm.showColumnsOriginal);
        }
    };

    var getEPADetailReport = function (filter) {
        ReportModel.getEPADetailReport(filter.periodId, filter.departmentId, filter.pageIndex, filter.itemsPerPage).then(function () {
            vm.epaDetail = ReportModel.epaDetail;
        });
    };

    vm.pageChanging = function (pageIndex, itemsPerPage) {
        vm.filter.pageIndex = pageIndex;
        vm.filter.itemsPerPage = itemsPerPage;

        getEPADetailReport(vm.filter);
    };

    vm.query = function (filter) {
        getShowHideColumns();

        getEPADetailReport(filter);
    };

    vm.exportReport = function () {
        vm.exportCriteria = {
            periodId: vm.filter.periodId,
            departmentId: vm.filter.departmentId
        };

        $scope.$broadcast('exportEpaDetailReport');
    };

    vm.initialize = function () {
        CommonModel.getDepartment().then(function () {
            vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
        });

        CommonModel.getPeriod().then(function () {
            vm.periods = CommonModel.periods;
        });

        getEPADetailReport(vm.filter);
    };

    vm.initialize();
}]);