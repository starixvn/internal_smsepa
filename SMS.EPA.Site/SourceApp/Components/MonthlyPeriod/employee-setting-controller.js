(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('monthlyPeriodModule').controller('EmployeeSettingMonthlyPeriodController', employeeSettingMonthlyPeriodController);
    employeeSettingMonthlyPeriodController.$inject = ['$scope', '$routeParams', 'MonthlyPeriodModel', 'CommonModel',
        'utilitiesFactory', 'notificationFactory', 'lockScreenService'];

    function employeeSettingMonthlyPeriodController($scope, $routeParams, PeriodModel, CommonModel, utilitiesFactory, notificationFactory, lockScreenService) {
        let vm = this;

        vm.period = null;
        vm.periodId = null;
        vm.employeeSettings = PeriodModel.employeeSettings;
        vm.isFirstLoad = true;
        vm.selectedAll = false;
        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.epaStatuses = [
            { Value: 0, Name: 'In Progress' },
            { Value: 1, Name: 'Closed' }
        ];
        vm.orgChart = [];
        utilitiesFactory.numberEntryList(vm);
        utilitiesFactory.getDateFormat(vm);

        vm.filter = {
            periodId: null,
            teamIds: null,
            isLocal: '',
            isExcludedEmployee: null,
            isGeneratedEPA: '',
            isEvaluated: '',
            epaStatus: $routeParams.inprogress !== undefined ? ($routeParams.inprogress === true || $routeParams.inprogress === 'true' ? 0 : '') : ''
        };

        function getEmployeeSetting(filter) {
            lockScreenService.start();

            PeriodModel.getEmployeeSettingById(
                filter.periodId,
                filter.teamIds,
                filter.isLocal,
                filter.isExcludedEmployee,
                filter.isGeneratedEPA,
                filter.isEvaluated,
                filter.epaStatus)
                .then(function () {
                    vm.employeeSettings = PeriodModel.employeeSettings;
                    utilitiesFactory.searchOnPage(vm, vm.employeeSettings);
                    vm.isFirstLoad = false;
                    vm.selectedAll = false;
                    lockScreenService.stop();
                });
        }

        $scope.$watch(function () {
            return $scope.updateCtrl.period && $scope.updateCtrl.permission && $scope.updateCtrl.dataTab === 3;
        }, function (newVal) {
            if (newVal && vm.isFirstLoad) {
                vm.period = $scope.updateCtrl.period;
                vm.filter.periodId = vm.period.PeriodId;
                vm.permission = $scope.updateCtrl.permission;

                CommonModel.getOrgChart().then(function () {
                    vm.orgChart = CommonModel.orgChart;
                });

                getEmployeeSetting(vm.filter);
            }
        });

        vm.query = function (filter) {
            getEmployeeSetting(filter);
        };

        vm.addEmployee = function () {
            PeriodModel.openPopupAddEmployeeSetting(vm).then(function () {
                vm.modalInstance = null;
                getEmployeeSetting(vm.filter);
            });
        };

        vm.updateEmployeeSetting = function (employeeSetting) {
            PeriodModel.openPopupUpdateEmployeeSetting(vm, new Array(employeeSetting));
        };

        vm.manageSetting = function () {
            var selectedEmployees = PeriodModel.getSelectedEmployee(vm.employeeSettings);

            if (!selectedEmployees || selectedEmployees.length === 0) {
                notificationFactory.error("SelectEmployeeBeforeEditSettingWarning");
            } else {
                PeriodModel.openPopupUpdateEmployeeSetting(vm, selectedEmployees);
            }
        };

        vm.includeExcludeEmployee = function (employee) {
            PeriodModel.openPopupIncludeExcludeEmployee(vm, vm.period.PeriodId, employee);
        };

        vm.generateEvaluation = function () {
            var selectedEmployees = PeriodModel.getSelectedEmployee(vm.employeeSettings),
                selectedEmployeeInfo = PeriodModel.getSelectedEmployeeInfo(selectedEmployees),
                period = {
                    periodId: vm.period.PeriodId,
                    employeeIds: selectedEmployeeInfo.selectedEmployeeIds
                };

            if (!selectedEmployees || selectedEmployees.length === 0) {
                notificationFactory.error("SelectEmployeeBeforeGenerateEvaluation");
            } else {
                if (PeriodModel.validateEmployeeSettingGenerateEPA(selectedEmployees)) {
                    PeriodModel.openPopupGenerateEvaluation(vm, period).then(function () {
                        vm.query(vm.filter);
                    });
                }
            }
        };

        vm.checkAll = function (selectedAll) {
            angular.forEach(vm.filteredItems, function (employeeSetting) {
                employeeSetting.Selected = selectedAll;
            });

            angular.forEach(vm.employeeSettings, function (employeeSetting) {
                employeeSetting.Selected = selectedAll;
            });
        };

        vm.syncSelectedEmployee = function (employeeId, selected) {
            var allUnchecked = true,
                allChecked = true;

            angular.forEach(vm.employeeSettings, function (employeeSetting) {
                if (employeeSetting.Employee.EmployeeId === employeeId) {
                    angular.forEach(vm.filteredItems, function (filterItem) {
                        if (employeeSetting.Employee.EmployeeId === filterItem.Employee.EmployeeId) {
                            filterItem.Selected = selected;
                            employeeSetting.Selected = selected;
                        }
                    });
                }
            });

            angular.forEach(vm.filteredItems, function (filterItem) {
                if (filterItem.Selected) {
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
        };

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.onSelectTeam = function (teamIds) {
            vm.filter.teamIds = teamIds;
        };
    }
})(angular);