(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('employeeModule').controller('CreateEmployeeController', createEmployeeController);
    createEmployeeController.$inject = ['$uibModalInstance', 'EmployeeModel', 'CommonModel'];

    function createEmployeeController($uibModalInstance, EmployeeModel, CommonModel) {
        var vm = this;

        vm.jobTitleOptions = [];
        vm.groupOptions = [];

        vm.employee = {
            NickName: '',
            FullName: '',
            GroupId: '',
            JobTitleId: '',
            Username: '',
            Password: '',
            Enabled: true,
            IsLocal: 'true',
            Email: ''
        };

        vm.cancel = cancel;
        vm.save = save;

        initialize();

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function initialize() {
            CommonModel.getAllGroup().then(function () {
                vm.groupOptions = CommonModel.groups;
            });

            CommonModel.getAllJobTitle().then(function () {
                vm.jobTitleOptions = CommonModel.jobTitles;
            });
        }

        function save(employee) {
            if (EmployeeModel.validateEmployee(employee, 'create')) {
                EmployeeModel.createEmployee(employee, $uibModalInstance);
            }
        }
    }
})(angular);