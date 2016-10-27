(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('commonModule').controller('OtherViolationDetailController', otherViolationDetailController);
    otherViolationDetailController.$inject = ['$modalInstance', 'CommonModel', 'epaId'];

    function otherViolationDetailController($modalInstance, CommonModel, epaId) {
        var vm = this;

        vm.otherViolationDetails = [];

        CommonModel.getOtherViolationDetailByEPAId(epaId).then(function () {
            vm.otherViolationDetails = CommonModel.otherViolationDetails;
        });

        vm.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    }
})(angular);