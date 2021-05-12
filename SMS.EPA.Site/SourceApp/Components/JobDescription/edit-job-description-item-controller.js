(function (angular) {
    'use strict';

    angular.module('jobDescriptionModule').controller('EditDescriptionItemController', editDescriptionItemController);
    editDescriptionItemController.$inject = ['$uibModalInstance', 'JobDescriptionModel', 'descriptionItem'];

    function editDescriptionItemController($uibModalInstance, JobDescriptionModel, descriptionItem) {
        var vm = this;

        vm.item = descriptionItem;
        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function save() {
            if (JobDescriptionModel.validateDescription(vm.item.Name)) {
                JobDescriptionModel.editDescriptionItem(vm.item, $uibModalInstance);
            }
        }
    }
})(angular);