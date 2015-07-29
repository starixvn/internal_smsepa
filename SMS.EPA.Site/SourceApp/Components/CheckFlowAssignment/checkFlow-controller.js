'use strict';
/*global checkFlowAssignmentModule*/

checkFlowAssignmentModule.controller('CheckFlowAssignmentController',
['$scope', 'CheckFlowModel', 'EmployeeModel', 'CommonModel', 'utilitiesFactory', 'TargetTemplateModel',
function ($scope, CheckFlowModel, EmployeeModel, CommonModel, utilitiesFactory, TargetTemplateModel) {
    var vm = this;

    /*
    ** Variable setting.
    */
    vm.employeeSettings = CheckFlowModel.employeeSettings;

    vm.filter = {
        groupId: null,
        employeeId: null,
        targetTypeId: null,
        employee: null
    };

    vm.selectedAll = false;
    vm.groupOptions = [];
    vm.currentPage = 1;
    vm.pageSize = 15;

    /*
    ** Get data to filter and setting information.
    */
    utilitiesFactory.numberEntryList(vm);
    utilitiesFactory.getDateFormat(vm);

    EmployeeModel.getAllGroup().then(function () {
        vm.groupOptions = EmployeeModel.groups;
    });

    CommonModel.getTargetType().then(function () {
        vm.targetTypes = CommonModel.targetTypes;
    });

    TargetTemplateModel.getTargetTemplate(null, true).then(function () {
        vm.targetTemplates = TargetTemplateModel.targetTemplates;
    });

    /*
    ** Search employee auto complete.
    */
    vm.getEmployee = function () {
        return EmployeeModel.getAllEmployee(vm.searchEmployee);
    };

    vm.unSelectEmployee = function () {
        if (vm.searchEmployee.trim() === '') {
            if (vm.filter.employee) {
                vm.filter.employee = null;
            }
        }
    };

    /*
    ** Get employees flow.
    */
    var getEmployeeSettings = function (groupId, employeeId, targetTypeId) {
        CheckFlowModel.getEmployeeFlowTargetSetting(groupId, employeeId, targetTypeId).then(function () {
            vm.employeeSettings = CheckFlowModel.employeeSettings;
            utilitiesFactory.searchOnPage(vm, vm.employeeSettings);
        });
    };

    getEmployeeSettings(vm.filter.groupId, vm.filter.employeeId, vm.filter.targetTypeId);

    vm.query = function (filter) {
        var employeeId = null;

        if (filter.employee) {
            employeeId = filter.employee.EmployeeId;
        }

        getEmployeeSettings(filter.groupId, employeeId, filter.targetTypeId);
    };

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
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
        angular.forEach(vm.employeeSettings, function (employeeSetting) {
            if (employeeSetting.EmployeeId === employeeId) {
                angular.forEach(vm.filteredItems, function (filterItem) {
                    if (employeeSetting.EmployeeId === filterItem.EmployeeId) {
                        filterItem.Selected = selected;
                        employeeSetting.Selected = selected;
                    }
                });
            }
        });
    };

    vm.manageSetting = function () {
        var selectedEmployees = CheckFlowModel.getSelectedEmployee(vm.employeeSettings);
        CheckFlowModel.openPopupEditEmployeeSetting(vm, selectedEmployees);
    };

    vm.editEmployeeSetting = function (employeeSetting) {
        CheckFlowModel.openPopupEditEmployeeSetting(vm, null, employeeSetting);
    };

    /*
    ** Export report.
    */
    vm.exportReport = function (filter) {
        var employeeId = null;

        if (filter.employee) {
            employeeId = filter.employee.EmployeeId;
        }

        vm.exportCriteria = {
            groupId: filter.groupId,
            employeeId: employeeId,
            targetTypeId: filter.targetTypeId
        };

        $scope.$broadcast('exportCheckAssignmentReport');
    };
}]);

checkFlowAssignmentModule.controller('EditEmployeeSettingController',
['$scope', '$modalInstance', 'selectedEmployees', 'employeeSetting', 'CheckFlowModel', 'EmployeeModel', 'TargetTemplateModel', 'utilitiesFactory',
function ($scope, $modalInstance, selectedEmployees, employeeSetting, CheckFlowModel, EmployeeModel, TargetTemplateModel, utilitiesFactory) {
    var vm = this;

    vm.selectedEmployees = selectedEmployees;
    vm.employeeSetting = employeeSetting;
    vm.employeeOptions = angular.copy(CheckFlowModel.filterAssign);

    var createEmployeeIds = function (evaluators) {
        var ids = [];

        angular.forEach(evaluators, function (evaluator) {
            ids.push(evaluator.EvaluatorInfo.EmployeeId);
        });

        return ids;
    };

    var getEvaluators = function (evaluators) {
        var evaluator = {
            startDate: CheckFlowModel.date,
            endDate: CheckFlowModel.date,
            hasSendMail: true,
            ids: []
        };

        if (evaluators && evaluators.length > 0) {
            evaluator.startDate = new Date(evaluators[0].StartDate);
            evaluator.endDate = new Date(evaluators[0].EndDate);
            evaluator.hasSendMail = evaluators[0].HasSendMail;
            evaluator.ids = createEmployeeIds(evaluators);
        }

        return evaluator;
    };

    var setEmployeeSetting = function () {
        vm.employeeOptions.evaluator = getEvaluators(vm.employeeSetting.Evaluators);
        vm.employeeOptions.reviewer = getEvaluators(vm.employeeSetting.Reviewers);
        vm.employeeOptions.management = getEvaluators(vm.employeeSetting.Managements);
        vm.employeeOptions.targetTemplateId = vm.employeeSetting.TargetTemplate.TargetTemplateId;
    }

    /*
    ** Get employees flow and target setting.
    */
    TargetTemplateModel.getTargetTemplate(null, true).then(function () {
        vm.targetTemplates = TargetTemplateModel.targetTemplates;
    });

    EmployeeModel.getAllEmployee().then(function () {
        vm.employees = EmployeeModel.employees;
        vm.employeeOptions.evaluatorEmployees = angular.copy(vm.employees);
        vm.employeeOptions.reviewerEmployees = angular.copy(vm.employees);
        vm.employeeOptions.managementEmployees = angular.copy(vm.employees);

        if (!vm.selectedEmployees) {
            setEmployeeSetting();
        }
    });

    vm.updateTrigger = function () {
        $scope.$broadcast('evaluatorChosen');
    };

    /*
    ** Calendar setting.
    */
    utilitiesFactory.getDateFormat(vm);
    vm.calendar = CheckFlowModel.calendar;
    vm.evaluatorType = CheckFlowModel.evaluatorType;

    vm.openPopupStartDate = function (event, evaluatorType) {
        CheckFlowModel.openPopupStartDate(vm, event, evaluatorType);
    };

    vm.openPopupEndDate = function (event, evaluatorType) {
        CheckFlowModel.openPopupEndDate(vm, event, evaluatorType);
    };

    var getUpdateTargetTemplate = function (targetTemplateId, targetTemplates) {
        var targetTemplate = '';

        for (var i = 0; i < targetTemplates.length; i++) {
            var item = targetTemplates[i];

            if (item.TargetTemplateId === targetTemplateId) {
                targetTemplate = item;
                i = targetTemplates.length;
            }
        }

        return targetTemplate;
    };

    var getEmployeeById = function (employees, employeeId) {
        var employee = null;

        for (var i = 0; i < employees.length; i++) {
            var employeeItem = employees[i];

            if (employeeItem.EmployeeId === employeeId) {
                employee = employeeItem;
            };
        };

        return employee;
    };

    var getUpdateEvaluators = function (evaluator, evaluatorType) {
        var evaluators = [];

        if (evaluator && evaluator.ids != null && evaluator.ids.length > 0) {
            angular.forEach(evaluator.ids, function (id) {
                var employee = getEmployeeById(vm.employees, id),
                    evaluatorItem = {
                        ProcessId: evaluatorType,
                        StartDate: evaluator.startDate,
                        EndDate: evaluator.endDate,
                        HasSendMail: evaluator.hasSendMail,
                        EvaluatorInfo: employee ? employee : {
                            EmployeeId: -1
                        }
                    };

                evaluators.push(evaluatorItem);
            });
        }
        else {
            var evaluatorItem = {
                ProcessId: evaluatorType,
                StartDate: evaluator.startDate,
                EndDate: evaluator.endDate,
                HasSendMail: evaluator.hasSendMail,
                EvaluatorInfo: {
                    EmployeeId: -1
                }
            };
            evaluators.push(evaluatorItem);
        }

        return evaluators;
    };

    var updateEmployeeSetting = function (employeeSetting, employeeOptions) {
        employeeSetting.TargetTemplate = getUpdateTargetTemplate(employeeOptions.targetTemplateId, vm.targetTemplates);
        employeeSetting.Evaluators = getUpdateEvaluators(employeeOptions.evaluator, vm.evaluatorType.Evaluator);
        employeeSetting.Reviewers = getUpdateEvaluators(employeeOptions.reviewer, vm.evaluatorType.Reviewer);
        employeeSetting.Managements = getUpdateEvaluators(employeeOptions.management, vm.evaluatorType.Management);
    };

    vm.save = function () {
        var selectedEmployees = null;

        if (vm.selectedEmployees) {
            selectedEmployees = vm.selectedEmployees;
            var employeeAssignmentSettings = CheckFlowModel.createAssignmentSettings(vm.employeeOptions, selectedEmployees);
        }
        else {
            selectedEmployees = [vm.employeeSetting.EmployeeId];
        }

        var employeeAssignmentSettings = CheckFlowModel.createAssignmentSettings(vm.employeeOptions, selectedEmployees);

        if (CheckFlowModel.validateCheckAssignments(selectedEmployees, employeeAssignmentSettings, vm.employeeOptions)) {
            CheckFlowModel.updateEmployeeFlowTargetSetting(employeeAssignmentSettings).then(function () {
                if (vm.selectedEmployees) {
                    $modalInstance.close(null);
                }
                else {
                    if (CheckFlowModel.uncheckPersonalSetting(vm.employeeOptions)) {
                        CheckFlowModel.getEmployeeFlowTargetSetting(null, vm.employeeSetting.EmployeeId, null).then(function () {
                            vm.employeeSettings = CheckFlowModel.employeeSettings;
                            $modalInstance.close(vm.employeeSettings[0]);
                        });
                    }
                    else {
                        updateEmployeeSetting(vm.employeeSetting, vm.employeeOptions);
                        $modalInstance.close(vm.employeeSetting);
                    }
                }

                CheckFlowModel.resetFilterAssign(vm);
            });
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
}]);