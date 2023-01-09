(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('UpdateJobLevelController', updateJobLevelController);
    updateJobLevelController.$inject = ['$uibModalInstance', 'JobLevelStandardPeriodModel', 'employee'];

    function updateJobLevelController($uibModalInstance, PeriodModel, employee) {
        let vm = this;
        vm.employee = employee;

        vm.save = function () {
            const params = {
                assessmentId: employee.AssessmentId,
                employeeId: employee.EmployeeId,
            };

            PeriodModel.updateJobLevel(params, $uibModalInstance);
            $uibModalInstance.close();
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
})(angular);