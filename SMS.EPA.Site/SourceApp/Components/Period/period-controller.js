'use strict';
/*global periodModule*/

periodModule.controller('PeriodController',
['PeriodModel', 'CommonModel', 'utilitiesFactory', 'baseUrl',
function (PeriodModel, CommonModel, utilitiesFactory, baseUrl) {
    var vm = this;

    vm.periods = PeriodModel.periods;
    vm.targetTypes = CommonModel.targetTypes;
    vm.periodDateFormat = PeriodModel.periodDateFormat;

    vm.filter = {
        targetTypeId: null,
        isActive: 'true'
    };

    vm.currentPage = 1;
    vm.pageSize = 20;

    utilitiesFactory.numberEntryList(vm);

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
}]);

periodModule.controller('CreatePeriodController',
['$modalInstance', 'PeriodModel', 'CommonModel',
function ($modalInstance, PeriodModel, CommonModel) {
    var vm = this;

    vm.targetTypes = CommonModel.targetTypes;
    vm.period = {
        Name: null,
        TargetTypeId: null,
        RawStartDate: new Date(),
        StartDate: null,
        IsActive: true,
        Description: null
    };

    vm.calendar = PeriodModel.calendar;
    vm.openPopupStartDate = function (event) {
        PeriodModel.openPopupStartDate(event);
    };

    vm.save = function (period) {
        if (PeriodModel.validatePeriod(period)) {
            vm.period.StartDate = vm.period.RawStartDate.getDate() + ' ' + PeriodModel.getShortNameMonth(vm.period.RawStartDate.getMonth());
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

    vm.period = {};
    vm.periodId = parseInt($routeParams.id);
    vm.date = new Date();

    vm.dataTab = {
        basicInformation: true,
        employeeSetting: false
    };

    vm.tabBasicSelect = function () {
        vm.dataTab.basicInformation = true;
        vm.dataTab.employeeSetting = false;
    };

    vm.employeeSetting = function () {
        vm.dataTab.basicInformation = false;
        vm.dataTab.employeeSetting = true;
    };

    PeriodModel.getPeriodById(vm.periodId).then(function () {
        vm.period = PeriodModel.period;
        vm.period.TargetTypeId = vm.period.TargetType.TargetTypeId;
        var startDate = vm.period.StartDate + ', ' + vm.date.getFullYear();
        vm.period.RawStartDate = new Date(startDate);
    });

    vm.backToList = function () {
        $window.history.back();
    };
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

    $scope.$watch(function () { return $scope.updateCtrl.period; },
        function (newVal) {
            if (newVal) {
                vm.period = $scope.updateCtrl.period;
            }
        });

    $scope.$watch(function () { return $scope.updateCtrl.dataTab.basicInformation; },
        function (newVal) {
            if (newVal && vm.isFirstLoad) {
                CommonModel.getTargetType().then(function () {
                    vm.targetTypes = CommonModel.targetTypes;
                    vm.isFirstLoad = false;
                });
            }
        });

    vm.save = function (period) {
        if (PeriodModel.validatePeriod(period)) {
            vm.period.StartDate = vm.period.RawStartDate.getDate() + ' ' + PeriodModel.getShortNameMonth(vm.period.RawStartDate.getMonth());
            PeriodModel.updateBasicInformationPeriod(period);
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };
}]);

periodModule.controller('EmployeeSettingPeriodController',
['$scope', '$window', 'PeriodModel', 'utilitiesFactory', 'CommonModel', 'EmployeeModel',
function ($scope, $window, PeriodModel, utilitiesFactory, CommonModel, EmployeeModel) {
    var vm = this;

    vm.period = {};
    vm.employeeSettings = PeriodModel.employeeSettings;
    vm.departments = CommonModel.departments;
    vm.isFirstLoad = true;
    vm.periodDateFormat = PeriodModel.periodDateFormat;

    vm.currentPage = 1;
    vm.pageSize = 15;
    utilitiesFactory.numberEntryList(vm);

    vm.filter = {
        employee: ''
    };

    $scope.$watch(function () { return $scope.updateCtrl.period; },
    function (newVal) {
        if (newVal) {
            vm.period = $scope.updateCtrl.period;
        }
    });

    $scope.$watch(function () { return $scope.updateCtrl.dataTab.employeeSetting; },
        function (newVal) {
            if (newVal && vm.isFirstLoad) {
                CommonModel.getDepartment(null, true).then(function () {
                    vm.departments = CommonModel.departments;
                });

                PeriodModel.getPeriodEmployeeSettingById(vm.period.PeriodId, vm.period.StartDate).then(function () {
                    vm.employeeSettings = PeriodModel.employeeSettings;
                    utilitiesFactory.searchOnPage(vm, vm.employeeSettings);
                    vm.isFirstLoad = false;
                });
            }
        });

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
            if (vm.filter.employee) {
                vm.filter.employee = null;
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

    vm.addEmployee = function (filter) {
        var employeeId = null;

        if (filter.employee) {
            employeeId = filter.employee.EmployeeId;
        }

        var employeeInfo = {
            periodId: vm.period.PeriodId,
            employeeId: employeeId,
            departments: getSelectedDepartment(vm.departments)
        };

        PeriodModel.addEmployeeToPeriod(employeeInfo);
    };

    vm.updateEmployeeSetting = function (employeeSetting) {
        PeriodModel.openPopupUpdateEmployeeSetting(employeeSetting);
    };
}]);

periodModule.controller('UpdateEmployeeSettingPeriodController',
['$scope', '$modalInstance', 'employeeSetting', 'PeriodModel', 'EmployeeModel', 'TargetTemplateModel', 'FlowMngModel',
function ($scope, $modalInstance, employeeSetting, PeriodModel, EmployeeModel, TargetTemplateModel, FlowMngModel) {
    var vm = this;

    vm.employeeSetting = employeeSetting;
    vm.periodDateFormat = PeriodModel.periodDateFormat;
    vm.remindDay = FlowMngModel.remindDay;

    vm.employeeOptions = {
        selftEvaluator: {
            ids: null,
            StartDate: null,
            EndDate: null,
            HasSendMail: true,
            RemindDay: 0,
            Duration: 0,
            originalDuration: 0
        },
        evaluator: {
            ids: null,
            StartDate: null,
            EndDate: null,
            HasSendMail: true,
            RemindDay: 0,
            Duration: 0,
            originalDuration: 0
        },
        reviewer: {
            ids: null,
            StartDate: null,
            EndDate: null,
            HasSendMail: true,
            RemindDay: 0,
            Duration: 0,
            originalDuration: 0
        },
        management: {
            ids: null,
            StartDate: null,
            EndDate: null,
            HasSendMail: true,
            RemindDay: 0,
            Duration: 0,
            originalDuration: 0
        },
        targetTemplateId: null,
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
            StartDate: '',
            EndDate: '',
            HasSendMail: true,
            RemindDay: 0,
            Duration: 0,
            originalDuration: 0,
            ids: [],
            ProcessId: 0
        };

        if (evaluators && evaluators.length > 0) {
            evaluator.StartDate = evaluators[0].StartDate ? new Date(evaluators[0].StartDate) : null;
            evaluator.EndDate = evaluators[0].EndDate ? new Date(evaluators[0].EndDate) : null;
            evaluator.HasSendMail = evaluators[0].HasSendMail;
            evaluator.RemindDay = evaluators[0].RemindDay;
            evaluator.Duration = evaluators[0].Duration;
            evaluator.originalDuration = evaluator.Duration;
            evaluator.ids = createEmployeeIds(evaluators);
            evaluator.ProcessId = evaluators[0].ProcessId;
        }

        return evaluator;
    };

    var setEmployeeSetting = function () {
        vm.employeeOptions.selfEvaluator = getEvaluators(vm.employeeSetting.SelfEvaluators);
        vm.employeeOptions.evaluator = getEvaluators(vm.employeeSetting.Evaluators);
        vm.employeeOptions.reviewer = getEvaluators(vm.employeeSetting.Reviewers);
        vm.employeeOptions.management = getEvaluators(vm.employeeSetting.Managements);
        vm.employeeOptions.targetTemplateId = vm.employeeSetting.TargetTemplate.TargetTemplateId;
    };

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

        setEmployeeSetting();
    });

    vm.updateTrigger = function () {
        $scope.$broadcast('evaluatorChosen');
    };

    vm.updateDate = function (evaluator) {
        if (evaluator && evaluator.ids.length > 0) {
            var duration = evaluator.Duration,
                originalDuration = evaluator.originalDuration,
                changeDuration = duration - originalDuration,
                processId = evaluator.ProcessId;

            evaluator.EndDate = new Date(evaluator.EndDate.setDate(evaluator.EndDate.getDate() + changeDuration));

            if (vm.employeeOptions.evaluator.ids.length > 0 && vm.employeeOptions.evaluator.ProcessId > processId) {
                var evaluatorStartDate = vm.employeeOptions.evaluator.StartDate,
                    evaluatorEndDate = vm.employeeOptions.evaluator.EndDate;

                vm.employeeOptions.evaluator.StartDate = new Date(evaluatorStartDate.setDate(evaluatorStartDate.getDate() + changeDuration));
                vm.employeeOptions.evaluator.EndDate = new Date(evaluatorEndDate.setDate(evaluatorEndDate.getDate() + changeDuration));
            }

            if (vm.employeeOptions.reviewer.ids.length > 0 && vm.employeeOptions.reviewer.ProcessId > processId) {
                var reviewerStartDate = vm.employeeOptions.reviewer.StartDate,
                    reviewerEndDate = vm.employeeOptions.reviewer.EndDate;

                vm.employeeOptions.reviewer.StartDate = new Date(reviewerStartDate.setDate(reviewerStartDate.getDate() + changeDuration));
                vm.employeeOptions.reviewer.EndDate = new Date(reviewerEndDate.setDate(reviewerEndDate.getDate() + changeDuration));
            }

            if (vm.employeeOptions.management.ids.length > 0 && vm.employeeOptions.management.ProcessId > processId) {
                var managementStartDate = vm.employeeOptions.management.StartDate,
                    managementEndDate = vm.employeeOptions.management.EndDate;

                vm.employeeOptions.management.StartDate = new Date(managementStartDate.setDate(managementStartDate.getDate() + changeDuration));
                vm.employeeOptions.management.EndDate = new Date(managementEndDate.setDate(managementEndDate.getDate() + changeDuration));
            }

            evaluator.originalDuration = duration;
        }
    };

    vm.save = function () {
        var employeeAssignmentSetting = PeriodModel.createAssignmentSettings(vm.employeeOptions, vm.employeeSetting);

        PeriodModel.updateEmployeeSettingOfPeriod(employeeAssignmentSetting).then(function () {
            $modalInstance.close();
        });
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
}]);
