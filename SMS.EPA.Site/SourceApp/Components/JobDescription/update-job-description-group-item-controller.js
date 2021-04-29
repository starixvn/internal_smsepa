(function () {
    'use strict';

    angular.module('jobDescriptionModule').controller('UpdateDescriptionGroupItemController', updateDescriptionGroupItemController);
    updateDescriptionGroupItemController.$inject = ['$uibModalInstance', 'JobDescriptionModel', 'getEmployeeSettingInfo'];

    function updateDescriptionGroupItemController($uibModalInstance, JobDescriptionModel, getEmployeeSettingInfo) {
        var vm = this;

        vm.groupItem = {
            Name: getEmployeeSettingInfo.Name,
            IsActive: getEmployeeSettingInfo.IsActive,
            GroupItemId : getEmployeeSettingInfo.GroupItemId
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