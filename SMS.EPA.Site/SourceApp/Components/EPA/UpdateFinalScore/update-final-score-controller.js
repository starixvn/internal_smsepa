(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('epaModule').controller('UpdateFinalScoreController', updateFinalScoreController);
    updateFinalScoreController.$inject = ['$scope', 'UpdateFinalScoreModel', 'CommonModel', 'notificationFactory', 'utilitiesFactory', 'lockScreenService'];

    function updateFinalScoreController($scope, UpdateFinalScoreModel, CommonModel, notificationFactory, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.currentPage = 1;
        vm.dataSaved = false;
        vm.groups = [];
        vm.employees = [];
        vm.epaInfos = [];
        vm.filter = {
            periodId: '',
            groupId: '',
            isLocal: '',
            employee: null
        };
        vm.pageSize = 20;
        vm.periods = [];
        vm.selectedAll = false;

        vm.checkAll = checkAll;
        vm.exportExcel = exportExcel;
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

        function exportExcel() {
            var index = vm.periods.map(function (e) { return e.PeriodId; }).indexOf(vm.filter.periodId);
            var selectedEPAs = getSelectedEPA(vm.filteredItems);
            var employeeIds = selectedEPAs.map(function (e) { return e.Employee.EmployeeId; }).join(',');

            vm.exportCriteria = {
                periodId: vm.filter.periodId,
                periodName: index !== -1 ? vm.periods[index].Name : '',
                groupId: vm.filter.groupId,
                isLocal: vm.filter.isLocal,
                employeeId: vm.filter.employee ? vm.filter.employee.EmployeeId : null,
                employeeIds: employeeIds
            };

            $scope.$broadcast('exportEpaAfterUpdateFinalScore');
        }

        function getEmployee() {
            if (vm.filter.periodId > 0) {
                return UpdateFinalScoreModel.getEmployeeToUpdateFinalScore(vm.filter.periodId, vm.filter.groupId, vm.filter.isLocal, vm.searchEmployee);
            }
        }

        function getEPAToUpdateFinalScore(filter) {
            if (vm.filter.periodId > 0) {
                vm.dataSaved = false;
                vm.searchKey = '';
                vm.currentPage = 1;
                vm.selectedAll = false;
                lockScreenService.start();
                var employeeId = filter.employee ? filter.employee.EmployeeId : null;

                UpdateFinalScoreModel.getEPAToUpdateFinalScore(filter.periodId, filter.groupId, filter.isLocal, employeeId).then(function () {
                    vm.epaInfos = UpdateFinalScoreModel.epaInfos;
                    utilitiesFactory.searchOnPage(vm, vm.epaInfos);
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
            UpdateFinalScoreModel.getPeriodToUpdateFinalScore().then(function () {
                vm.periods = UpdateFinalScoreModel.periods;
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
                UpdateFinalScoreModel.updateFinalScore(vm, selectedEPAs);
            }
        }

        function unSelectEmployee() {
            if (vm.searchEmployee.trim() === '') {
                vm.filter.employee = null;
            }
        }
    }
})(angular);