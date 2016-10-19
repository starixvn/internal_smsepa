(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('monthlyPeriodModule').controller('EmployeeSettingMonthlyPeriodController', employeeSettingMonthlyPeriodController);
    employeeSettingMonthlyPeriodController.$inject = ['$scope', '$routeParams', 'MonthlyPeriodModel', 'CommonModel',
        'utilitiesFactory', 'notificationFactory', 'lockScreenService'];

    function employeeSettingMonthlyPeriodController($scope, $routeParams, MonthlyPeriodModel, CommonModel,
        utilitiesFactory, notificationFactory, lockScreenService) {
        var vm = this;

        vm.period = null;
        vm.periodId = null;
        vm.employeeSettings = MonthlyPeriodModel.employeeSettings;
        vm.isFirstLoad = true;
        vm.departmentOptions = [];
        vm.selectedAll = false;
        vm.currentPage = 1;
        vm.pageSize = 20;

        utilitiesFactory.numberEntryList(vm);
        utilitiesFactory.getDateFormat(vm);

        vm.filter = {
            periodId: null,
            departmentId: null,
            isLocal: '',
            isExcludedEmployee: null,
            isGeneratedEPA: '',
            isEvaluated: '',
            epaStatus: $routeParams.inprogress === 'true' ? '0' : ''
        };

        vm.filterAssign = {
            employee: ''
        };

        function getEmployeeSetting(filter) {
            lockScreenService.start();

            MonthlyPeriodModel.getEmployeeSettingMonthlyPeriodById(
                filter.periodId,
                filter.departmentId,
                filter.isLocal,
                filter.isExcludedEmployee,
                filter.isGeneratedEPA,
                filter.isEvaluated,
                filter.epaStatus)
            .then(function () {
                vm.employeeSettings = MonthlyPeriodModel.employeeSettings;
                utilitiesFactory.searchOnPage(vm, vm.employeeSettings);
                vm.isFirstLoad = false;
                vm.selectedAll = false;
                lockScreenService.stop();
            });
        }

        $scope.$watch(function () {
            return $scope.updateCtrl.period && $scope.updateCtrl.permission && $scope.updateCtrl.dataTab.employeeSetting;
        },
        function (newVal) {
            if (newVal && vm.isFirstLoad) {
                vm.period = $scope.updateCtrl.period;
                vm.filter.periodId = vm.period.PeriodId;
                vm.permission = $scope.updateCtrl.permission;

                CommonModel.getDepartment(null, true).then(function () {
                    var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
                    vm.departmentOptions = angular.copy(treeDepartment);
                });

                getEmployeeSetting(vm.filter);
            }
        });

        vm.query = function (filter) {
            getEmployeeSetting(filter);
        };

        vm.getEmployee = function () {
            return CommonModel.getAllEmployee(vm.searchEmployee);
        };

        vm.unSelectEmployee = function () {
            if (vm.searchEmployee.trim() === '') {
                if (vm.filterAssign.employee) {
                    vm.filterAssign.employee = null;
                }
            }
        };

        function getSelectedDepartment(departments) {
            var selectedDepartments = [];

            angular.forEach(departments, function (department) {
                if (department.ticked) {
                    var departmentId = department.DepartmentId,
                        selectedDepartment = {
                            departmentId: departmentId < 0 ? -departmentId : departmentId,
                            isGroup: department.IsGroup
                        };

                    selectedDepartments.push(selectedDepartment);
                }
            });

            return selectedDepartments;
        }

        function resetFilterAssign() {
            vm.filterAssign.employee = '';
            vm.searchEmployee = '';

            angular.forEach(vm.departments, function (department) {
                department.ticked = false;
            });
        }

        vm.addEmployeeToMonthlyPeriod = function (filterAssign) {
            var employeeId = null;

            if (filterAssign.employee) {
                employeeId = filterAssign.employee.EmployeeId;
            }

            var employeeInfo = {
                periodId: vm.period.PeriodId,
                employeeId: employeeId,
                departments: getSelectedDepartment(vm.departmentOptions)
            };

            if (employeeInfo.employeeId || (employeeInfo.departments && employeeInfo.departments.length > 0)) {
                if (filterAssign.employee && !filterAssign.employee.HasMonthlyEPA) {
                    notificationFactory.error("EmployeeNotSetMonthlyEvaluation");
                } else {
                    MonthlyPeriodModel.addEmployeeToMonthlyPeriod(employeeInfo).then(function () {
                        resetFilterAssign();
                        getEmployeeSetting(vm.filter);
                    });
                }
            } else {
                notificationFactory.error("SelectDepartmentSectionEmployeeBeforeAddToPeriodWarning");
            }
        };

        vm.updateEmployeeSetting = function (employeeSetting) {
            MonthlyPeriodModel.openPopupUpdateEmployeeSetting(vm, new Array(employeeSetting));
        };

        vm.manageSetting = function () {
            var selectedEmployees = MonthlyPeriodModel.getSelectedEmployee(vm.employeeSettings);

            if (!selectedEmployees || selectedEmployees.length === 0) {
                notificationFactory.error("SelectEmployeeBeforeEditSettingWarning");
            } else {
                MonthlyPeriodModel.openPopupUpdateEmployeeSetting(vm, selectedEmployees);
            }
        };

        vm.includeExcludeEmployee = function (employee) {
            MonthlyPeriodModel.openPopupIncludeExcludeEmployee(vm, vm.period.PeriodId, employee);
        };

        vm.generateEvaluation = function () {
            var selectedEmployees = MonthlyPeriodModel.getSelectedEmployee(vm.employeeSettings),
                selectedEmployeeInfo = MonthlyPeriodModel.getSelectedEmployeeInfo(selectedEmployees),
                period = {
                    periodId: vm.period.PeriodId,
                    employeeIds: selectedEmployeeInfo.selectedEmployeeIds
                };

            if (!selectedEmployees || selectedEmployees.length === 0) {
                notificationFactory.error("SelectEmployeeBeforeGenerateEvaluation");
            } else {
                if (MonthlyPeriodModel.validateEmployeeSettingGenerateEPA(selectedEmployees)) {
                    MonthlyPeriodModel.openPopupGenerateEvaluation(vm, period).then(function () {
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
    }
})(angular);