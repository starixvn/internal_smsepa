(function () {
    'use strict';

    angular.module('jobDescriptionModule').controller('CreateDescriptionItemController', createDescriptionItemController);
    createDescriptionItemController.$inject = ['$uibModalInstance','JobDescriptionModel'];

    function createDescriptionItemController($uibModalInstance, JobDescriptionModel) {
        var vm = this;

        vm.item = {
            Name: '',
            IsActive: true
        };
        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function save() {
            if (JobDescriptionModel.validateDescription(vm.item.Name)) {
                JobDescriptionModel.createDescriptionGroupItem(vm.groupItem, $uibModalInstance);
            }
        }
    }
})(angular);