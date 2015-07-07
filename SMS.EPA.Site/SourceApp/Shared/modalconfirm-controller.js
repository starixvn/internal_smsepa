/*global appModule*/
'use strict';

appModule.controller("ModalConfirmController", ['$modalInstance', 'name', 'action',
 function ($modalInstance, name, action) {
    var vm = this;

    vm.name = name;
    vm.action = action;

    vm.ok = function () {
        $modalInstance.close();
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
 }]);