(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('employeeModule').controller('EmployeeController', employeeController);
    employeeController.$inject = ['$scope', 'EmployeeModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function employeeController($scope, EmployeeModel, CommonModel, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.defaultPassword = EmployeeModel.defaultPassword;
        vm.employees = EmployeeModel.employees;
        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.orgChart = [];
        vm.permission = EmployeeModel.permission;
        vm.policyEnum = utilitiesFactory.policy;
        utilitiesFactory.numberEntryList(vm);

        function getEmployee(query, teamIds, isLocal, isSpecialUser, isDisabledUser, hasMonthlyEPA) {
            lockScreenService.start();
            if (!hasMonthlyEPA) {
                hasMonthlyEPA = '';
            }

            EmployeeModel.getEmployee(query, teamIds, isLocal, isSpecialUser, isDisabledUser, hasMonthlyEPA).then(function () {
                vm.employees = EmployeeModel.employees;
                utilitiesFactory.searchOnPage(vm, vm.employees);
                lockScreenService.stop();
            });
        }

        function initFilter() {
            vm.filter = {
                query: '',
                teamIds: '',
                isLocal: '',
                isSpecialUser: '',
                isDisabledUser: '',
                hasMonthlyEPA: ''
            };
        }

        function initialize() {
            initFilter();

            CommonModel.getOrgChart().then(function () {
                vm.orgChart = CommonModel.orgChart;
            });

            EmployeeModel.getDefaultPassword().then(function () {
                vm.defaultPassword = EmployeeModel.defaultPassword;
            });

            EmployeeModel.getUserPermissionOnEmployee().then(function () {
                vm.permission = EmployeeModel.permission;
            });

            getEmployee(vm.filter.query, vm.filter.teamIds, vm.filter.isLocal, vm.filter.isSpecialUser, vm.filter.isDisabledUser, vm.filter.hasMonthlyEPA);
        }

        vm.onSelectTeam = function (teamIds) {
            vm.filter.teamIds = teamIds;
        };

        vm.query = function () {
            vm.searchKey = '';
            vm.currentPage = 1;
            getEmployee(vm.filter.query, vm.filter.teamIds, vm.filter.isLocal, vm.filter.isSpecialUser, vm.filter.isDisabledUser, vm.filter.hasMonthlyEPA);
        };

        vm.assignRole = function (employee) {
            EmployeeModel.openPopupAssignRole(employee);
        };

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.create = function () {
            EmployeeModel.openPopupCreateNewEmployee(vm).then(function () {
                getEmployee(vm.filter.query, vm.filter.teamIds, vm.filter.isLocal, vm.filter.isSpecialUser, vm.filter.isDisabledUser, vm.filter.hasMonthlyEPA);
            })['finally'](function () {
                vm.modalInstance = '';
            });
        };

        vm.exportReport = function (filter) {
            vm.exportCriteria = filter;

            $scope.$broadcast('exportEmployeeInfoReport');
        };

        vm.resetPassword = function (employee) {
            EmployeeModel.openPopupResetPassword(employee, vm.defaultPassword);
        };

        vm.edit = function (employee) {
            EmployeeModel.openPopupUpdateEmployee(vm, employee).then(function (updatedEmployee) {
                EmployeeModel.updateChangedEmployee(employee, updatedEmployee);
                getEmployee(vm.filter.query, vm.filter.teamIds, vm.filter.isLocal, vm.filter.isSpecialUser, vm.filter.isDisabledUser, vm.filter.hasMonthlyEPA);
            })['finally'](function () {
                vm.modalInstance = '';
            });
        };

        vm.editHasMonthlyEPA = function (employee) {
            EmployeeModel.openPopupUpdateHasMonthlyEPA(vm, employee);
        };

        vm.editJobLevel = function (employee) {
            EmployeeModel.openPopupEditJobLevel(vm, employee).then(function () {
                getEmployee(vm.filter.query, vm.filter.teamIds, vm.filter.isLocal, vm.filter.isSpecialUser, vm.filter.isDisabledUser, vm.filter.hasMonthlyEPA);
            })['finally'](function () {
                vm.modalInstance = '';
            });
        };

        initialize();
    }
})(angular);