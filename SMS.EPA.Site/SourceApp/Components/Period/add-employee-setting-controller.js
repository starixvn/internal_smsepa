(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('periodModule').controller('AddPeriodEmployeeSettingController', addPeriodEmployeeSettingController);
    addPeriodEmployeeSettingController.$inject = ['$uibModalInstance', 'PeriodModel', 'CommonModel', 'periodId', 'orgChart'];

    function addPeriodEmployeeSettingController($uibModalInstance, PeriodModel, CommonModel, periodId, orgChart) {
        let vm = this;

        vm.searchEmployee = '';
        vm.selectedEmployee = null;
        vm.orgChart = orgChart;
        vm.teamIds = '';

        vm.getEmployee = function () {
            return CommonModel.searchEmployee(vm.searchEmployee, vm.teamIds);
        };

        vm.unSelectEmployee = function () {
            if (vm.searchEmployee.trim() === '') {
                if (vm.selectedEmployee) {
                    vm.selectedEmployee = null;
                }
            }
        };

        vm.onSelectTeam = function (teamIds) {
            vm.teamIds = teamIds;
        };

        vm.save = function () {
            const params = {
                periodId,
                employeeId: vm.selectedEmployee ? vm.selectedEmployee.EmployeeId : null,
                teamIds: vm.teamIds
            };
            const isValid = PeriodModel.validateAddEmployeeSetting(params);
            if (isValid) {
                PeriodModel.addEmployeeSetting(params, $uibModalInstance);
            }
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
})(angular);