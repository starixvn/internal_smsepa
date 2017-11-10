(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('periodModule').controller('EmployeeSettingPeriodController', employeeSettingPeriodController);
    employeeSettingPeriodController.$inject = ['$scope', '$routeParams', '$window', 'PeriodModel', 'CommonModel',
        'utilitiesFactory', 'notificationFactory', 'lockScreenService'];

    function employeeSettingPeriodController($scope, $routeParams, $window, PeriodModel, CommonModel,
        utilitiesFactory, notificationFactory, lockScreenService) {
        var vm = this;

        vm.period = null;
        vm.periodId = null;
        vm.employeeSettings = [];
        vm.isFirstLoad = true;
        vm.departmentOptions = [];

        vm.currentPage = 1;
        vm.pageSize = 15;
        utilitiesFactory.numberEntryList(vm);
        utilitiesFactory.getDateFormat(vm);
        vm.selectedAll = false;
        vm.targetTypeEnum = utilitiesFactory.targetType;

        vm.filter = {
            periodId: null,
            departmentId: null,
            isLocal: '',
            isExcludedEmployee: null,
            isGeneratedEPA: '',
            isEvaluated: '',
            hasMonthlyEPA: '',
            epaStatus: $routeParams.inprogress === true ? 0 : ''
        };

        vm.filterAssign = {
            employee: ''
        };

        var getPeriodEmployeeSetting = function (filter) {
            lockScreenService.start();

            PeriodModel.getPeriodEmployeeSettingById(
                filter.periodId,
                filter.departmentId,
                filter.isLocal,
                filter.isExcludedEmployee,
                filter.isGeneratedEPA,
                filter.isEvaluated,
                filter.hasMonthlyEPA,
                filter.epaStatus)
            .then(function () {
                vm.employeeSettings = PeriodModel.employeeSettings;
                utilitiesFactory.searchOnPage(vm, vm.employeeSettings);
                vm.isFirstLoad = false;
                vm.selectedAll = false;
                lockScreenService.stop();
            });
        };

        $scope.$watch(function () {
            return $scope.updateCtrl.dataTab === 3 && $scope.updateCtrl.period && $scope.updateCtrl.permission;
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

                getPeriodEmployeeSetting(vm.filter);
            }
        });

        vm.query = function (filter) {
            getPeriodEmployeeSetting(filter);
        };

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
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

        vm.addEmployee = function (filterAssign) {
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
                PeriodModel.addEmployeeToPeriod(employeeInfo).then(function () {
                    resetFilterAssign();
                    getPeriodEmployeeSetting(vm.filter);
                });
            } else {
                notificationFactory.error("SelectDepartmentSectionEmployeeBeforeAddToPeriodWarning");
            }
        };

        vm.updateEmployeeSetting = function (employeeSetting) {
            PeriodModel.openPopupUpdateEmployeeSetting(vm, [employeeSetting]);
        };

        vm.includeExcludeEmployee = function (employee) {
            PeriodModel.openPopupIncludeExcludeEmployee(vm, vm.period.PeriodId, employee);
        };

        function checkEditSettingOnDifferentType(selectedEmployees) {
            var isValid = true;

            if (!selectedEmployees || selectedEmployees.length > 0) {
                var hasMonthlyEPA = selectedEmployees[0].Employee.HasMonthlyEPA;

                for (var i = 1; i < selectedEmployees.length; i++) {
                    var employee = selectedEmployees[i].Employee;

                    if (employee.HasMonthlyEPA !== hasMonthlyEPA) {
                        isValid = false;

                        break;
                    }
                }
            }

            return isValid;
        }

        vm.manageSetting = function () {
            var selectedEmployees = PeriodModel.getSelectedEmployee(vm.employeeSettings);

            if (!selectedEmployees || selectedEmployees.length === 0) {
                notificationFactory.error("SelectEmployeeBeforeEditSettingWarning");
            } else if (!checkEditSettingOnDifferentType(selectedEmployees)) {
                notificationFactory.error("CanNotEditSettingOnDifferentTypeWarning");
            } else {
                PeriodModel.openPopupUpdateEmployeeSetting(vm, selectedEmployees);
            }
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
            } else if (PeriodModel.validateEmployeeSettingGenerateEPA(selectedEmployees)) {
                PeriodModel.openPopupGenerateEvaluation(vm, period).then(function () {
                    vm.query(vm.filter);
                });
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
                }
                else {
                    allChecked = false;
                }
            });

            if (allUnchecked) {
                vm.selectedAll = false;
            }

            if (allChecked) {
                vm.selectedAll = true;
            }
            else {
                vm.selectedAll = false;
            }
        };

        vm.exportReport = function (filter) {
            vm.exportCriteria = {
                periodId: filter.periodId,
                periodName: vm.period.Name,
                departmentId: filter.departmentId,
                isLocal: filter.isLocal,
                isExcludedEmployee: filter.isExcludedEmployee
            };

            $scope.$broadcast('exportEmployeeFlowSettingReport');
        };
    }
})(angular);