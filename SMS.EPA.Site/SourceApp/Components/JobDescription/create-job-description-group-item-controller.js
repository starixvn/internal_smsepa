(function (angular) {
    'use strict';

    angular.module('jobDescriptionModule').controller('CreateDescriptionGroupItemController', createDescriptionGroupItemController);
    createDescriptionGroupItemController.$inject = ['$uibModalInstance', 'JobDescriptionModel', 'groupId', 'employeeId'];

    function createDescriptionGroupItemController($uibModalInstance, JobDescriptionModel, groupId, employeeId) {
        var vm = this;

        vm.groupItem = {
            name: '',
            groupId: groupId,
            employeeId: employeeId
        };
        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function save() {
            if (JobDescriptionModel.validateDescription(vm.groupItem.name)) {
                JobDescriptionModel.createDescriptionGroupItem(vm.groupItem, $uibModalInstance);
            }
        }
    }
})(angular);