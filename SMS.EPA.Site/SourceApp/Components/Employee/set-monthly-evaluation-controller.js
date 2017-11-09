(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('employeeModule').controller('ModalConfirmSetUnsetMonthlyEvaluationController', modalConfirmSetUnsetMonthlyEvaluationController);
    modalConfirmSetUnsetMonthlyEvaluationController.$inject = ['$uibModalInstance', 'name', 'action'];

    function modalConfirmSetUnsetMonthlyEvaluationController($uibModalInstance, name, action) {
        var vm = this;

        vm.action = action;
        vm.name = name;

        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function save() {
            $uibModalInstance.close();
        }
    }
})(angular);