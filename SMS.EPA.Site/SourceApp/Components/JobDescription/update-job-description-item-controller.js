(function () {
    'use strict';

    angular.module('jobDescriptionModule').controller('UpdateDescriptionItemController', updateDescriptionItemController);
    updateDescriptionItemController.$inject = ['$uibModalInstance', 'JobDescriptionModel', 'getItem'];

    function updateDescriptionItemController($uibModalInstance, JobDescriptionModel, getItem) {
        var vm = this;

        vm.item = {
            Name: getItem.Name,
            ItemId: getItem.ItemId
        };
        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function save() {
            if (JobDescriptionModel.validateDescription(vm.item.Name)) {
                JobDescriptionModel.updateDescriptionItem(vm.item, $uibModalInstance);
            }
        }
    }
})(angular);