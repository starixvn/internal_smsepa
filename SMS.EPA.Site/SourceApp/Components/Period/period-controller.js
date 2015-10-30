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
        vm.searchKey = '';
        vm.currentPage = 1;
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
['$scope', '$routeParams', '$window', 'PeriodModel', 'utilitiesFactory', 'CommonModel', 'notificationFactory',
function ($scope, $routeParams, $window, PeriodModel, utilitiesFactory, CommonModel, notificationFactory) {
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
        isExcludedEmployee: null,
        isGeneratedEPA: "",
        isEvaluated: ""
    };

    vm.filterAssign = {
        employee: ''
    };

    var getPeriodEmployeeSetting = function (periodId, departmentId, isExcludedEmployee, isGeneratedEPA, isEvaluated) {
        PeriodModel.getPeriodEmployeeSettingById(periodId, departmentId, isExcludedEmployee, isGeneratedEPA, isEvaluated).then(function () {
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

        getPeriodEmployeeSetting(vm.filter.periodId, vm.filter.departmentId, vm.filter.isExcludedEmployee, vm.filter.isGeneratedEPA, vm.filter.isEvaluated);
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
        getPeriodEmployeeSetting(filter.periodId, filter.departmentId, filter.isExcludedEmployee, filter.isGeneratedEPA, filter.isEvaluated);
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

        if (employeeInfo.employeeId || (employeeInfo.departments && employeeInfo.departments.length > 0)) {
            PeriodModel.addEmployeeToPeriod(employeeInfo).then(function () {
                resetFilterAssign();
                getPeriodEmployeeSetting(vm.filter.periodId, vm.filter.departmentId, vm.filter.isExcludedEmployee, vm.filter.isGeneratedEPA, vm.filter.isEvaluated);
            });
        }
        else {
            notificationFactory.error("SelectDepartmentSectionEmployeeBeforeAddToPeriodWarning");
        }
    };

    vm.updateEmployeeSetting = function (employeeSetting) {
        PeriodModel.openPopupUpdateEmployeeSetting(vm, [employeeSetting]);
    };

    vm.includeExcludeEmployee = function (employee) {
        PeriodModel.openPopupIncludeExcludeEmployee(vm, vm.period.PeriodId, employee);
    };

    vm.manageSetting = function () {
        var selectedEmployees = PeriodModel.getSelectedEmployee(vm.employeeSettings);

        if (!selectedEmployees || selectedEmployees.length === 0) {
            notificationFactory.error("SelectEmployeeBeforeEditSettingWarning");
        }
        else {
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
        }
        else {
            if (PeriodModel.validateEmployeeSettingGenerateEPA(selectedEmployees)) {
                PeriodModel.openPopupGenerateEvaluation(vm, period).then(function () {
                    vm.query(vm.filter);
                });
            }
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

    /*
    ** Export report.
    */
    vm.exportReport = function (filter) {
          vm.exportCriteria = {
              periodId: filter.periodId,
              periodName: vm.period.Name,
              departmentId: filter.departmentId,
              isExcludedEmployee: filter.isExcludedEmployee
        };

          $scope.$broadcast('exportEmployeeSettingReport');
    };
}]);

periodModule.controller('UpdateEmployeeSettingPeriodController',
['$scope', '$modalInstance', 'period', 'selectedEmployees',
'permission', 'PeriodModel', 'TargetTemplateModel', 'FlowMngModel', 'CommonModel',
function ($scope, $modalInstance, period, selectedEmployees,
permission, PeriodModel, TargetTemplateModel, FlowMngModel, CommonModel) {
    var vm = this,
        date = new Date();

    vm.period = period;
    vm.selectedEmployees = selectedEmployees;
    vm.singleMode = vm.selectedEmployees.length === 1 ? true : false;

    vm.permission = permission;
    vm.periodDateFormat = PeriodModel.periodDateFormat;
    vm.evaluatorType = PeriodModel.evaluatorType;

    var employeeInfo = PeriodModel.getSelectedEmployeeInfo(vm.selectedEmployees);
    vm.selectedEmployeeIds = employeeInfo.selectedEmployeeIds;
    vm.selectedEmployeeNames = employeeInfo.selectedEmployeeNames;

    vm.flowCalendar = PeriodModel.flowCalendar;
    vm.hasSelfStarted = PeriodModel.hasSelfStarted;

    vm.fieldEdit = {
        targetTemplate: false,
        evaluator: false,
        startDate: false,
        endDate: false
    };

    vm.employeeOptions = {
        humanResource: {
            ids: null,
            StartDate: null,
            EndDate: null,
            ProcessStatus: null,
            HasEvaluationGenerated: null
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
            });
        }

        return ids;
    };

    var getEvaluators = function (evaluators) {
        var evaluator = {
            StartDate: date,
            EndDate: date,
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

    var setEmployeeSetting = function (employeeSetting) {
        vm.employeeOptions.humanResource = getEvaluators(employeeSetting.HumanResources);
        vm.employeeOptions.selfEvaluator = getEvaluators(employeeSetting.SelfEvaluators);
        vm.employeeOptions.evaluator = getEvaluators(employeeSetting.Evaluators);
        vm.employeeOptions.reviewer = getEvaluators(employeeSetting.Reviewers);
        vm.employeeOptions.management = getEvaluators(employeeSetting.Managements);

        if (employeeSetting.TargetTemplate) {
            vm.employeeOptions.targetTemplateId = employeeSetting.TargetTemplate.TargetTemplateId;
            updateTargetTemplates(employeeSetting.TargetTemplate);
        }
    };

    var getProcessSetingById = function (processSettings, processId) {
        var processSetting = null;

        angular.forEach(processSettings, function (processItem) {
            if (processItem.ProcessId === processId) {
                processSetting = processItem;
            }
        });

        return processSetting;
    };

    var setDateSetting = function (startDate, evaluator, processSettings, processId) {
        var processSetting = getProcessSetingById(processSettings, processId);

        evaluator.StartDate = angular.copy(startDate);
        evaluator.EndDate = new Date(startDate.getFullYear(), startDate.getMonth(), startDate.getDate() + processSetting.Duration - 1);
    };

    var getProcessAndSetDateSetting = function () {
        PeriodModel.getProcessSettingByPeriodId(vm.period.PeriodId).then(function () {
            vm.processSettings = PeriodModel.processSettings;

            var startDate = vm.period.StartDate ? new Date(vm.period.StartDate) : new Date(),
                year = null,
                month = null,
                day = null;

            setDateSetting(startDate, vm.employeeOptions.humanResource, vm.processSettings, vm.evaluatorType.HumanResources);

            year = vm.employeeOptions.humanResource.EndDate.getFullYear();
            month = vm.employeeOptions.humanResource.EndDate.getMonth();
            day = vm.employeeOptions.humanResource.EndDate.getDate();
            setDateSetting(new Date(year, month, day + 1), vm.employeeOptions.selfEvaluator, vm.processSettings, vm.evaluatorType.SelfEvaluator);

            year = vm.employeeOptions.selfEvaluator.EndDate.getFullYear();
            month = vm.employeeOptions.selfEvaluator.EndDate.getMonth();
            day = vm.employeeOptions.selfEvaluator.EndDate.getDate();
            setDateSetting(new Date(year, month, day + 1), vm.employeeOptions.evaluator, vm.processSettings, vm.evaluatorType.Evaluator);

            year = vm.employeeOptions.evaluator.EndDate.getFullYear();
            month = vm.employeeOptions.evaluator.EndDate.getMonth();
            day = vm.employeeOptions.evaluator.EndDate.getDate();
            setDateSetting(new Date(year, month, day + 1), vm.employeeOptions.reviewer, vm.processSettings, vm.evaluatorType.Reviewer);

            year = vm.employeeOptions.reviewer.EndDate.getFullYear();
            month = vm.employeeOptions.reviewer.EndDate.getMonth();
            day = vm.employeeOptions.reviewer.EndDate.getDate();
            setDateSetting(new Date(year, month, day + 1), vm.employeeOptions.management, vm.processSettings, vm.evaluatorType.Management);
        });
    };

    /*
   ** Get employees flow and target setting.
   */
    TargetTemplateModel.getTargetTemplate(vm.period.TargetType.TargetTypeId, true).then(function () {
        vm.targetTemplates = TargetTemplateModel.targetTemplates;
        vm.employeeOptions.targetTemplateId = vm.targetTemplates.length > 0 ? vm.targetTemplates[0].TargetTemplateId : null;

        CommonModel.getAllEmployee().then(function () {
            vm.employees = CommonModel.employees;
            vm.employeeOptions.humanResourceEmployees = angular.copy(vm.employees);
            vm.employeeOptions.evaluatorEmployees = angular.copy(vm.employees);
            vm.employeeOptions.reviewerEmployees = angular.copy(vm.employees);
            vm.employeeOptions.managementEmployees = angular.copy(vm.employees);

            if (vm.singleMode) {
                setEmployeeSetting(vm.selectedEmployees[0]);
            }
            else {
                getProcessAndSetDateSetting();
            }
        });
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

    PeriodModel.checkSelfProcessStarted(vm.period.PeriodId, vm.selectedEmployeeIds).then(function () {
        vm.hasSelfStarted = PeriodModel.hasSelfStarted;
    });

    var checkChangeEmployeeSetting = function (employeeOptions, employeeSetting) {
        var hasEdited = false,
            humanResource = getEvaluators(employeeSetting.HumanResources),
            selfEvaluator = getEvaluators(employeeSetting.SelfEvaluators),
            evaluator = getEvaluators(employeeSetting.Evaluators),
            reviewer = getEvaluators(employeeSetting.Reviewers),
            management = getEvaluators(employeeSetting.Managements),
            originalTargetTemplateId = employeeSetting.TargetTemplate ? employeeSetting.TargetTemplate.TargetTemplateId : null;

        if (vm.employeeOptions.targetTemplateId !== originalTargetTemplateId) {
            hasEdited = true;
        }
        else if (JSON.stringify(humanResource) !== JSON.stringify(employeeOptions.humanResource)) {
            hasEdited = true;
        }
        else if (JSON.stringify(selfEvaluator) !== JSON.stringify(employeeOptions.selfEvaluator)) {
            hasEdited = true;
        }
        else if (JSON.stringify(evaluator) !== JSON.stringify(employeeOptions.evaluator)) {
            hasEdited = true;
        }
        else if (JSON.stringify(reviewer) !== JSON.stringify(employeeOptions.reviewer)) {
            hasEdited = true;
        }
        else if (JSON.stringify(management) !== JSON.stringify(employeeOptions.management)) {
            hasEdited = true;
        }

        return hasEdited;
    };

    vm.save = function () {
        if (vm.singleMode) {
            if (PeriodModel.validateEmployeeSetting(vm.employeeOptions)) {
                if (checkChangeEmployeeSetting(vm.employeeOptions, vm.selectedEmployees[0])) {
                    var employeeAssignmentSetting = PeriodModel.createAssignmentSettings(vm.employeeOptions, vm.period.PeriodId, vm.selectedEmployees, vm.selectedEmployeeIds);

                    PeriodModel.updateEmployeeSettingOfPeriod(employeeAssignmentSetting).then(function () {
                        $modalInstance.close();
                    });
                }
                else {
                    $modalInstance.dismiss("cancel");
                }
            }
        }
        else {
            if (PeriodModel.checkHasFieldEdit(vm.fieldEdit)) {
                var updatedEmployees = PeriodModel.updateEmployeeSettings(vm.employeeOptions, vm.fieldEdit, vm.selectedEmployees);

                if (PeriodModel.validateEmployeeSettingMultipleMode(updatedEmployees)) {
                    var employeeAssignmentSetting = PeriodModel.createAssignmentSettingsMultipleMode(updatedEmployees, vm);

                    PeriodModel.updateEmployeeSettingOfPeriod(employeeAssignmentSetting).then(function () {
                        $modalInstance.close();
                    });
                }
            }
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
}]);