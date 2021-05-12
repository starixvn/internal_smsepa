(function (angular) {
    'use strict';

    angular.module('jobDescriptionModule').controller('EditDescriptionGroupItemController', editDescriptionGroupItemController);
    editDescriptionGroupItemController.$inject = ['$uibModalInstance', 'JobDescriptionModel', 'descriptionGroupItem'];

    function editDescriptionGroupItemController($uibModalInstance, JobDescriptionModel, descriptionGroupItem) {
        var vm = this;

        vm.groupItem = descriptionGroupItem;
        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function save() {
            if (JobDescriptionModel.validateDescription(vm.groupItem.Name)) {
                JobDescriptionModel.editDescriptionGroupItem(vm.groupItem, $uibModalInstance);
            }
        }
    }
})(angular);