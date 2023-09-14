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
            { Id: 2, ColumnId: 10, PhaseId: 2, Text: 'Self', Enabled: true, Checked: false, IsLeaf: true },
            { Id: 3, ColumnId: 11, PhaseId: 3, Text: 'Evaluator', Enabled: true, Checked: false, IsLeaf: true },
            { Id: 4, ColumnId: 12, PhaseId: 4, Text: 'Reviewer', Enabled: true, Checked: false, IsLeaf: true },
        ];
        vm.showColumns = [10, 11, 12];
        vm.showColumnsOriginal = [10, 11, 12];
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
            vm.hideColumns = [];
            vm.showColumns = [];
            const phaseIds = vm.filter.phaseIds.split(',');

            angular.forEach(vm.scoredBy, function (item) {
                const index = phaseIds.findIndex(x => x === item.PhaseId.toString());
                if (index >= 0) {
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

        vm.onSelectPhase = function (phaseIds) {
            vm.filter.phaseIds = phaseIds;
        };
    }
})(angular);