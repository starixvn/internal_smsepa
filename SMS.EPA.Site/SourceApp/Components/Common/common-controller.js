'use strict';
/*global commonModule*/

commonModule.controller('OtherViolationDetailController',
['$modalInstance', 'CommonModel', 'epaId',
function ($modalInstance, CommonModel, epaId) {
    var vm = this;

    vm.otherViolationDetails = [];

    CommonModel.getOtherViolationDetailByEPAId(epaId).then(function () {
        vm.otherViolationDetails = CommonModel.otherViolationDetails;
    });

    vm.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}]);

commonModule.controller('MenuController',
['CommonModel',
function (CommonModel) {
    var vm = this;

    vm.menus = [];

    CommonModel.getMenusByEmployeeId().then(function () {
        vm.menus = CommonModel.menus;
    });
}]);