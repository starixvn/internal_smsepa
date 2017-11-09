(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('kpiGroupModule').controller('CreateKPIGroupController', createKPIGroupController);
    createKPIGroupController.$inject = ['$uibModalInstance', 'KPIGroupModel'];

    function createKPIGroupController($uibModalInstance, KPIGroupModel) {
        var vm = this;

        vm.kpiGroup = {
            Name: '',
            IsActive: true,
            Description: ''
        };

        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function save() {
            if (KPIGroupModel.validateKPIGroup(vm.kpiGroup)) {
                KPIGroupModel.createKPIGroup(vm.kpiGroup, $uibModalInstance);
            }
        }
    }
})(angular);