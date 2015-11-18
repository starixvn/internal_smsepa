'use strict';
/*global reportModule*/

reportModule.controller('EPASummaryReportController',
['$scope', 'ReportModel', 'CommonModel', 'utilitiesFactory', 'usSpinnerService',
function ($scope, ReportModel, CommonModel, utilitiesFactory, usSpinnerService) {
    var vm = this;

    vm.treeDepartment = CommonModel.treeDepartment;
    vm.periods = ReportModel.periods;
    vm.epaSummary = ReportModel.epaSummary;

    vm.pageOption = utilitiesFactory.pageOption;

    vm.filter = {
        periodIds: '',
        departmentId: '',
        isLocal: '',
        pageIndex: utilitiesFactory.pageOption.pageIndex,
        itemsPerPage: utilitiesFactory.pageOption.itemsPerPage
    };

    var getEPASummaryReport = function (filter) {
        usSpinnerService.spin("spinner-1");

        ReportModel.getEPASummaryReport(filter.periodIds, filter.departmentId, filter.isLocal, filter.pageIndex, filter.itemsPerPage).then(function () {
            vm.epaSummary = ReportModel.epaSummary;
            usSpinnerService.stop('spinner-1');
        });
    };

    vm.pageChanging = function (pageIndex, itemsPerPage) {
        vm.filter.pageIndex = pageIndex;
        vm.filter.itemsPerPage = itemsPerPage;

        getEPASummaryReport(vm.filter);
    };

    vm.query = function (filter) {
        filter.pageIndex = utilitiesFactory.pageOption.pageIndex;
        filter.periodIds = utilitiesFactory.getSelectedData(vm.periods, 'PeriodId');

        getEPASummaryReport(filter);
    };

    vm.exportReport = function () {
        vm.exportCriteria = {
            periodIds: utilitiesFactory.getSelectedData(vm.periods, 'PeriodId'),
            departmentId: vm.filter.departmentId,
            isLocal: vm.filter.isLocal
        };

        $scope.$broadcast('exportEpaSummaryReport');
    };

    var groupingPeriods = function (periods) {
        var result = [], years = [];
        var startGrouping, endGrouping;

        angular.forEach(periods, function (period) {
            if (period.Year) {
                period.ticked = false;

                if (years.indexOf(period.Year) === -1) {
                    startGrouping = { Name: '<strong>' + period.Year + '</strong>', msGroup: true };
                    if (years.length === 0) {
                        result.push(startGrouping);
                    } else {
                        endGrouping = { msGroup: false };
                        result.push(endGrouping);
                        result.push(startGrouping);
                    }

                    years.push(period.Year);
                    result.push(period);
                } else {
                    result.push(period);
                }
            }
        });

        endGrouping = { msGroup: false };
        result.push(endGrouping);

        if (periods.length > 0 && result.length > 0) {
            result[1].ticked = true;
            vm.filter.periodIds = result[1].PeriodId;

            angular.forEach(periods, function (period) {
                if (!period.Year) {
                    period.ticked = false;
                    result.unshift(period);
                }
            });
        } else {
            angular.forEach(periods, function (period) {
                if (!period.Year) {
                    period.ticked = false;
                    result.unshift(period);
                }
            });

            if (result.length > 0) {
                result[0].ticked = true;
                vm.filter.periodIds = result[0].PeriodId;
            }
        }

        return result;
    };

    vm.initialize = function () {
        CommonModel.getDepartment().then(function () {
            vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
        });

        ReportModel.getAllPeriodIncludeImport().then(function () {
            vm.periods = groupingPeriods(ReportModel.periods);

            getEPASummaryReport(vm.filter);
        });
    };

    vm.initialize();
}]);