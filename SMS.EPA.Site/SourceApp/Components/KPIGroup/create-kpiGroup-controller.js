(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('kpiGroupModule').controller('CreateKPIGroupController', createKPIGroupController);
    createKPIGroupController.$inject = ['$modalInstance', 'KPIGroupModel'];

    function createKPIGroupController($modalInstance, KPIGroupModel) {
        var vm = this;

        vm.kpiGroup = {
            Name: '',
            IsActive: true,
            Description: ''
        };

        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $modalInstance.dismiss('cancel');
        }

        function save() {
            if (KPIGroupModel.validateKPIGroup(vm.kpiGroup)) {
                KPIGroupModel.createKPIGroup(vm.kpiGroup, $modalInstance);
            }
        }
    }
})(angular);