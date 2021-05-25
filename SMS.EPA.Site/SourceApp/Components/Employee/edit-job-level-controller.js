(function (angular) {
    'use strict';

    angular.module('employeeModule').controller('EditJobLevelEmployeeController', editJobLevelEmployeeController);
    editJobLevelEmployeeController.$inject = ['$uibModalInstance', 'EmployeeModel', 'CommonModel', 'employee'];

    function editJobLevelEmployeeController($uibModalInstance, EmployeeModel, CommonModel, employee) {
        var vm = this;

        vm.jobLevelOptions = [];

        vm.employee = {
            EmployeeId: employee.EmployeeId,
            JobLevelId: employee.JobLevel ? employee.JobLevel.JobLevelId : ''
        };

        vm.cancel = cancel;
        vm.save = save;

        initialize();

        function initialize() {
            CommonModel.getJobLevel(false).then(function () {
                vm.jobLevelOptions = CommonModel.jobLevels;
            });
        }

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function save() {
            EmployeeModel.updateJobLevel(vm.employee, $uibModalInstance);
        }
    }
})(angular);