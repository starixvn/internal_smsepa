'use strict';
/*global periodModule*/

periodModule.controller('PeriodController',
['PeriodModel', 'CommonModel', 'utilitiesFactory', 'baseUrl',
function (PeriodModel, CommonModel, utilitiesFactory, baseUrl) {
    var vm = this;

    vm.periods = PeriodModel.periods;
    vm.targetTypes = CommonModel.targetTypes;
    vm.permission = PeriodModel.permission;

    vm.filter = {
        targetTypeId: null,
        isActive: 'true'
    };

    vm.currentPage = 1;
    vm.pageSize = 20;

    utilitiesFactory.numberEntryList(vm);
    utilitiesFactory.getDateFormat(vm);

    CommonModel.getTargetType().then(function () {
        vm.targetTypes = CommonModel.targetTypes;
    });

    var getPeriod = function (targetTypeId, isActive) {
        PeriodModel.getPeriod(targetTypeId, isActive).then(function () {
            vm.periods = PeriodModel.periods;
            utilitiesFactory.searchOnPage(vm, vm.periods);
        });
    };

    getPeriod(vm.filter.targetTypeId, vm.filter.isActive);

    vm.query = function (filter) {
        getPeriod(filter.targetTypeId, filter.isActive);
    };

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };

    vm.create = function () {
        PeriodModel.openPopupCreatePeriod(vm).then(function () {
            vm.modalInstance = null;
            getPeriod(vm.filter.targetTypeId, vm.filter.isActive);
        });
    };

    vm.update = function (periodId) {
        vm.updatePeriodUrl = baseUrl + 'Period/Update/' + periodId;
    };

    vm.confirmActive = function (period) {
        PeriodModel.openPopupUpdateStatusPeriod(period);
    };

    vm.delete = function (period) {
        PeriodModel.openPopupDeletePeriod(vm, period);
    };

    PeriodModel.getPermissionOnPeriod().then(function () {
        vm.permission = PeriodModel.permission;
    });

    vm.viewEmployeeSetting = function (period) {
        vm.viewEmployeeSettingUrl = baseUrl + 'Period/Update/' + period.PeriodId + '?viewSetting';
    };
}]);

periodModule.controller('CreatePeriodController',
['$modalInstance', 'PeriodModel', 'CommonModel',
function ($modalInstance, PeriodModel, CommonModel) {
    var vm = this;

    vm.targetTypes = CommonModel.targetTypes;
    vm.period = {
        Name: null,
        TargetTypeId: null,
        StartDate: null,
        EndDate: null,
        RawStartDate: null,
        RawEndDate: null,
        AttendanceStartDate: null,
        AttendanceEndDate: null,
        IsActive: true,
        Description: null
    };

    vm.calendar = PeriodModel.calendar;

    vm.openPopupStartDate = function (event) {
        PeriodModel.openPopupStartDate(event);
    };

    vm.openPopupEndDate = function (event) {
        PeriodModel.openPopupEndDate(event);
    };

    vm.openPopupAttendanceStartDate = function (event) {
        PeriodModel.openPopupAttendanceStartDate(event);
    };

    vm.openPopupAttendanceEndDate = function (event) {
        PeriodModel.openPopupAttendanceEndDate(event);
    };

    vm.save = function (period) {
        if (PeriodModel.validatePeriod(period)) {
            period.StartDate = period.RawStartDate ? period.RawStartDate.toDateString() : null;
            period.EndDate = period.RawEndDate ? period.RawEndDate.toDateString() : null;
            period.AttendanceStartDate = period.RawAttendanceStartDate ? period.RawAttendanceStartDate.toDateString() : null;
            period.AttendanceEndDate = period.RawAttendanceEndDate ? period.RawAttendanceEndDate.toDateString() : null;

            PeriodModel.createPeriod(period, $modalInstance);
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
}]);

periodModule.controller('UpdatePeriodController',
['$window', '$routeParams', 'PeriodModel', 
function ($window, $routeParams, PeriodModel) {
    var vm = this;

    vm.period = null;
    vm.periodId = parseInt($routeParams.id);
    vm.viewMode = $routeParams.viewSetting ? true : false;

    vm.date = new Date();
    vm.permission = null;

    vm.dataTab = {
        basicInformation: !vm.viewMode,
        employeeSetting: vm.viewMode,
        processSetting: false
    };

    vm.tabBasicSelect = function () {
        vm.dataTab.basicInformation = true;
        vm.dataTab.employeeSetting = false;
        vm.dataTab.processSetting = false;
    };

    vm.tabEmployeeSettingSelect = function () {
        vm.dataTab.basicInformation = false;
        vm.dataTab.employeeSetting = true;
        vm.dataTab.processSetting = false;
    };

    vm.tabProcessSettingSelect = function () {
        vm.dataTab.basicInformation = false;
        vm.dataTab.employeeSetting = false;
        vm.dataTab.processSetting = true;
    };

    PeriodModel.getPeriodById(vm.periodId).then(function () {
        vm.period = PeriodModel.period;
        vm.period.TargetTypeId = vm.period.TargetType.TargetTypeId;
    });

    vm.backToList = function () {
        $window.history.back();
    };

    PeriodModel.getPermissionOnPeriod().then(function () {
        vm.permission = PeriodModel.permission;
    });
}]);

periodModule.controller('UpdateBasicInformationPeriodController',
['$scope', '$window', 'PeriodModel', 'CommonModel',
function ($scope, $window, PeriodModel, CommonModel) {
    var vm = this;

    vm.targetTypes = CommonModel.targetTypes;
    vm.period = {};
    vm.isFirstLoad = true;

    vm.calendar = PeriodModel.calendar;

    vm.openPopupStartDate = function (event) {
        PeriodModel.openPopupStartDate(event);
    };

    vm.openPopupEndDate = function (event) {
        PeriodModel.openPopupEndDate(event);
    };

    vm.openPopupAttendanceStartDate = function (event) {
        PeriodModel.openPopupAttendanceStartDate(event);
    };

    vm.openPopupAttendanceEndDate = function (event) {
        PeriodModel.openPopupAttendanceEndDate(event);
    };

    var getTargetType = function () {
        CommonModel.getTargetType().then(function () {
            vm.targetTypes = CommonModel.targetTypes;
            vm.isFirstLoad = false;
        });
    };

    $scope.$watch(function () { return $scope.updateCtrl.period; },
        function (newVal) {
            if (newVal) {
                vm.period = $scope.updateCtrl.period;

                vm.period.RawStartDate = vm.period.StartDate ? new Date(vm.period.StartDate) : null;
                vm.period.RawEndDate = vm.period.EndDate ? new Date(vm.period.EndDate) : null;
                vm.period.RawAttendanceStartDate = vm.period.AttendanceStartDate ? new Date(vm.period.AttendanceStartDate) : null;
                vm.period.RawAttendanceEndDate = vm.period.AttendanceEndDate ? new Date(vm.period.AttendanceEndDate) : null;

                if ($scope.updateCtrl.dataTab.basicInformation && vm.isFirstLoad && vm.period.PeriodId) {
                    getTargetType();
                }
            }
        });

    $scope.$watch(function () { return $scope.updateCtrl.dataTab.basicInformation; },
        function (newVal) {
            if (newVal && vm.isFirstLoad && vm.period.PeriodId) {
                getTargetType();
            }
        });

    vm.save = function (period) {
        if (PeriodModel.validatePeriod(period)) {
            period.StartDate = period.RawStartDate ? period.RawStartDate.toDateString() : null;
            period.EndDate = period.RawEndDate ? period.RawEndDate.toDateString() : null;
            period.AttendanceStartDate = period.RawAttendanceStartDate ? period.RawAttendanceStartDate.toDateString() : null;
            period.AttendanceEndDate = period.RawAttendanceEndDate ? period.RawAttendanceEndDate.toDateString() : null;

            PeriodModel.updateBasicInformationPeriod(period);
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };
}]);

periodModule.controller('ProcessSettingPeriodController',
['$scope', '$window', 'PeriodModel', 'FlowMngModel',
function ($scope, $window, PeriodModel, FlowMngModel) {
    var vm = this;

    vm.period = {};
    vm.isFirstLoad = true;
    vm.processSettings = PeriodModel.processSettings;
    vm.remindDay = FlowMngModel.remindDay;

    var getProcessSetting = function (periodId) {
        PeriodModel.getProcessSettingByPeriodId(periodId).then(function () {
            vm.processSettings = PeriodModel.processSettings;
            vm.isFirstLoad = false;
        });
    };

    $scope.$watch(function () { return $scope.updateCtrl.period; },
        function (newVal) {
            if (newVal) {
                vm.period = $scope.updateCtrl.period;

                if ($scope.updateCtrl.dataTab.processSetting && vm.isFirstLoad && vm.period.PeriodId) {
                    getProcessSetting(vm.period.PeriodId);
                }
            }
        });

    $scope.$watch(function () { return $scope.updateCtrl.dataTab.processSetting; },
    function (newVal) {
        if (newVal && vm.isFirstLoad && vm.period) {
            getProcessSetting(vm.period.PeriodId);
        }
    });

    vm.save = function (period) {
        var periodProcessSetting = {
            PeriodId: vm.period.PeriodId,
            Processes: vm.processSettings
        };

        PeriodModel.updatePeriodProcessSetting(periodProcessSetting);
    };

    vm.cancel = function () {
        $window.history.back();
    };
}]);

periodModule.controller('EmployeeSettingPeriodController',
['$scope', '$routeParams', '$window', 'PeriodModel', 'utilitiesFactory', 'CommonModel', 'EmployeeModel', 'notificationFactory',
function ($scope, $routeParams, $window, PeriodModel, utilitiesFactory, CommonModel, EmployeeModel, notificationFactory) {
    var vm = this;

    vm.period = null;
    vm.periodId = null;
    vm.employeeSettings = PeriodModel.employeeSettings;
    vm.departments = CommonModel.departments;
    vm.treeDepartment = CommonModel.treeDepartment;
    vm.isFirstLoad = true;
    vm.departmentOptions = [];

    vm.currentPage = 1;
    vm.pageSize = 15;
    utilitiesFactory.numberEntryList(vm);
    utilitiesFactory.getDateFormat(vm);
    vm.selectedAll = false;

    vm.filter = {
        periodId: null,
        departmentId: null,
        isExcludedEmployee: null
    };

    vm.filterAssign = {
        employee: ''
    };

    var getPeriodEmployeeSetting = function (periodId, departmentId, isExcludedEmployee) {
        PeriodModel.getPeriodEmployeeSettingById(periodId, departmentId, isExcludedEmployee).then(function () {
            vm.employeeSettings = PeriodModel.employeeSettings;
            utilitiesFactory.searchOnPage(vm, vm.employeeSettings);
            vm.isFirstLoad = false;
            vm.selectedAll = false;
        });
    };

    var getDepartmentAndEmployeeSetting = function () {
        CommonModel.getDepartment(null, true).then(function () {
            vm.departments = CommonModel.departments;
            vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
            vm.departmentOptions = angular.copy(vm.treeDepartment);
        });

        getPeriodEmployeeSetting(vm.filter.periodId, vm.filter.departmentId, vm.filter.isExcludedEmployee);
        vm.permission = $scope.updateCtrl.permission;
    };

    $scope.$watch(function () { return $scope.updateCtrl.period && $scope.updateCtrl.permission; },
    function (newVal) {
        if (newVal) {
            vm.period = $scope.updateCtrl.period;
            vm.filter.periodId = vm.period.PeriodId;

            if ($scope.updateCtrl.dataTab.employeeSetting && vm.isFirstLoad) {
                getDepartmentAndEmployeeSetting();
            }
        }
    });

    $scope.$watch(function () { return $scope.updateCtrl.dataTab.employeeSetting; },
        function (newVal) {
            if (newVal && vm.isFirstLoad && vm.period && $scope.updateCtrl.permission) {
                getDepartmentAndEmployeeSetting();
            }
        });

    vm.query = function (filter) {
        getPeriodEmployeeSetting(filter.periodId, filter.departmentId, filter.isExcludedEmployee);
    };

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };

    vm.getEmployee = function () {
        return EmployeeModel.getAllEmployee(vm.searchEmployee);
    };

    vm.getEmployee = function () {
        return EmployeeModel.getAllEmployee(vm.searchEmployee);
    };

    vm.unSelectEmployee = function () {
        if (vm.searchEmployee.trim() === '') {
            if (vm.filterAssign.employee) {
                vm.filterAssign.employee = null;
            }
        }
    };

    var getSelectedDepartment = function (departments) {
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
    };

    var resetFilterAssign = function () {
        vm.filterAssign.employee = '';
        vm.searchEmployee = '';

        angular.forEach(vm.departments, function (department) {
            department.ticked = false;
        });
    };

    vm.addEmployee = function (filterAssign) {
        var employeeId = null;

        if (filterAssign.employee) {
            employeeId = filterAssign.employee.EmployeeId;
        }

        var employeeInfo = {
            periodId: vm.period.PeriodId,
            employeeId: employeeId,
            departments: getSelectedDepartment(vm.departments)
        };

        PeriodModel.addEmployeeToPeriod(employeeInfo).then(function () {
            resetFilterAssign();
            getPeriodEmployeeSetting(vm.filter.periodId, vm.filter.departmentId, vm.filter.isExcludedEmployee);
        });
    };

    vm.updateEmployeeSetting = function (employeeSetting) {
        PeriodModel.openPopupUpdateEmployeeSetting(vm, employeeSetting.EmployeeId, employeeSetting);
    };

    vm.includeExcludeEmployee = function (employee) {
        PeriodModel.openPopupIncludeExcludeEmployee(vm.period.PeriodId, employee);
    };

    vm.manageSetting = function () {
        var selectedEmployees = PeriodModel.getSelectedEmployee(vm.employeeSettings);

        if (!selectedEmployees) {
            notificationFactory.error("SelectEmployeeBeforeEditSettingWarning");
        }
        else {
            PeriodModel.openPopupUpdateEmployeeSetting(vm, selectedEmployees);
        }
    };

    vm.generateEvaluation = function () {
        var selectedEmployees = PeriodModel.getSelectedEmployee(vm.employeeSettings),
            period = {
                periodId: vm.period.PeriodId,
                employeeIds: selectedEmployees
            };

        if (!selectedEmployees) {
            notificationFactory.error("SelectEmployeeBeforeGenerateEvaluation");
        }
        else {
            PeriodModel.openPopupGenerateEvaluation(period);
        }
    };

    /*
    ** Check all and sync selected employee setting between full list and filter list.
    */
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
            if (employeeSetting.EmployeeId === employeeId) {
                angular.forEach(vm.filteredItems, function (filterItem) {
                    if (employeeSetting.EmployeeId === filterItem.EmployeeId) {
                        filterItem.selected = selected;
                        employeeSetting.selected = selected;
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

    /*
    ** Export report.
    */
    vm.exportReport = function (filter) {
          vm.exportCriteria = {
              periodId: filter.periodId,
            departmentId: filter.departmentId,
            isExcludedEmployee: filter.isExcludedEmployee
        };

          $scope.$broadcast('exportEmployeeSettingReport');
    };
}]);

periodModule.controller('UpdateEmployeeSettingPeriodController',
['$scope', '$modalInstance', 'period', 'employeeSetting', 'selectedEmployee',
'permission', 'PeriodModel', 'EmployeeModel', 'TargetTemplateModel', 'FlowMngModel',
function ($scope, $modalInstance, period, employeeSetting, selectedEmployee,
permission, PeriodModel, EmployeeModel, TargetTemplateModel, FlowMngModel) {
    var vm = this;

    vm.period = period;
    vm.employeeSetting = employeeSetting;
    vm.permission = permission;
    vm.periodDateFormat = PeriodModel.periodDateFormat;
    vm.evaluatorType = PeriodModel.evaluatorType;
    vm.selectedEmployee = selectedEmployee;
    vm.flowCalendar = PeriodModel.flowCalendar;
    
    vm.employeeOptions = {
        humanResource: {
            ids: null,
            StartDate: null,
            EndDate: null
        },
        selfEvaluator: {
            ids: null,
            StartDate: null,
            EndDate: null
        },
        evaluator: {
            ids: null,
            StartDate: null,
            EndDate: null
        },
        reviewer: {
            ids: null,
            StartDate: null,
            EndDate: null
        },
        management: {
            ids: null,
            StartDate: null,
            EndDate: null
        },
        targetTemplateId: null,
        humanResourceEmployees: [],
        evaluatorEmployees: [],
        reviewerEmployees: [],
        managementEmployees: []
    };

    var createEmployeeIds = function (evaluators) {
        var ids = [];

        if (evaluators && evaluators.length > 0) {
            angular.forEach(evaluators, function (evaluator) {
                if (evaluator.EvaluatorInfo) {
                    ids.push(evaluator.EvaluatorInfo.EmployeeId);
                }
                else {
                    ids.push(-1);
                }
            });
        }

        return ids;
    };

    var getEvaluators = function (evaluators) {
        var evaluator = {
            StartDate: null,
            EndDate: null,
            ids: [],
            ProcessId: 0
        };

        if (evaluators && evaluators.length > 0) {
            evaluator.StartDate = evaluators[0].StartDate ? new Date(evaluators[0].StartDate) : null;
            evaluator.EndDate = evaluators[0].EndDate ? new Date(evaluators[0].EndDate) : null;
            evaluator.ids = createEmployeeIds(evaluators);
            evaluator.ProcessId = evaluators[0].ProcessId;
        }

        return evaluator;
    };

    var updateTargetTemplates = function (targetTemplate) {
        var existed = false;

        angular.forEach(vm.targetTemplates, function (item) {
            if (item.TargetTemplateId === targetTemplate.TargetTemplateId) {
                existed = true;
            }
        });

        if (!existed) {
            vm.targetTemplates.push(targetTemplate);
        }
    };

    var setEmployeeSetting = function () {
        vm.employeeOptions.humanResource = getEvaluators(vm.employeeSetting.HumanResources);
        vm.employeeOptions.selfEvaluator = getEvaluators(vm.employeeSetting.SelfEvaluators);
        vm.employeeOptions.evaluator = getEvaluators(vm.employeeSetting.Evaluators);
        vm.employeeOptions.reviewer = getEvaluators(vm.employeeSetting.Reviewers);
        vm.employeeOptions.management = getEvaluators(vm.employeeSetting.Managements);
        vm.employeeOptions.targetTemplateId = vm.employeeSetting.TargetTemplate.TargetTemplateId;

        updateTargetTemplates(vm.employeeSetting.TargetTemplate);
    };

    /*
   ** Get employees flow and target setting.
   */
    TargetTemplateModel.getTargetTemplate(vm.period.TargetType.TargetTypeId, true).then(function () {
        vm.targetTemplates = TargetTemplateModel.targetTemplates;
    });

    EmployeeModel.getAllEmployee().then(function () {
        vm.employees = EmployeeModel.employees;
        vm.employeeOptions.humanResourceEmployees = angular.copy(vm.employees);
        vm.employeeOptions.evaluatorEmployees = angular.copy(vm.employees);
        vm.employeeOptions.reviewerEmployees = angular.copy(vm.employees);
        vm.employeeOptions.managementEmployees = angular.copy(vm.employees);

        if (vm.employeeSetting) {
            setEmployeeSetting();
        }
    });

    vm.updateTrigger = function () {
        $scope.$broadcast('evaluatorChosen');
    };

    vm.openFlowPopupStartDate = function (event, evaluatorType) {
        PeriodModel.openFlowPopupStartDate(vm, event, evaluatorType);
    };

    vm.openFlowPopupEndDate = function (event, evaluatorType) {
        PeriodModel.openFlowPopupEndDate(vm, event, evaluatorType);
    };

    var getUpdatedEmployeeSetting = function (employeeSetting, employeeOptions) {
        employeeSetting.TargetTemplate = PeriodModel.getUpdateTargetTemplate(employeeOptions.targetTemplateId, vm.targetTemplates);
        employeeSetting.HumanResources = PeriodModel.getUpdateEvaluators(employeeOptions.humanResource, vm.evaluatorType.HumanResources, vm.employees);
        employeeSetting.SelfEvaluators = PeriodModel.getUpdateEvaluators(employeeOptions.selfEvaluator, vm.evaluatorType.SelfEvaluator, vm.employees);
        employeeSetting.Evaluators = PeriodModel.getUpdateEvaluators(employeeOptions.evaluator, vm.evaluatorType.Evaluator, vm.employees);
        employeeSetting.Reviewers = PeriodModel.getUpdateEvaluators(employeeOptions.reviewer, vm.evaluatorType.Reviewer, vm.employees);
        employeeSetting.Managements = PeriodModel.getUpdateEvaluators(employeeOptions.management, vm.evaluatorType.Management, vm.employees);

        return employeeSetting;
    };

    vm.save = function () {
        if (PeriodModel.validateEmployeeSetting(vm.employeeOptions)) {
            var employeeAssignmentSetting = PeriodModel.createAssignmentSettings(vm.employeeOptions, vm.period.PeriodId, vm.selectedEmployee);

            PeriodModel.updateEmployeeSettingOfPeriod(employeeAssignmentSetting).then(function () {
                if (!vm.employeeSetting) {
                    $modalInstance.close();
                }
                else {
                    var updatedEmployeeSetting = getUpdatedEmployeeSetting(vm.employeeSetting, vm.employeeOptions);
                    $modalInstance.close(updatedEmployeeSetting);
                }
            });
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
}]);
