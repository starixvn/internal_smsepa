(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('reportModule').controller('EPADetailReportController', epaDetailReportController);
    epaDetailReportController.$inject = ['$scope', '$routeParams', 'ReportModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function epaDetailReportController($scope, $routeParams, ReportModel, CommonModel, utilitiesFactory, lockScreenService) {
        let vm = this;

        vm.epaDetail = [];
        vm.hideColumns = [];
        vm.pageOption = utilitiesFactory.pageOption;
        vm.periods = [];
        vm.scoredBy = [
            { Id: 2, ColumnId: 11, PhaseId: 2, Text: 'Self', Enabled: true, Checked: false, IsLeaf: true },
            { Id: 3, ColumnId: 12, PhaseId: 3, Text: 'Evaluator', Enabled: true, Checked: false, IsLeaf: true },
            { Id: 4, ColumnId: 13, PhaseId: 4, Text: 'Reviewer', Enabled: true, Checked: false, IsLeaf: true },
            { Id: 5, ColumnId: 14, PhaseId: 5, Text: 'Management', Enabled: true, Checked: false, IsLeaf: true }
        ];
        vm.showColumns = [11, 12, 13, 14];
        vm.showColumnsOriginal = [12, 12, 13, 14];
        vm.orgChart = [];

        initialize();

        function getEPADetailReport(filter) {
            lockScreenService.start();

            ReportModel.getEPADetailReport(
                filter.periodId,
                filter.teamIds,
                filter.isLocal,
                filter.employeeId,
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

        function initFilter() {
            const filter = {
                periodId: $routeParams.periodId ? parseInt($routeParams.periodId) : '',
                teamIds: $routeParams.teamIds,
                isLocal: $routeParams.isLocal ? $routeParams.isLocal : '',
                phaseIds: $routeParams.phaseIds ? $routeParams.phaseIds : '',
                sortColumnName: '',
                sortDirection: '',
                pageIndex: utilitiesFactory.pageOption.pageIndex,
                itemsPerPage: utilitiesFactory.pageOption.itemsPerPage,
                employee: null,
                employeeId: $routeParams.employeeId ? parseInt($routeParams.employeeId) : ''
            };

            if (filter.employeeId) {
                CommonModel.getEmployeeById(filter.employeeId).then(function () {
                    filter.employee = CommonModel.employee;
                    vm.searchKeyword = filter.employee.NickName + ' (' + filter.employee.FullName + ')';
                });
            }

            return filter;
        }

        function initialize() {
            vm.filter = initFilter();
            getShowHideColumns();

            CommonModel.getOrgChart().then(function () {
                vm.orgChart = CommonModel.orgChart;
            });

            ReportModel.getAllPeriodIncludeImport().then(function () {
                vm.periods = ReportModel.periods;

                if (!vm.filter.periodId && vm.periods.length > 0) {
                    vm.filter.periodId = vm.periods[0].PeriodId;
                }

                getEPADetailReport(vm.filter);
            });
        }

        vm.getEmployee = function () {
            return ReportModel.getEmployeeByActorId(vm.searchKeyword, vm.filter.periodId, vm.filter.teamIds, vm.filter.isLocal);
        };

        vm.exportReport = function () {
            vm.exportCriteria = {
                periodId: vm.filter.periodId,
                teamIds: vm.filter.teamIds,
                isLocal: vm.filter.isLocal,
                employeeId: vm.filter.employee ? vm.filter.employee.EmployeeId : null,
                phaseIds: vm.filter.phaseIds,
                sortColumnName: vm.filter.sortColumnName,
                sortDirection: vm.filter.sortDirection
            };

            $scope.$broadcast('exportEpaDetailReport');
        };

        vm.exportRecommendationReport = function () {
            vm.exportRecommendationCriteria = {
                periodId: vm.filter.periodId
            };

            $scope.$broadcast('exportRecommendationSummaryReport');
        };

        vm.exportCommentReport = function () {
            const periodName = vm.periods.filter(period => period.PeriodId == vm.filter.periodId)[0].Name;
            vm.exportCommentCriteria = {
                periodId: vm.filter.periodId,
                periodName: periodName
            };

            $scope.$broadcast('exportCommentSummaryReport');
        };

        vm.query = function (filter) {
            getShowHideColumns();
            filter.pageIndex = utilitiesFactory.pageOption.pageIndex;
            filter.sortColumnName = '';
            filter.sortDirection = '';

            if (filter.employee) {
                vm.filter.employeeId = filter.employee.EmployeeId;
            }

            getEPADetailReport(filter);
        };

        vm.sortChanging = function (sortColumnName, sortDirection) {
            vm.filter.sortColumnName = sortColumnName;
            vm.filter.sortDirection = sortDirection;

            getEPADetailReport(vm.filter);
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

        vm.onSelectPhase = function (phaseIds) {
            vm.filter.phaseIds = phaseIds;
        };
    }
})(angular);