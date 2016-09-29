(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('disciplineModule').controller('DisciplineSummaryByPeriodController', disciplineSummaryByPeriodController);
    disciplineSummaryByPeriodController.$inject = ['$scope', 'DisciplineSummaryModel', 'CommonModel', 'lockScreenService',
        'utilitiesFactory', 'historyFactory'];

    function disciplineSummaryByPeriodController($scope, DisciplineSummaryModel, CommonModel, lockScreenService,
        utilitiesFactory, historyFactory) {
        var vm = this;

        vm.currentPage = 1;
        vm.disciplineTypes = [
            { Id: 2, Name: 'No time in & out' },
            { Id: 3, Name: 'Tardiness' },
            { Id: 6, Name: 'Unpaid leave' },
            { Id: 7, Name: 'Undertime' },
            { Id: -1, Name: 'Misconduct case' }
        ];
        vm.employeeDisciplines = [];
        vm.pageSize = 15;

        vm.exportReport = exportReport;
        vm.getEmployee = getEmployee;
        vm.getIndex = getIndex;
        vm.query = query;
        vm.unSelectEmployee = unSelectEmployee;

        initialize();

        function exportReport(filter) {
            var index = vm.departments.map(function (e) { return e.DepartmentId; }).indexOf(filter.departmentId);

            vm.exportCriteria = angular.copy(filter);
            vm.exportCriteria.employeeName = filter.employee ? filter.employee.Name : '';
            vm.exportCriteria.departmentName = index !== -1 ? vm.departments[index].Name : '';
            $scope.$broadcast('exportDisciplineSummaryByPeriodReport');
        }

        function getDisciplineSummaryByPeriod(periodId, disciplineTypeId, departmentId, isLocal, employeeId) {
            lockScreenService.start();

            DisciplineSummaryModel.getDisciplineSummaryByPeriod(periodId, disciplineTypeId, departmentId, isLocal, employeeId).then(function () {
                vm.employeeDisciplines = DisciplineSummaryModel.employeeDisciplines;
                utilitiesFactory.searchOnPage(vm, vm.employeeDisciplines);
                lockScreenService.stop();
            });

            historyFactory.saveState(vm.filter);
        }

        function getEmployee() {
            return CommonModel.getAllEmployee(vm.searchEmployee, null, null, null, true, true);
        }

        function getIndex(index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        function initFilter() {
            var filter = historyFactory.getState();

            return filter ? filter : {
                periodId: null,
                employee: null,
                departmentId: null,
                isLocal: '',
                employeeId: null,
                disciplineTypeId: null
            };
        }

        function initialize() {
            utilitiesFactory.numberEntryList(vm);
            vm.filter = initFilter();

            if (vm.filter.employeeId) {
                CommonModel.getEmployeeById(vm.filter.employeeId).then(function () {
                    vm.filter.employee = CommonModel.employee;
                    vm.searchEmployee = vm.filter.employee.NickName + ' (' + vm.filter.employee.FullName + ')';
                });
            }

            CommonModel.getDepartment().then(function () {
                vm.departments = CommonModel.departments;
                vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
            });

            CommonModel.getPeriod(utilitiesFactory.targetType.SemiAnnual, true).then(function () {
                vm.periods = CommonModel.periods;
            });

            getDisciplineSummaryByPeriod(vm.filter.periodId, vm.filter.disciplineTypeId, vm.filter.departmentId, vm.filter.isLocal, vm.filter.employeeId);
        }

        function query(filter) {
            var employeeId = null;
            vm.searchKey = '';
            vm.currentPage = 1;

            if (filter.employee) {
                employeeId = filter.employee.EmployeeId;
                vm.filter.employeeId = employeeId;
            }

            getDisciplineSummaryByPeriod(filter.periodId, vm.filter.disciplineTypeId, filter.departmentId, filter.isLocal, employeeId);
        }

        function unSelectEmployee() {
            if (vm.searchEmployee.trim() === '' && vm.filter.employee) {
                vm.filter.employee = null;
                vm.filter.employeeId = null;
            }
        }
    }
})(angular);