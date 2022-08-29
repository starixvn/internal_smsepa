(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('AddEmployeeSettingController', addEmployeeSettingController);
    addEmployeeSettingController.$inject = ['$uibModalInstance', 'JobLevelStandardPeriodModel', 'CommonModel', 'periodId', 'orgChart'];

    function addEmployeeSettingController($uibModalInstance, PeriodModel, CommonModel, periodId, orgChart) {
        let vm = this;

        vm.searchEmployee = '';
        vm.selectedEmployee = null;
        vm.orgChart = orgChart;
        vm.teamIds = '';

        vm.getEmployee = function () {
            return CommonModel.getAllEmployee(vm.searchEmployee);
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