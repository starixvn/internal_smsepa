(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('reportModule').controller('EPADetailReportController', epaDetailReportController);
    epaDetailReportController.$inject = ['$scope', '$routeParams', 'ReportModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function epaDetailReportController($scope, $routeParams, ReportModel, CommonModel, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.epaDetail = [];
        vm.hideColumns = [];
        vm.pageOption = utilitiesFactory.pageOption;
        vm.periods = [];
        vm.scoredBy = [
            { ColumnId: 8, PhaseId: 2, Name: 'Self' },
            { ColumnId: 9, PhaseId: 3, Name: 'Evaluator' },
            { ColumnId: 10, PhaseId: 4, Name: 'Reviewer' },
            { ColumnId: 11, PhaseId: 5, Name: 'Management' }
        ];
        vm.showColumns = [8, 9, 10, 11];
        vm.showColumnsOriginal = [8, 9, 10, 11];
        vm.treeDepartment = {};

        initialize();

        function getEPADetailReport(filter) {
            lockScreenService.start();

            ReportModel.getEPADetailReport(
                filter.periodId,
                filter.departmentId,
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

        function initFilter() {
            var filter = {
                periodId: $routeParams.periodId ? parseInt($routeParams.periodId) : '',
                departmentId: $routeParams.departmentId ? parseInt($routeParams.departmentId) : '',
                isLocal: $routeParams.isLocal ? $routeParams.isLocal : '',
                phaseIds: $routeParams.phaseIds ? $routeParams.phaseIds : '',
                sortColumnName: '',
                sortDirection: '',
                pageIndex: utilitiesFactory.pageOption.pageIndex,
                itemsPerPage: utilitiesFactory.pageOption.itemsPerPage,
                employee: null,
                employeeId: $routeParams.employeeId ? $routeParams.employeeId : null
            };

            if (filter.employeeId) {
                CommonModel.getEmployeeById(filter.employeeId).then(function () {
                    filter.employee = CommonModel.employee;
                    vm.searchEmployee = filter.employee.NickName + ' (' + filter.employee.FullName + ')';
                });
            }

            return filter;
        }

        function initialize() {
            vm.filter = initFilter();
            utilitiesFactory.setSelectedItem(vm.scoredBy, 'PhaseId', vm.filter.phaseIds);
            getShowHideColumns();

            CommonModel.getDepartment().then(function () {
                vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
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
            return ReportModel.getEmployeeByActorId(vm.searchEmployee, vm.filter.periodId, vm.filter.departmentId, vm.filter.isLocal);
        };

        vm.exportReport = function () {
            vm.exportCriteria = {
                periodId: vm.filter.periodId,
                departmentId: vm.filter.departmentId,
                isLocal: vm.filter.isLocal,
                employeeId: vm.filter.employee ? vm.filter.employee.EmployeeId : null,
                phaseIds: vm.filter.phaseIds,
                sortColumnName: vm.filter.sortColumnName,
                sortDirection: vm.filter.sortDirection
            };

            $scope.$broadcast('exportEpaDetailReport');
        };

        vm.query = function (filter) {
            getShowHideColumns();
            filter.pageIndex = utilitiesFactory.pageOption.pageIndex;
            filter.phaseIds = utilitiesFactory.getSelectedData(vm.scoredBy, 'PhaseId');
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
            if (vm.searchEmployee.trim() === '') {
                vm.filter.employee = null;
                vm.filter.employeeId = null;
            }
        };
    }
})(angular);