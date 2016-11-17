(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').controller('ModalConfirmController', modalConfirmController);
    modalConfirmController.$inject = ['$modalInstance', 'name', 'action'];

    function modalConfirmController($modalInstance, name, action) {
        var vm = this;

        vm.name = name;
        vm.action = action;

        vm.ok = function () {
            $modalInstance.close();
        };

        vm.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    }
})(angular);