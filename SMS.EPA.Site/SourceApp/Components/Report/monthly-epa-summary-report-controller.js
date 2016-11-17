(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('reportModule').controller('MonthlyEPASummaryReportController', monthlyEPASummaryReportController);
    monthlyEPASummaryReportController.$inject = ['$scope', 'ReportModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function monthlyEPASummaryReportController($scope, ReportModel, CommonModel, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.epaSummary = [];
        vm.filter = {
            periodIds: '',
            departmentId: '',
            isLocal: '',
            pageIndex: utilitiesFactory.pageOption.pageIndex,
            itemsPerPage: utilitiesFactory.pageOption.itemsPerPage,
            sortColumnName: '',
            sortDirection: ''
        };
        vm.pageOption = utilitiesFactory.pageOption;
        vm.periods = CommonModel.monthlyPeriods;
        vm.treeDepartment = CommonModel.treeDepartment;

        initialize();

        function getHalfTime(period) {
            var endDate = new Date(period.EndDate);
            if (endDate.getMonth() >= 0 && endDate.getMonth() <= 5) {
                return "1st Half";
            }

            if (endDate.getMonth() >= 6 && endDate.getMonth() <= 11) {
                return "2nd Half";
            }
        }

        function getHalfTimesOfPeriod(periods, year) {
            var halfTimes = [];

            angular.forEach(periods, function (period) {
                if (period.Year === year) {
                    var halfTime = getHalfTime(period),
                        index = halfTimes.indexOf(halfTime);
                    if (index === -1) {
                        halfTimes.push(halfTime);
                    }
                }
            });

            return halfTimes;
        }

        function getMonthlyEPASummaryReport() {
            lockScreenService.start();

            ReportModel.getMonthlyEPASummaryReport(
                vm.filter.periodIds,
                vm.filter.departmentId,
                vm.filter.isLocal,
                vm.filter.sortColumnName,
                vm.filter.sortDirection)
            .then(function () {
                vm.epaSummary = ReportModel.epaSummary;
                lockScreenService.stop();
            });
        }

        function getTheFirstPeriod(periods) {
            var index = -1;
            angular.forEach(periods, function (value, key) {
                if (index === -1 && angular.isDefined(value.PeriodId)) {
                    index = key;
                }
            });

            return index;
        }

        function getYearsOfPeriod(periods) {
            var years = [];
            angular.forEach(periods, function (period) {
                if (years.indexOf(period.Year) === -1) {
                    years.push(period.Year);
                }
            });

            return years;
        }

        function groupPeriodByYearAndHalfTime(periods) {
            var result = [],
                years = getYearsOfPeriod(periods);

            angular.forEach(years, function (year) {
                result.push({ Name: '<strong>' + year + '</strong>', msGroup: true });
                var halfTimes = getHalfTimesOfPeriod(periods, year);
                angular.forEach(halfTimes, function (halfTime) {
                    result.push({ Name: '<strong>' + halfTime + '</strong>', msGroup: true });
                    angular.forEach(periods, function (period) {
                        if (period.Year === year && getHalfTime(period) === halfTime) {
                            result.push(period);
                        }
                    });
                    result.push({ msGroup: false });
                });
                result.push({ msGroup: false });
            });

            var selectedIndex = getTheFirstPeriod(result);
            if (selectedIndex !== -1) {
                result[selectedIndex].ticked = true;
            }

            return result;
        }

        function initialize() {
            CommonModel.getDepartment().then(function () {
                vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
            });

            CommonModel.getAllMonthlyPeriod().then(function () {
                vm.periods = groupPeriodByYearAndHalfTime(CommonModel.monthlyPeriods);
                vm.filter.periodIds = utilitiesFactory.getSelectedData(vm.periods, 'PeriodId');
                getMonthlyEPASummaryReport();
            });
        }

        vm.exportReport = function () {
            vm.exportCriteria = {
                periodIds: utilitiesFactory.getSelectedData(vm.periods, 'PeriodId'),
                departmentId: vm.filter.departmentId,
                isLocal: vm.filter.isLocal,
                sortColumnName: vm.filter.sortColumnName,
                sortDirection: vm.filter.sortDirection
            };

            $scope.$broadcast('exportMonthlyEpaSummaryReport');
        };

        vm.query = function () {
            vm.filter.pageIndex = utilitiesFactory.pageOption.pageIndex;
            vm.filter.periodIds = utilitiesFactory.getSelectedData(vm.periods, 'PeriodId');
            vm.filter.sortColumnName = '';
            vm.filter.sortDirection = '';

            getMonthlyEPASummaryReport();
        };

        vm.sortChanging = function (sortColumnName, sortDirection) {
            vm.filter.sortColumnName = sortColumnName;
            vm.filter.sortDirection = sortDirection;

            getMonthlyEPASummaryReport();
        };
    }
})(angular);