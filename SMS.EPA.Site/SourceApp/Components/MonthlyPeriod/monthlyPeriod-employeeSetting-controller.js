'use strict';
/*global monthlyPeriodModule*/

monthlyPeriodModule.controller('EmployeeSettingMonthlyPeriodController',
['$scope', '$routeParams', '$window', 'MonthlyPeriodModel', 'utilitiesFactory', 'CommonModel', 'notificationFactory',
function ($scope, $routeParams, $window, MonthlyPeriodModel, utilitiesFactory, CommonModel, notificationFactory) {
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
        isGeneratedEPA: "",
        isEvaluated: "",
        epaStatus: $routeParams.inprogress === 'true' ? '0' : ''
    };

    vm.filterAssign = {
        employee: ''
    };

    function getEmployeeSetting(filter) {
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
        });
    }

    $scope.$watch(function () {
        return $scope.updateCtrl.dataTab.employeeSetting && $scope.updateCtrl.period && $scope.updateCtrl.permission;
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
        }
        else {
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
        }
        else {
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
        }
        else {
            if (MonthlyPeriodModel.validateEmployeeSettingGenerateEPA(selectedEmployees)) {
                MonthlyPeriodModel.openPopupGenerateEvaluation(vm, period).then(function () {
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

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };
}]);

monthlyPeriodModule.controller('UpdateEmployeeSettingMonthlyPeriodController',
['$scope', '$modalInstance', 'MonthlyPeriodModel', 'CommonModel', 'enumerationsFactory', 'period', 'selectedEmployees', 'permission',
function ($scope, $modalInstance, MonthlyPeriodModel, CommonModel, enumerationsFactory, period, selectedEmployees, permission) {
    var vm = this,
        date = new Date();

    vm.period = period;
    vm.selectedEmployees = selectedEmployees;
    vm.singleMode = vm.selectedEmployees.length === 1 ? true : false;
    vm.permission = permission;
    vm.policyEnum = enumerationsFactory.policy;

    vm.fieldEdit = {
        evaluator: false,
        startDate: false,
        endDate: false
    };

    vm.evaluatorType = {
        HumanResources: 1,
        SelfEvaluator: 2,
        Evaluator: 3,
        Reviewer: 4
    };

    vm.flowCalendar = {
        humanResource: {
            startDateOpened: false,
            endDateOpened: false
        },
        selfEvaluator: {
            startDateOpened: false,
            endDateOpened: false
        },
        evaluator: {
            startDateOpened: false,
            endDateOpened: false
        },
        reviewer: {
            startDateOpened: false,
            endDateOpened: false
        }
    };

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
        targetTemplateId: null,
        humanResourceEmployees: [],
        evaluatorEmployees: [],
        reviewerEmployees: []
    };

    var employeeInfo = MonthlyPeriodModel.getSelectedEmployeeInfo(vm.selectedEmployees);
    vm.selectedEmployeeIds = employeeInfo.selectedEmployeeIds;
    vm.selectedEmployeeNames = employeeInfo.selectedEmployeeNames;
    vm.hasSelfStarted = MonthlyPeriodModel.hasSelfStarted;

    MonthlyPeriodModel.checkSelfProcessStarted(vm.period.PeriodId, vm.selectedEmployeeIds).then(function () {
        vm.hasSelfStarted = MonthlyPeriodModel.hasSelfStarted;
    });

    function resetCalendar() {
        vm.flowCalendar.humanResource.startDateOpened = false;
        vm.flowCalendar.humanResource.endDateOpened = false;

        vm.flowCalendar.selfEvaluator.startDateOpened = false;
        vm.flowCalendar.selfEvaluator.endDateOpened = false;

        vm.flowCalendar.evaluator.startDateOpened = false;
        vm.flowCalendar.evaluator.endDateOpened = false;

        vm.flowCalendar.reviewer.startDateOpened = false;
        vm.flowCalendar.reviewer.endDateOpened = false;
    }

    vm.openFlowPopupStartDate = function (event, evaluatorType) {
        event.preventDefault();
        event.stopPropagation();
        resetCalendar();

        switch (evaluatorType) {
            case vm.evaluatorType.HumanResources:
                vm.flowCalendar.humanResource.startDateOpened = true;
                break;
            case vm.evaluatorType.SelfEvaluator:
                vm.flowCalendar.selfEvaluator.startDateOpened = true;
                break;
            case vm.evaluatorType.Evaluator:
                vm.flowCalendar.evaluator.startDateOpened = true;
                break;
            case vm.evaluatorType.Reviewer:
                vm.flowCalendar.reviewer.startDateOpened = true;
                break;
        }
    };

    vm.openFlowPopupEndDate = function (event, evaluatorType) {
        event.preventDefault();
        event.stopPropagation();
        resetCalendar();

        switch (evaluatorType) {
            case vm.evaluatorType.HumanResources:
                vm.flowCalendar.humanResource.endDateOpened = true;
                break;
            case vm.evaluatorType.SelfEvaluator:
                vm.flowCalendar.selfEvaluator.endDateOpened = true;
                break;
            case vm.evaluatorType.Evaluator:
                vm.flowCalendar.evaluator.endDateOpened = true;
                break;
            case vm.evaluatorType.Reviewer:
                vm.flowCalendar.reviewer.endDateOpened = true;
                break;
        }
    };

    vm.updateTrigger = function () {
        $scope.$broadcast('evaluatorChosen');
    };

    function createEmployeeIds(evaluators) {
        var ids = [];

        if (evaluators && evaluators.length > 0) {
            angular.forEach(evaluators, function (evaluator) {
                if (evaluator.EvaluatorInfo) {
                    ids.push(evaluator.EvaluatorInfo.EmployeeId);
                }
            });
        }

        return ids;
    }

    function getEvaluators(evaluators) {
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
    }

    function updateTargetTemplates(targetTemplate) {
        var existed = false;

        angular.forEach(vm.targetTemplates, function (item) {
            if (item.TargetTemplateId === targetTemplate.TargetTemplateId) {
                existed = true;
            }
        });

        if (!existed) {
            vm.targetTemplates.push(targetTemplate);
        }
    }

    function setEmployeeSetting(employeeSetting) {
        vm.employeeOptions.humanResource = getEvaluators(employeeSetting.HumanResources);
        vm.employeeOptions.selfEvaluator = getEvaluators(employeeSetting.SelfEvaluators);
        vm.employeeOptions.evaluator = getEvaluators(employeeSetting.Evaluators);
        vm.employeeOptions.reviewer = getEvaluators(employeeSetting.Reviewers);

        if (employeeSetting.TargetTemplate) {
            vm.employeeOptions.targetTemplateId = employeeSetting.TargetTemplate.TargetTemplateId;
            updateTargetTemplates(employeeSetting.TargetTemplate);
        }
    }

    function getProcessSetingById(processSettings, processId) {
        var processSetting = null;

        angular.forEach(processSettings, function (processItem) {
            if (processItem.ProcessId === processId) {
                processSetting = processItem;
            }
        });

        return processSetting;
    }

    function setDateSetting(startDate, evaluator, processSettings, processId) {
        var processSetting = getProcessSetingById(processSettings, processId);

        evaluator.StartDate = angular.copy(startDate);
        evaluator.EndDate = new Date(startDate.getFullYear(), startDate.getMonth(), startDate.getDate() + processSetting.Duration - 1);
    }

    function getProcessAndSetDateSetting() {
        MonthlyPeriodModel.getProcessSettingByPeriodId(vm.period.PeriodId).then(function () {
            vm.processSettings = MonthlyPeriodModel.processSettings;

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
        });
    }

    /*
   ** Get employees flow and target setting.
   */
    CommonModel.getTargetTemplate(vm.period.TargetType.TargetTypeId, true).then(function () {
        vm.targetTemplates = CommonModel.targetTemplates;
        vm.employeeOptions.targetTemplateId = vm.targetTemplates.length > 0 ? vm.targetTemplates[0].TargetTemplateId : null;

        CommonModel.getAllEmployee().then(function () {
            vm.employees = CommonModel.employees;
            vm.employeeOptions.humanResourceEmployees = angular.copy(vm.employees);
            vm.employeeOptions.evaluatorEmployees = angular.copy(vm.employees);
            vm.employeeOptions.reviewerEmployees = angular.copy(vm.employees);

            if (vm.singleMode) {
                setEmployeeSetting(vm.selectedEmployees[0]);
            } else {
                getProcessAndSetDateSetting();
            }
        });
    });

    function checkChangeEmployeeSetting(employeeOptions, employeeSetting) {
        var hasEdited = false,
            humanResource = getEvaluators(employeeSetting.HumanResources),
            selfEvaluator = getEvaluators(employeeSetting.SelfEvaluators),
            evaluator = getEvaluators(employeeSetting.Evaluators),
            reviewer = getEvaluators(employeeSetting.Reviewers),
            originalTargetTemplateId = employeeSetting.TargetTemplate ? employeeSetting.TargetTemplate.TargetTemplateId : null;

        if (vm.employeeOptions.targetTemplateId !== originalTargetTemplateId) {
            hasEdited = true;
        } else if (JSON.stringify(humanResource) !== JSON.stringify(employeeOptions.humanResource)) {
            hasEdited = true;
        } else if (JSON.stringify(selfEvaluator) !== JSON.stringify(employeeOptions.selfEvaluator)) {
            hasEdited = true;
        } else if (JSON.stringify(evaluator) !== JSON.stringify(employeeOptions.evaluator)) {
            hasEdited = true;
        } else if (JSON.stringify(reviewer) !== JSON.stringify(employeeOptions.reviewer)) {
            hasEdited = true;
        }

        return hasEdited;
    }

    function createAssignmentSettings(employeeOptions, periodId, selectedEmployees, employeeIds) {
        var employeeAssignments = [],
            targetTemplateId = employeeOptions.targetTemplateId;

        angular.forEach(selectedEmployees, function (employee) {
            if (employeeOptions.humanResource.ids && employeeOptions.humanResource.ids.length > 0) {
                angular.forEach(employeeOptions.humanResource.ids, function (humanResourceId, index) {
                    var humanResourceAssignment = {
                        employeeId: employee.Employee.EmployeeId,
                        processId: vm.evaluatorType.HumanResources,
                        startDate: employeeOptions.humanResource.StartDate.toDateString(),
                        endDate: employeeOptions.humanResource.EndDate.toDateString(),
                        priority: index + 1,
                        evaluatorInfo: {
                            employeeId: humanResourceId
                        }
                    };

                    employeeAssignments.push(humanResourceAssignment);
                });
            }

            var selfEvaluatorAssignments = {
                employeeId: employee.Employee.EmployeeId,
                processId: vm.evaluatorType.SelfEvaluator,
                startDate: employeeOptions.selfEvaluator.StartDate.toDateString(),
                endDate: employeeOptions.selfEvaluator.EndDate.toDateString(),
                priority: 1,
                evaluatorInfo: {
                    employeeId: employeeOptions.selfEvaluator.ids[0]
                }
            };
            employeeAssignments.push(selfEvaluatorAssignments);

            if (employeeOptions.evaluator.ids && employeeOptions.evaluator.ids.length > 0) {
                angular.forEach(employeeOptions.evaluator.ids, function (evaluatorId, index) {
                    var evaluatorAssignment = {
                        employeeId: employee.Employee.EmployeeId,
                        processId: vm.evaluatorType.Evaluator,
                        startDate: employeeOptions.evaluator.StartDate.toDateString(),
                        endDate: employeeOptions.evaluator.EndDate.toDateString(),
                        priority: index + 1,
                        evaluatorInfo: {
                            employeeId: evaluatorId
                        }
                    };

                    employeeAssignments.push(evaluatorAssignment);
                });
            }

            if (employeeOptions.reviewer.ids && employeeOptions.reviewer.ids.length > 0) {
                angular.forEach(employeeOptions.reviewer.ids, function (reviewerId, index) {
                    var reviewerAssignment = {
                        employeeId: employee.Employee.EmployeeId,
                        processId: vm.evaluatorType.Reviewer,
                        startDate: employeeOptions.reviewer.StartDate.toDateString(),
                        endDate: employeeOptions.reviewer.EndDate.toDateString(),
                        priority: index + 1,
                        evaluatorInfo: {
                            employeeId: reviewerId
                        }
                    };

                    employeeAssignments.push(reviewerAssignment);
                });
            }
        });

        var employeeSetting = {
            periodId: periodId,
            targetTemplateId: targetTemplateId,
            employeeAssignments: employeeAssignments,
            employeeIds: employeeIds
        };

        return employeeSetting;
    }

    function createAssignmentSettingsMultipleMode(updatedEmployees) {
        var employeeAssignments = [];

        angular.forEach(updatedEmployees, function (employee) {
            if (employee.HumanResources && employee.HumanResources.length > 0) {
                angular.forEach(employee.HumanResources, function (humanResource, index) {
                    var humanResourceAssignment = {
                        employeeId: employee.Employee.EmployeeId,
                        processId: vm.evaluatorType.HumanResources,
                        startDate: new Date(humanResource.StartDate).toDateString(),
                        endDate: new Date(humanResource.EndDate).toDateString(),
                        priority: index + 1,
                        evaluatorInfo: {
                            employeeId: humanResource.EvaluatorInfo.EmployeeId
                        }
                    };

                    employeeAssignments.push(humanResourceAssignment);
                });
            }

            var selfEvaluatorAssignments = {
                employeeId: employee.Employee.EmployeeId,
                processId: vm.evaluatorType.SelfEvaluator,
                startDate: new Date(employee.SelfEvaluators[0].StartDate).toDateString(),
                endDate: new Date(employee.SelfEvaluators[0].EndDate).toDateString(),
                priority: 1,
                evaluatorInfo: {
                    employeeId: employee.Employee.EmployeeId
                }
            };
            employeeAssignments.push(selfEvaluatorAssignments);

            if (employee.Evaluators && employee.Evaluators.length > 0) {
                angular.forEach(employee.Evaluators, function (evaluator, index) {
                    var evaluatorAssignment = {
                        employeeId: employee.Employee.EmployeeId,
                        processId: vm.evaluatorType.Evaluator,
                        startDate: new Date(evaluator.StartDate).toDateString(),
                        endDate: new Date(evaluator.EndDate).toDateString(),
                        priority: index + 1,
                        evaluatorInfo: {
                            employeeId: evaluator.EvaluatorInfo.EmployeeId
                        }
                    };

                    employeeAssignments.push(evaluatorAssignment);
                });
            }

            if (employee.Reviewers && employee.Reviewers.length > 0) {
                angular.forEach(employee.Reviewers, function (reviewer, index) {
                    var reviewerAssignment = {
                        employeeId: employee.Employee.EmployeeId,
                        processId: vm.evaluatorType.Reviewer,
                        startDate: new Date(reviewer.StartDate).toDateString(),
                        endDate: new Date(reviewer.EndDate).toDateString(),
                        priority: index + 1,
                        evaluatorInfo: {
                            employeeId: reviewer.EvaluatorInfo.EmployeeId
                        }
                    };

                    employeeAssignments.push(reviewerAssignment);
                });
            }
        });

        var employeeSetting = {
            periodId: vm.period.PeriodId,
            employeeIds: vm.selectedEmployeeIds,
            targetTemplateId: vm.fieldEdit.targetTemplate ? vm.employeeOptions.targetTemplateId : null,
            employeeAssignments: employeeAssignments
        };

        return employeeSetting;
    }

    function updateEvaluatorToEmployee(employee, employeeOptions) {
        if (employeeOptions.humanResource.ids && employeeOptions.humanResource.ids.length > 0) {
            if (employee.HumanResources && employee.HumanResources.length > 0) {
                var originalHumanResource = employee.HumanResources[0];
                employee.HumanResources = [];

                angular.forEach(employeeOptions.humanResource.ids, function (id) {
                    var humanResource = {
                        ProcessId: originalHumanResource.ProcessId,
                        StartDate: originalHumanResource.StartDate,
                        EndDate: originalHumanResource.EndDate,
                        EvaluatorInfo: {
                            EmployeeId: id
                        }
                    };

                    employee.HumanResources.push(humanResource);
                });
            } else {
                angular.forEach(employeeOptions.humanResource.ids, function (id) {
                    var humanResource = {
                        ProcessId: vm.evaluatorType.HumanResources,
                        EvaluatorInfo: {
                            EmployeeId: id
                        }
                    };

                    employee.HumanResources.push(humanResource);
                });
            }
        }

        if (employeeOptions.evaluator.ids && employeeOptions.evaluator.ids.length > 0) {
            if (employee.Evaluators && employee.Evaluators.length > 0) {
                var originalEvaluator = employee.Evaluators[0];
                employee.Evaluators = [];

                angular.forEach(employeeOptions.evaluator.ids, function (id) {
                    var evaluator = {
                        ProcessId: originalEvaluator.ProcessId,
                        StartDate: originalEvaluator.StartDate,
                        EndDate: originalEvaluator.EndDate,
                        EvaluatorInfo: {
                            EmployeeId: id
                        }
                    };

                    employee.Evaluators.push(evaluator);
                });
            } else {
                angular.forEach(employeeOptions.evaluator.ids, function (id) {
                    var evaluator = {
                        ProcessId: vm.evaluatorType.Evaluator,
                        EvaluatorInfo: {
                            EmployeeId: id
                        }
                    };

                    employee.Evaluators.push(evaluator);
                });
            }
        }

        if (employeeOptions.reviewer.ids && employeeOptions.reviewer.ids.length > 0) {
            if (employee.Reviewers && employee.Reviewers.length > 0) {
                var originalReviewer = employee.Reviewers[0];
                employee.Reviewers = [];

                angular.forEach(employeeOptions.reviewer.ids, function (id) {
                    var reviewer = {
                        ProcessId: originalReviewer.ProcessId,
                        StartDate: originalReviewer.StartDate,
                        EndDate: originalReviewer.EndDate,
                        EvaluatorInfo: {
                            EmployeeId: id
                        }
                    };

                    employee.Reviewers.push(reviewer);
                });
            } else {
                angular.forEach(employeeOptions.reviewer.ids, function (id) {
                    var reviewer = {
                        ProcessId: vm.evaluatorType.Reviewer,
                        EvaluatorInfo: {
                            EmployeeId: id
                        }
                    };

                    employee.Reviewers.push(reviewer);
                });
            }
        }

        return employee;
    }

    function updateStartDateToEmployee(employee, employeeOptions) {
        angular.forEach(employee.HumanResources, function (humanResource) {
            humanResource.StartDate = employeeOptions.humanResource.StartDate;
        });

        angular.forEach(employee.SelfEvaluators, function (self) {
            self.StartDate = employeeOptions.selfEvaluator.StartDate;
        });

        angular.forEach(employee.Evaluators, function (evaluator) {
            evaluator.StartDate = employeeOptions.evaluator.StartDate;
        });

        angular.forEach(employee.Reviewers, function (reviewer) {
            reviewer.StartDate = employeeOptions.reviewer.StartDate;
        });

        return employee;
    }

    function updateEndDateToEmployee(employee, employeeOptions) {
        angular.forEach(employee.HumanResources, function (humanResource) {
            humanResource.EndDate = employeeOptions.humanResource.EndDate;
        });

        angular.forEach(employee.SelfEvaluators, function (self) {
            self.EndDate = employeeOptions.selfEvaluator.EndDate;
        });

        angular.forEach(employee.Evaluators, function (evaluator) {
            evaluator.EndDate = employeeOptions.evaluator.EndDate;
        });

        angular.forEach(employee.Reviewers, function (reviewer) {
            reviewer.EndDate = employeeOptions.reviewer.EndDate;
        });

        return employee;
    }

    function updateEmployeeSettings(employeeOptions, fieldEdit, selectedEmployees) {
        angular.forEach(selectedEmployees, function (employee) {
            if (fieldEdit.evaluator) {
                employee = updateEvaluatorToEmployee(employee, employeeOptions);
            }

            if (fieldEdit.startDate) {
                employee = updateStartDateToEmployee(employee, employeeOptions);
            }

            if (fieldEdit.endDate) {
                employee = updateEndDateToEmployee(employee, employeeOptions);
            }
        });

        return selectedEmployees;
    }

    vm.save = function () {
        var employeeAssignmentSetting;

        if (vm.singleMode) {
            if (MonthlyPeriodModel.validateEmployeeSetting(vm.employeeOptions)) {
                if (checkChangeEmployeeSetting(vm.employeeOptions, vm.selectedEmployees[0])) {
                    employeeAssignmentSetting = createAssignmentSettings(vm.employeeOptions, vm.period.PeriodId, vm.selectedEmployees, vm.selectedEmployeeIds);

                    MonthlyPeriodModel.updateEmployeeSettingOfPeriod(employeeAssignmentSetting).then(function () {
                        $modalInstance.close();
                    });
                }
                else {
                    $modalInstance.dismiss("cancel");
                }
            }
        }
        else {
            if (MonthlyPeriodModel.checkHasFieldEdit(vm.fieldEdit)) {
                var updatedEmployees = updateEmployeeSettings(vm.employeeOptions, vm.fieldEdit, vm.selectedEmployees);

                if (MonthlyPeriodModel.validateEmployeeSettingMultipleMode(updatedEmployees)) {
                    employeeAssignmentSetting = createAssignmentSettingsMultipleMode(updatedEmployees);

                    MonthlyPeriodModel.updateEmployeeSettingOfPeriod(employeeAssignmentSetting).then(function () {
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