(function () {
    'use strict';

    angular.module('jobDescriptionModule').controller('CreateDescriptionGroupItemController', createDescriptionGroupItemController);
    createDescriptionGroupItemController.$inject = ['$uibModalInstance', 'JobDescriptionModel'];

    function createDescriptionGroupItemController($uibModalInstance, JobDescriptionModel) {
        var vm = this;

        vm.groupItem = {
            Name: '',
            IsActive: true,
            EmployeeId: JobDescriptionModel.employeeSettingInfo.EmployeeId
        };
        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function save() {
            if (JobDescriptionModel.validateDescription(vm.groupItem.Name)) {
                JobDescriptionModel.createDescriptionGroupItem(vm.groupItem, $uibModalInstance);
            }
        }
    }
})(angular);