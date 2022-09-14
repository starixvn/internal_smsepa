(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('employeeModule').controller('UpdateEmployeeController', updateEmployeeController);
    updateEmployeeController.$inject = ['$uibModalInstance', 'EmployeeModel', 'CommonModel', 'employee'];

    function updateEmployeeController($uibModalInstance, EmployeeModel, CommonModel, employee) {
        let vm = this;

        vm.jobTitleOptions = [];
        vm.jobLevelOptions = [];
        vm.orgChart = [];

        vm.employee = {
            EmployeeId: employee.EmployeeId,
            NickName: employee.NickName,
            FullName: employee.FullName,
            GroupId: employee.Group ? employee.Group.GroupId.toString() : '',
            JobTitleId: employee.JobTitle ? employee.JobTitle.JobTitleId : '',
            JobLevelId: employee.JobLevel ? employee.JobLevel.JobLevelId : '',
            Username: employee.Username,
            Enabled: employee.Enabled,
            IsLocal: employee.IsLocal ? 'true' : 'false',
            Email: employee.Email
        };

        function initialize() {
            CommonModel.getOrgChart().then(function () {
                vm.orgChart = CommonModel.orgChart;
            });

            CommonModel.getAllJobTitle().then(function () {
                vm.jobTitleOptions = CommonModel.jobTitles;
            });

            CommonModel.getJobLevel(false).then(function () {
                vm.jobLevelOptions = CommonModel.jobLevels;
            });
        }

        vm.onSelectTeam = function (teamId) {
            vm.employee.GroupId = teamId;
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

        vm.save = function () {
            if (EmployeeModel.validateEmployee(vm.employee, 'update')) {
                EmployeeModel.updateEmployee(vm.employee, $uibModalInstance);
            }
        };

        initialize();
    }
})(angular);