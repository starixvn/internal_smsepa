(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('employeeModule').controller('UpdateEmployeeController', updateEmployeeController);
    updateEmployeeController.$inject = ['$modalInstance', 'EmployeeModel', 'CommonModel', 'employee'];

    function updateEmployeeController($modalInstance, EmployeeModel, CommonModel, employee) {
        var vm = this;

        vm.jobTitleOptions = [];
        vm.groupOptions = [];
        vm.originalEmployee = employee;

        vm.employee = {
            EmployeeId: employee.EmployeeId,
            NickName: employee.NickName,
            FullName: employee.FullName,
            GroupId: employee.Group ? employee.Group.GroupId : '',
            JobTitleId: employee.JobTitle ? employee.JobTitle.JobTitleId : '',
            Username: employee.Username,
            Enabled: employee.Enabled,
            IsLocal: employee.IsLocal ? 'true' : 'false',
            Email: employee.Email
        };

        vm.cancel = cancel;
        vm.save = save;

        initialize();

        function cancel() {
            $modalInstance.dismiss('cancel');
        }

        function initialize() {
            CommonModel.getAllGroup().then(function () {
                vm.groupOptions = CommonModel.groups;
            });

            CommonModel.getAllJobTitle().then(function () {
                vm.jobTitleOptions = CommonModel.jobTitles;
            });
        }

        function save(updatedEmployee) {
            if (EmployeeModel.validateEmployee(updatedEmployee, 'update')) {
                EmployeeModel.updateEmployee(updatedEmployee, $modalInstance);
            }
        }
    }
})(angular);