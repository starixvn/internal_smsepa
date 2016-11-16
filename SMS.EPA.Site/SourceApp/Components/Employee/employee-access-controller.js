(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('employeeModule').controller('EmployeeController', employeeController);
    employeeController.$inject = ['$window', '$scope', 'EmployeeModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function employeeController($window, $scope, EmployeeModel, CommonModel, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.currentPage = 1;
        vm.defaultPassword = EmployeeModel.defaultPassword;
        vm.employees = EmployeeModel.employees;
        vm.groupOptions = [];
        vm.pageSize = 20;
        vm.permission = EmployeeModel.permission;
        vm.policyEnum = utilitiesFactory.policy;

        vm.assignRole = assignRole;
        vm.createEmployee = createEmployee;
        vm.exportReport = exportReport;
        vm.getIndex = getIndex;
        vm.queryEmployee = queryEmployee;
        vm.resetPassword = resetPassword;
        vm.updateEmployee = updateEmployee;
        vm.updateHasMonthlyEPA = updateHasMonthlyEPA;

        utilitiesFactory.numberEntryList(vm);
        initialize();

        function assignRole(employee) {
            EmployeeModel.openPopupAssignRole(employee);
        }

        function createEmployee() {
            EmployeeModel.openPopupCreateNewEmployee(vm).then()['finally'](function () {
                vm.modalInstance = '';
                getEmployee(vm.filter.query, vm.filter.groupId, vm.filter.isLocal, vm.filter.isSpecialUser);
            });
        }

        function exportReport(filter) {
            vm.exportCriteria = filter;

            $scope.$broadcast('exportEmployeeInfoReport');
        }

        function getEmployee(query, groupId, isLocal, isSpecialUser, isDisabledUser, hasMonthlyEPA) {
            lockScreenService.start();
            if (!hasMonthlyEPA) {
                hasMonthlyEPA = '';
            }

            EmployeeModel.getAllEmployee(query, groupId, isLocal, isSpecialUser, isDisabledUser, hasMonthlyEPA).then(function () {
                vm.employees = EmployeeModel.employees;
                utilitiesFactory.searchOnPage(vm, vm.employees);
                lockScreenService.stop();
            });
        }

        function getIndex(index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        function initFilter() {
            vm.filter = {
                query: '',
                groupId: '',
                isLocal: '',
                isSpecialUser: '',
                isDisabledUser: '',
                hasMonthlyEPA: ''
            };
        }

        function initialize() {
            initFilter();

            CommonModel.getAllGroup().then(function () {
                vm.groupOptions = CommonModel.groups;
            });

            EmployeeModel.getDefaultPassword().then(function () {
                vm.defaultPassword = EmployeeModel.defaultPassword;
            });

            EmployeeModel.getUserPermissionOnEmployee().then(function () {
                vm.permission = EmployeeModel.permission;
            });

            getEmployee(vm.filter.query, vm.filter.groupId, vm.filter.isLocal, vm.filter.isSpecialUser, vm.filter.isDisabledUser, vm.filter.hasMonthlyEPA);
        }

        function queryEmployee(filter) {
            vm.searchKey = '';
            vm.currentPage = 1;
            getEmployee(filter.query, filter.groupId, filter.isLocal, filter.isSpecialUser, filter.isDisabledUser, filter.hasMonthlyEPA);
        }

        function resetPassword(employee) {
            EmployeeModel.openPopupResetPassword(employee, vm.defaultPassword);
        }

        function updateEmployee(employee) {
            EmployeeModel.openPopupUpdateEmployee(vm, employee).then(function (updatedEmployee) {
                EmployeeModel.updateChangedEmployee(employee, updatedEmployee);
            })['finally'](function () {
                vm.modalInstance = '';
                getEmployee(vm.filter.query, vm.filter.groupId, vm.filter.isLocal, vm.filter.isSpecialUser, vm.filter.isDisabledUser, vm.filter.hasMonthlyEPA);
            });
        }

        function updateHasMonthlyEPA(employee) {
            EmployeeModel.openPopupUpdateHasMonthlyEPA(vm, employee);
        }
    }
})(angular);