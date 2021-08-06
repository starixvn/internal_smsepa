(function (angular) {
    'use strict';

    angular.module('jobDescriptionModule').controller('CopyEmployeeSettingController', copyEmployeeSettingController);
    copyEmployeeSettingController.$inject = ['$uibModalInstance', 'notificationFactory', 'JobDescriptionModel', 'employee'];

    function copyEmployeeSettingController($uibModalInstance, notificationFactory, JobDescriptionModel, employee) {
        let vm = this;

        vm.fromEmployeeId = '';
        vm.employee = {
            ...employee,
            displayName: employee.FullName + (employee.Nickname ? ' (' + employee.Nickname + ')' : '')
        };
        vm.employeeHasSettings = [];
        vm.cancel = cancel;
        vm.save = save;

        initialize();

        function initialize() {
            JobDescriptionModel.getEmployeeHasSetting().then(
                function () {
                    vm.employeeHasSettings = JobDescriptionModel.employeeHasSettings
                        .filter(item => item.EmployeeId !== vm.employee.EmployeeId)
                        .map(item => { return { ...item, displayName: item.FullName + (item.Nickname ? ' (' + item.Nickname + ')' : '') }; });
                });
        }

        function validate() {
            let isValid = true;
            if (vm.fromEmployeeId === '') {
                isValid = false;
                notificationFactory.error('', 'Please select employee you want to copy setting.');
            }

            return isValid;
        }

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function save() {
            if (validate()) {
                const setting = { fromEmployeeId: vm.fromEmployeeId, toEmployeeId: vm.employee.EmployeeId };
                JobDescriptionModel.copyEmployeeSetting(setting, $uibModalInstance);
            }
        }
    }
})(angular);