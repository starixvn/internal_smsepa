(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('appModule').controller('ModalConfirmController', modalConfirmController);
    modalConfirmController.$inject = ['$uibModalInstance', 'name', 'action'];

    function modalConfirmController($uibModalInstance, name, action) {
        var vm = this;

        vm.name = name;
        vm.action = action;

        vm.ok = function () {
            $uibModalInstance.close();
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
})(angular);