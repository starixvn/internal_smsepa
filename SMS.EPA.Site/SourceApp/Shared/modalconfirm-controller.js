(function (angular) {
    'use strict';

    angular.module('appModule').controller('ModalConfirmController', modalConfirmController);
    modalConfirmController.$inject = ['$uibModalInstance', 'name', 'action'];

    function modalConfirmController($uibModalInstance, name, action) {
        let vm = this;

        vm.name = name;
        vm.action = action;

        vm.ok = function () {
            $uibModalInstance.close();
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }

    angular.module('appModule').controller('ConfirmRefreshDataController', confirmRefreshDataController);
    confirmRefreshDataController.$inject = ['$uibModalInstance'];

    function confirmRefreshDataController($uibModalInstance) {
        let vm = this;

        vm.ok = function () {
            $uibModalInstance.close();
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
})(angular);