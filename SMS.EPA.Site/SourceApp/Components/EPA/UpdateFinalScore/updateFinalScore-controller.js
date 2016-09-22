(function (angular) {
    'use strict';

    angular.module('epaModule').controller('UpdateFinalScoreController', updateFinalScoreController);
    updateFinalScoreController.$inject = ['EmployeeEPAModel', 'CommonModel', 'notificationFactory', 'utilitiesFactory', 'lockScreenService'];

    function updateFinalScoreController(EmployeeEPAModel, CommonModel, notificationFactory, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.currentPage = 1;
        vm.groups = [];
        vm.employees = [];
        vm.epaInfos = [];
        vm.filter = {
            periodId: '',
            groupId: '',
            employee: null
        };
        vm.pageSize = 20;
        vm.periods = [];
        vm.selectedAll = false;

        vm.checkAll = checkAll;
        vm.getEmployee = getEmployee;
        vm.getEPAToUpdateFinalScore = getEPAToUpdateFinalScore;
        vm.getIndex = getIndex;
        vm.updateFinalScore = updateFinalScore;
        vm.unSelectEmployee = unSelectEmployee;
        vm.syncSelectedEPA = syncSelectedEPA;

        utilitiesFactory.numberEntryList(vm);
        initialize();

        function checkAll(selectedAll) {
            angular.forEach(vm.epaInfos, function (epaInfo) {
                epaInfo.Selected = selectedAll;
            });
        }

        function getEmployee() {
            if (vm.filter.periodId > 0) {
                return EmployeeEPAModel.getEmployeeToUpdateFinalScore(vm.filter.periodId, vm.filter.groupId, vm.searchEmployee);
            }
        }

        function getEPAToUpdateFinalScore(filter) {
            if (vm.filter.periodId > 0) {
                lockScreenService.start();
                var employeeId = filter.employee ? filter.employee.EmployeeId : null;

                EmployeeEPAModel.getEPAToUpdateFinalScore(filter.periodId, filter.groupId, employeeId).then(function () {
                    vm.searchKey = '';
                    vm.currentPage = 1;
                    vm.epaInfos = EmployeeEPAModel.epaInfos;
                    utilitiesFactory.searchOnPage(vm, vm.epaInfos);
                    vm.selectedAll = false;
                    lockScreenService.stop();
                });
            }
        }

        function getIndex(index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        function getSelectedEPA(epaInfos) {
            var selectedEPAs = [];

            angular.forEach(epaInfos, function (epaInfo) {
                if (epaInfo.Selected) {
                    selectedEPAs.push(epaInfo);
                }
            });

            return selectedEPAs;
        }

        function initialize() {
            EmployeeEPAModel.getPeriodToUpdateFinalScore().then(function () {
                vm.periods = EmployeeEPAModel.periods;
            });

            CommonModel.getAllGroup().then(function () {
                vm.groups = CommonModel.groups;
            });
        }

        function syncSelectedEPA(epaId, selected) {
            var allUnchecked = true,
                allChecked = true;

            angular.forEach(vm.epaInfos, function (epaInfo) {
                if (epaInfo.EPAId === epaId) {
                    epaInfo.Selected = selected;
                }
            });

            angular.forEach(vm.epaInfos, function (epaInfo) {
                if (epaInfo.Selected) {
                    allUnchecked = false;
                } else {
                    allChecked = false;
                }
            });

            if (allUnchecked) {
                vm.selectedAll = false;
            }

            if (allChecked) {
                vm.selectedAll = true;
            } else {
                vm.selectedAll = false;
            }
        }

        function updateFinalScore() {
            var selectedEPAs = getSelectedEPA(vm.filteredItems);

            if (!selectedEPAs || selectedEPAs.length === 0) {
                notificationFactory.error('SelectEPABeforeEditSettingWarning');
            } else {
                EmployeeEPAModel.updateFinalScore(vm, selectedEPAs);
            }
        }

        function unSelectEmployee() {
            if (vm.searchEmployee.trim() === '') {
                vm.filter.employee = null;
            }
        }
    }
})(angular);