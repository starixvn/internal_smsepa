(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('reportModule').controller('EPASummaryReportController', epaSummaryReportController);
    epaSummaryReportController.$inject = ['$scope', 'ReportModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function epaSummaryReportController($scope, ReportModel, CommonModel, utilitiesFactory, lockScreenService) {
        let vm = this;

        vm.epaSummary = [];
        vm.filter = {
            periodIds: '',
            teamIds: '',
            isLocal: '',
            sortColumnName: '',
            sortDirection: '',
            pageIndex: utilitiesFactory.pageOption.pageIndex,
            itemsPerPage: utilitiesFactory.pageOption.itemsPerPage,
            employee: null,
            employeeId: null
        };
        vm.pageOption = utilitiesFactory.pageOption;
        vm.periods = [];
        vm.orgChart = [];

        initialize();

        function getEPASummaryReport(filter) {
            lockScreenService.start();

            ReportModel.getEPASummaryReport(
                filter.periodIds,
                filter.teamIds,
                filter.isLocal,
                filter.employeeId,
                filter.sortColumnName,
                filter.sortDirection)
            .then(function () {
                vm.epaSummary = ReportModel.epaSummary;
                lockScreenService.stop();
            });
        }

        function groupingPeriods(periods) {
            const result = [], years = [];
            let startGrouping, endGrouping;

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
        }

        function initialize() {
            CommonModel.getOrgChart().then(function () {
                vm.orgChart = CommonModel.orgChart;
            });

            ReportModel.getAllPeriodIncludeImport().then(function () {
                vm.periods = groupingPeriods(ReportModel.periods);

                getEPASummaryReport(vm.filter);
            });
        }

        vm.getEmployee = function () {
            vm.filter.periodIds = utilitiesFactory.getSelectedData(vm.periods, 'PeriodId');

            return ReportModel.getEmployeeByActorId(vm.searchKeyword, vm.filter.periodIds, vm.filter.teamIds, vm.filter.isLocal);
        };

        vm.exportReport = function () {
            vm.exportCriteria = {
                periodIds: utilitiesFactory.getSelectedData(vm.periods, 'PeriodId'),
                teamIds: vm.filter.teamIds,
                isLocal: vm.filter.isLocal,
                employeeId: vm.filter.employee ? vm.filter.employee.EmployeeId : null,
                sortColumnName: vm.filter.sortColumnName,
                sortDirection: vm.filter.sortDirection
            };

            $scope.$broadcast('exportEpaSummaryReport');
        };

        vm.query = function (filter) {
            filter.pageIndex = utilitiesFactory.pageOption.pageIndex;
            filter.periodIds = utilitiesFactory.getSelectedData(vm.periods, 'PeriodId');
            filter.sortColumnName = '';
            filter.sortDirection = '';

            if (filter.employee) {
                vm.filter.employeeId = filter.employee.EmployeeId;
            }

            getEPASummaryReport(filter);
        };

        vm.sortChanging = function (sortColumnName, sortDirection) {
            vm.filter.sortColumnName = sortColumnName;
            vm.filter.sortDirection = sortDirection;

            getEPASummaryReport(vm.filter);
        };

        vm.unSelectEmployee = function () {
            if (vm.searchKeyword.trim() === '') {
                vm.filter.employee = null;
                vm.filter.employeeId = null;
            }
        };

        vm.onSelectTeam = function (teamIds) {
            vm.filter.teamIds = teamIds;
        };
    }
})(angular);