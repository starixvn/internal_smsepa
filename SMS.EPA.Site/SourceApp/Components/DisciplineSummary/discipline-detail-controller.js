(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('disciplineModule').controller('AttendanceDetailController', attendanceDetailController);
    attendanceDetailController.$inject = ['$scope', '$routeParams', 'DisciplineSummaryModel', 'CommonModel',
        'utilitiesFactory', 'lockScreenService', 'historyFactory'];

    function attendanceDetailController($scope, $routeParams, DisciplineSummaryModel, CommonModel,
        utilitiesFactory, lockScreenService, historyFactory) {
        var vm = this;

        vm.employeeDisciplines = [];
        vm.calendar = {
            startDateOpened: false,
            endDateOpened: false
        };
        vm.currentPage = 1;
        vm.filter = {};
        vm.fromSummaryPage = $routeParams.id ? true : false;
        vm.pageSize = 15;
        vm.disciplineTypes = [];

        utilitiesFactory.numberEntryList(vm);
        utilitiesFactory.getDateFormat(vm);
        utilitiesFactory.getTimeFormat(vm);

        function getDisciplineDetail(startDate, endDate, employeeId, departmentId, isLocal, reasonId, hasIgnored) {
            lockScreenService.start();

            DisciplineSummaryModel.getDisciplineDetail(startDate, endDate, employeeId, departmentId, isLocal, reasonId, hasIgnored).then(function () {
                vm.employeeDisciplines = DisciplineSummaryModel.employeeDisciplines;
                utilitiesFactory.searchOnPage(vm, vm.employeeDisciplines);
                lockScreenService.stop();
            });
        }

        function initFilter() {
            var dateRangeFilter = DisciplineSummaryModel.getRangeDateFilter();

            vm.filter = {
                startDate: $routeParams.startDate ? new Date($routeParams.startDate) : dateRangeFilter.startDate,
                endDate: $routeParams.endDate ? new Date($routeParams.endDate) : dateRangeFilter.endDate,
                employee: null,
                departmentId: null,
                isLocal: $routeParams.isLocal ? $routeParams.isLocal : '',
                employeeId: $routeParams.id ? parseInt($routeParams.id) : null,
                reasonId: parseInt($routeParams.reasonId),
                hasIgnored: ''
            };

            if (vm.filter.employeeId) {
                CommonModel.getEmployeeById(vm.filter.employeeId).then(function () {
                    vm.filter.employee = CommonModel.employee;
                    vm.filter.employee.Name = vm.filter.employee.NickName;
                    vm.searchEmployee = vm.filter.employee.NickName + ' (' + vm.filter.employee.FullName + ')';
                });
            }
        }

        function initialize() {
            initFilter();

            CommonModel.getDepartment().then(function () {
                vm.departments = CommonModel.departments;
                vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
            });

            DisciplineSummaryModel.getDisciplineType(false, true).then(function () {
                vm.disciplineTypes = DisciplineSummaryModel.disciplineTypes;
            });

            getDisciplineDetail(
                vm.filter.startDate,
                vm.filter.endDate,
                vm.filter.employeeId,
                vm.filter.departmentId,
                vm.filter.isLocal,
                vm.filter.reasonId,
                vm.filter.hasIgnored);
        }

        initialize();

        vm.backToList = function () {
            historyFactory.back();
        };

        vm.exportReport = function (filter) {
            var index = vm.departments.map(function (e) { return e.DepartmentId; }).indexOf(filter.departmentId);

            vm.exportCriteria = angular.copy(filter);
            vm.exportCriteria.employeeName = filter.employee ? filter.employee.Name : '';
            vm.exportCriteria.departmentName = index !== -1 ? vm.departments[index].Name : '';
            vm.exportCriteria.isLocal = filter.isLocal;
            vm.exportCriteria.startDate = filter.startDate.toDateString();
            vm.exportCriteria.endDate = filter.endDate.toDateString();
            $scope.$broadcast('exportDisciplineDetailReport');
        };

        vm.getEmployee = function () {
            return CommonModel.getAllEmployee(vm.searchEmployee, null, null, null, true, true);
        };

        vm.openPopupEndDate = function (event) {
            utilitiesFactory.openPopupEndDate(event, vm);
        };

        vm.openPopupStartDate = function (event) {
            utilitiesFactory.openPopupStartDate(event, vm);
        };

        vm.query = function (filter) {
            var employeeId = null;

            if (filter.employee) {
                employeeId = filter.employee.EmployeeId;
            }

            getDisciplineDetail(
                vm.filter.startDate,
                vm.filter.endDate,
                employeeId,
                vm.filter.departmentId,
                vm.filter.isLocal,
                vm.filter.reasonId,
                vm.filter.hasIgnored);
        };

        vm.unSelectEmployee = function () {
            if (vm.searchEmployee.trim() === '') {
                if (vm.filter.employee) {
                    vm.filter.employee = null;
                    vm.filter.employeeId = null;
                }
            }
        };
    }
})(angular);