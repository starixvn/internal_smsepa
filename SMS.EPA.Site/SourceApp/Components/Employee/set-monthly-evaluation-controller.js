(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('employeeModule').controller('ModalConfirmSetUnsetMonthlyEvaluationController', modalConfirmSetUnsetMonthlyEvaluationController);
    modalConfirmSetUnsetMonthlyEvaluationController.$inject = ['$modalInstance', 'name', 'action'];

    function modalConfirmSetUnsetMonthlyEvaluationController($modalInstance, name, action) {
        var vm = this;

        vm.action = action;
        vm.name = name;

        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $modalInstance.dismiss('cancel');
        }

        function save() {
            $modalInstance.close();
        }
    }
})(angular);