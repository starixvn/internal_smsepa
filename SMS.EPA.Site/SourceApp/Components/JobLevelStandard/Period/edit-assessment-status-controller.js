(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('EditAssessmentStatusController', editAssessmentStatusController);
    editAssessmentStatusController.$inject = ['$uibModalInstance', 'JobLevelStandardPeriodModel', 'utilitiesFactory', 'employee'];

    function editAssessmentStatusController($uibModalInstance, PeriodModel, utilitiesFactory, employee) {
        let vm = this;

        vm.employee = employee.FullName + ' (' + employee.Nickname + ')';
        vm.statusId = employee.StatusId;

        vm.assessmentStatusOptions = [
            { StatusId: utilitiesFactory.jobLevelAssessmentStatus.Pending, Name: 'Pending' },
            { StatusId: utilitiesFactory.jobLevelAssessmentStatus.Verified, Name: 'Verified' },
            { StatusId: utilitiesFactory.jobLevelAssessmentStatus.Approved, Name: 'Approved' }
        ];

        vm.save = function () {
            const params = {
                assessmentId: employee.AssessmentId,
                statusId: vm.statusId
            };
            PeriodModel.editAssessmentStatus(params, $uibModalInstance);
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
})(angular);