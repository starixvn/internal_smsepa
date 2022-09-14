(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('employeeModule').controller('CreateEmployeeController', createEmployeeController);
    createEmployeeController.$inject = ['$uibModalInstance', 'EmployeeModel', 'CommonModel'];

    function createEmployeeController($uibModalInstance, EmployeeModel, CommonModel) {
        let vm = this;

        vm.jobTitleOptions = [];
        vm.jobLevelOptions = [];
        vm.orgChart = [];

        vm.employee = {
            NickName: '',
            FullName: '',
            GroupId: '',
            JobTitleId: '',
            Username: '',
            Password: '',
            Enabled: true,
            IsLocal: 'true',
            Email: '',
            JobLevelId: ''
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
            if (EmployeeModel.validateEmployee(vm.employee, 'create')) {
                EmployeeModel.createEmployee(vm.employee, $uibModalInstance);
            }
        };

        initialize();
    }
})(angular);