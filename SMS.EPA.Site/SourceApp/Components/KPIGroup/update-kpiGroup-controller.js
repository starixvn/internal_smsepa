(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('kpiGroupModule').controller('UpdateKPIGroupController', updateKPIGroupController);
    updateKPIGroupController.$inject = ['$modalInstance', 'KPIGroupModel', 'kpiGroup'];

    function updateKPIGroupController($modalInstance, KPIGroupModel, kpiGroup) {
        var vm = this;

        vm.kpiGroup = {
            KPIGroupId: kpiGroup.KPIGroupId,
            Name: kpiGroup.Name,
            IsActive: kpiGroup.IsActive,
            Description: kpiGroup.Description
        };

        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $modalInstance.dismiss('cancel');
        }

        function save() {
            if (KPIGroupModel.validateKPIGroup(vm.kpiGroup)) {
                KPIGroupModel.updateKPIGroup(vm.kpiGroup, $modalInstance);
            }
        }
    }
})(angular);