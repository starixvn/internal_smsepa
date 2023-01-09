(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('epaModule').controller('UpdateFinalScoreController', updateFinalScoreController);
    updateFinalScoreController.$inject = ['$scope', 'UpdateFinalScoreModel', 'CommonModel', 'notificationFactory', 'utilitiesFactory', 'lockScreenService'];

    function updateFinalScoreController($scope, UpdateFinalScoreModel, CommonModel, notificationFactory, utilitiesFactory, lockScreenService) {
        let vm = this;

        vm.currentPage = 1;
        vm.dataSaved = false;
        vm.groups = [];
        vm.employees = [];
        vm.epaInfos = [];
        vm.filter = {
            periodId: '',
            teamIds: null,
            isLocal: '',
            employee: null
        };
        vm.orgChart = [];

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
            const index = vm.periods.map(function (e) { return e.PeriodId; }).indexOf(vm.filter.periodId);
            const selectedEPAs = getSelectedEPA(vm.filteredItems);
            const employeeIds = selectedEPAs.map(function (e) { return e.Employee.EmployeeId; }).join(',');

            vm.exportCriteria = {
                periodId: vm.filter.periodId,
                periodName: index !== -1 ? vm.periods[index].Name : '',
                teamIds: vm.filter.teamIds,
                isLocal: vm.filter.isLocal,
                employeeId: vm.filter.employee ? vm.filter.employee.EmployeeId : null,
                employeeIds
            };

            $scope.$broadcast('exportEpaAfterUpdateFinalScore');
        }

        function getEmployee() {
            if (vm.filter.periodId > 0) {
                return UpdateFinalScoreModel.getEmployeeToUpdateFinalScore(vm.filter.periodId, vm.filter.teamIds, vm.filter.isLocal, vm.searchKeyword);
            }
        }

        function getEPAToUpdateFinalScore(filter) {
            if (vm.filter.periodId > 0) {
                vm.dataSaved = false;
                vm.searchKey = '';
                vm.currentPage = 1;
                vm.selectedAll = false;
                lockScreenService.start();
                const employeeId = filter.employee ? filter.employee.EmployeeId : null;

                UpdateFinalScoreModel.getEPAToUpdateFinalScore(filter.periodId, filter.teamIds, filter.isLocal, employeeId).then(function () {
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
            const employeeIds = [];

            angular.forEach(epaInfos, function (epaInfo) {
                if (epaInfo.Selected) {
                    employeeIds.push(epaInfo.Employee.EmployeeId);
                }
            });

            return employeeIds;
        }

        function initialize() {
            UpdateFinalScoreModel.getPeriodToUpdateFinalScore().then(function () {
                vm.periods = UpdateFinalScoreModel.periods;
            });

            CommonModel.getOrgChart().then(function () {
                vm.orgChart = CommonModel.orgChart;
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
            const employeeIds = getSelectedEPA(vm.filteredItems);

            if (!employeeIds || employeeIds.length === 0) {
                notificationFactory.error('SelectEPABeforeEditSettingWarning');
            } else {
                UpdateFinalScoreModel.updateFinalScore(vm, employeeIds.join(','));
            }
        }

        function unSelectEmployee() {
            if (vm.searchKeyword.trim() === '') {
                vm.filter.employee = null;
            }
        }

        vm.onSelectTeam = function (teamIds) {
            vm.filter.teamIds = teamIds;
        };
    }
})(angular);