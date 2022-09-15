(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('reportModule').controller('MonthlyEPADetailReportController', monthlyEPADetailReportController);
    monthlyEPADetailReportController.$inject = ['$scope', 'ReportModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function monthlyEPADetailReportController($scope, ReportModel, CommonModel, utilitiesFactory, lockScreenService) {
        let vm = this;

        vm.epaDetail = [];
        vm.hideColumns = [];
        vm.pageOption = utilitiesFactory.pageOption;
        vm.periods = [];
        vm.scoredBy = [
            { ColumnId: 7, PhaseId: 2, Name: 'Self' },
            { ColumnId: 8, PhaseId: 3, Name: 'Evaluator' },
            { ColumnId: 9, PhaseId: 4, Name: 'Reviewer' },
        ];
        vm.showColumns = [7, 8, 9];
        vm.showColumnsOriginal = [7, 8, 9];
        vm.orgChart = [];

        initialize();

        function initFilter() {
            return {
                periodId: '',
                teamIds: null,
                isLocal: '',
                phaseIds: '',
                pageIndex: utilitiesFactory.pageOption.pageIndex,
                itemsPerPage: utilitiesFactory.pageOption.itemsPerPage,
                sortColumnName: '',
                sortDirection: ''
            };
        }

        function initialize() {
            vm.filter = initFilter();

            CommonModel.getOrgChart().then(function () {
                vm.orgChart = CommonModel.orgChart;
            });

            CommonModel.getAllMonthlyPeriod().then(function () {
                vm.periods = CommonModel.monthlyPeriods;

                if (vm.periods.length > 0) {
                    vm.filter.periodId = vm.periods[0].PeriodId;
                }

                getMonthlyEPADetailReport(vm.filter);
            });
        }

        function getMonthlyEPADetailReport(filter) {
            lockScreenService.start();

            ReportModel.getMonthlyEPADetailReport(
                filter.periodId,
                filter.teamIds,
                filter.isLocal,
                filter.phaseIds,
                filter.sortColumnName,
                filter.sortDirection)
            .then(function () {
                vm.epaDetail = ReportModel.epaDetail;
                lockScreenService.stop();
            });
        }

        function getShowHideColumns() {
            vm.hideColumns = [], vm.showColumns = [];

            angular.forEach(vm.scoredBy, function (item) {
                if (item.ticked) {
                    vm.showColumns.push(item.ColumnId);
                } else {
                    vm.hideColumns.push(item.ColumnId);
                }
            });

            if (vm.hideColumns.length === vm.showColumnsOriginal.length) {
                vm.hideColumns = [];
                vm.showColumns = angular.copy(vm.showColumnsOriginal);
            }
        }

        vm.exportReport = function () {
            vm.exportCriteria = {
                periodId: vm.filter.periodId,
                teamIds: vm.filter.teamIds,
                isLocal: vm.filter.isLocal,
                phaseIds: vm.filter.phaseIds,
                sortColumnName: vm.filter.sortColumnName,
                sortDirection: vm.filter.sortDirection
            };

            $scope.$broadcast('exportMonthlyEpaDetailReport');
        };

        vm.query = function (filter) {
            getShowHideColumns();
            filter.pageIndex = utilitiesFactory.pageOption.pageIndex;
            filter.phaseIds = utilitiesFactory.getSelectedData(vm.scoredBy, 'PhaseId');
            filter.sortColumnName = '';
            filter.sortDirection = '';

            getMonthlyEPADetailReport(filter);
        };

        vm.sortChanging = function (sortColumnName, sortDirection) {
            vm.filter.sortColumnName = sortColumnName;
            vm.filter.sortDirection = sortDirection;

            getMonthlyEPADetailReport(vm.filter);
        };

        vm.onSelectTeam = function (teamIds) {
            vm.filter.teamIds = teamIds;
        };
    }
})(angular);