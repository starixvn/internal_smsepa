'use strict';

(function (angular) {
    function UpdateFinalScoreController(EmployeeEPAModel, CommonModel) {
        var vm = this;

        vm.departments = [];
        vm.employees = [];
        vm.epaInfos = [];
        vm.filter = {
            periodId: '',
            groupId: '',
            employee: null
        };
        vm.periods = [];

        function getEmployee() {
            if (vm.filter.periodId > 0) {
                return EmployeeEPAModel.getEmployeeToUpdateFinalScore(vm.filter.periodId, vm.filter.groupId, vm.searchEmployee);
            }
        }

        function getEPAToUpdateFinalScore(filter) {
            if (vm.filter.periodId > 0) {
                var employeeId = filter.employee ? filter.employee.EmployeeId : null;

                EmployeeEPAModel.getEPAToUpdateFinalScore(filter.periodId, filter.groupId, employeeId)
                    .then(function () {
                        vm.epaInfos = angular.copy(EmployeeEPAModel.epaInfos);
                    });
            }
        }

        function initialize() {
            EmployeeEPAModel.getPeriodToUpdateFinalScore().then(function () {
                vm.periods = EmployeeEPAModel.periods;
            });

            CommonModel.getDepartment().then(function () {
                vm.departments = CommonModel.departments;
            });
        }

        function updateFinalScore() {
            EmployeeEPAModel.updateFinalScore(vm);
        }

        function unSelectEmployee() {
            if (vm.searchEmployee.trim() === '') {
                vm.filter.employee = null;
            }
        }

        vm.getEmployee = getEmployee;
        vm.getEPAToUpdateFinalScore = getEPAToUpdateFinalScore;
        vm.initialize = initialize;
        vm.updateFinalScore = updateFinalScore;
        vm.unSelectEmployee = unSelectEmployee;

        vm.initialize();
    }

    UpdateFinalScoreController.$inject = ['EmployeeEPAModel', 'CommonModel'];

    angular
        .module('epaModule')
        .controller('UpdateFinalScoreController', UpdateFinalScoreController);
})(angular);