(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('kpiGroupModule').controller('UpdateKPIGroupController', updateKPIGroupController);
    updateKPIGroupController.$inject = ['$uibModalInstance', 'KPIGroupModel', 'kpiGroup'];

    function updateKPIGroupController($uibModalInstance, KPIGroupModel, kpiGroup) {
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
            $uibModalInstance.dismiss('cancel');
        }

        function save() {
            if (KPIGroupModel.validateKPIGroup(vm.kpiGroup)) {
                KPIGroupModel.updateKPIGroup(vm.kpiGroup, $uibModalInstance);
            }
        }
    }
})(angular);