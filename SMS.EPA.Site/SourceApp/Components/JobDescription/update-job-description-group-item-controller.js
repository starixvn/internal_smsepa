(function () {
    'use strict';

    angular.module('jobDescriptionModule').controller('UpdateDescriptionGroupItemController', updateDescriptionGroupItemController);
    updateDescriptionGroupItemController.$inject = ['$uibModalInstance', 'JobDescriptionModel', 'getGroupItem'];

    function updateDescriptionGroupItemController($uibModalInstance, JobDescriptionModel, getGroupItem) {
        var vm = this;

        vm.groupItem = {
            Name: getGroupItem.Name,
            IsActive: getGroupItem.IsActive,
            GroupItemId: getGroupItem.GroupItemId
        };
        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function save() {
            if (JobDescriptionModel.validateDescription(vm.groupItem.Name)) {
                JobDescriptionModel.updateDescriptionGroupItem(vm.groupItem, $uibModalInstance);
            }
        }
    }
})(angular);