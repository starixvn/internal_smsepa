(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('periodModule').controller('UpdateEmployeeSettingPeriodController', updateEmployeeSettingPeriodController);
    updateEmployeeSettingPeriodController.$inject = ['$scope', '$uibModalInstance', 'PeriodModel', 'CommonModel',
        'utilitiesFactory', 'period', 'selectedEmployees', 'permission'];

    function updateEmployeeSettingPeriodController($scope, $uibModalInstance, PeriodModel, CommonModel,
        utilitiesFactory, period, selectedEmployees, permission) {
        var vm = this,
            date = new Date();

        vm.period = period;
        vm.selectedEmployees = selectedEmployees;
        vm.singleMode = vm.selectedEmployees.length === 1 ? true : false;
        vm.isEditOnEPAHasMonthly = vm.selectedEmployees[0].Employee.HasMonthlyEPA && vm.period.IsSemiAnnual;

        vm.permission = permission;
        vm.periodDateFormat = PeriodModel.periodDateFormat;
        vm.evaluatorType = PeriodModel.evaluatorType;
        vm.policyEnum = utilitiesFactory.policy;

        var employeeInfo = PeriodModel.getSelectedEmployeeInfo(vm.selectedEmployees);
        vm.selectedEmployeeIds = employeeInfo.selectedEmployeeIds;
        vm.selectedEmployeeNames = employeeInfo.selectedEmployeeNames;

        vm.flowCalendar = PeriodModel.flowCalendar;
        vm.hasSelfStarted = PeriodModel.hasSelfStarted;
        vm.disciplineCalendar = {
            startDateOpened: false,
            endDateOpened: false
        };

        vm.fieldEdit = {
            targetTemplate: false,
            evaluator: false,
            startDate: false,
            endDate: false,
            disciplineStartDate: false,
            disciplineEndDate: false
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
            managementEmployees: [],
            disciplineStartDate: null,
            disciplineEndDate: null
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
            vm.employeeOptions.management = getEvaluators(employeeSetting.Managements);

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
        }

        CommonModel.getTargetTemplate(vm.period.TargetType.TargetTypeId, true).then(function () {
            vm.targetTemplates = CommonModel.targetTemplates;
            vm.employeeOptions.targetTemplateId = vm.targetTemplates.length > 0 ? vm.targetTemplates[0].TargetTemplateId : null;
            vm.employeeOptions.disciplineStartDate = vm.selectedEmployees[0].DisciplineStartDate ? new Date(vm.selectedEmployees[0].DisciplineStartDate) : null;
            vm.employeeOptions.disciplineEndDate = vm.selectedEmployees[0].DisciplineEndDate ? new Date(vm.selectedEmployees[0].DisciplineEndDate) : null;

            CommonModel.getAllEmployee().then(function () {
                vm.employees = CommonModel.employees;
                vm.employeeOptions.humanResourceEmployees = angular.copy(vm.employees);
                vm.employeeOptions.evaluatorEmployees = angular.copy(vm.employees);
                vm.employeeOptions.reviewerEmployees = angular.copy(vm.employees);
                vm.employeeOptions.managementEmployees = angular.copy(vm.employees);

                if (vm.singleMode) {
                    setEmployeeSetting(vm.selectedEmployees[0]);
                } else {
                    getProcessAndSetDateSetting();
                }
            });
        });

        vm.updateTrigger = function () {
            $scope.$broadcast('updatedChosen');
        };

        vm.openFlowPopupStartDate = function (event, evaluatorType) {
            PeriodModel.openFlowPopupStartDate(vm, event, evaluatorType);
        };

        vm.openFlowPopupEndDate = function (event, evaluatorType) {
            PeriodModel.openFlowPopupEndDate(vm, event, evaluatorType);
        };

        vm.openPopupDisciplineStartDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            vm.disciplineCalendar.startDateOpened = true;
            vm.disciplineCalendar.endDateOpened = false;
        };

        vm.openPopupDisciplineEndDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            vm.disciplineCalendar.startDateOpened = false;
            vm.disciplineCalendar.endDateOpened = true;
        };

        PeriodModel.checkSelfProcessStarted(vm.period.PeriodId, vm.selectedEmployeeIds).then(function () {
            vm.hasSelfStarted = PeriodModel.hasSelfStarted;
        });

        function checkChangeEmployeeSetting(employeeOptions, employeeSetting) {
            var hasEdited = false,
                humanResource = getEvaluators(employeeSetting.HumanResources),
                selfEvaluator = getEvaluators(employeeSetting.SelfEvaluators),
                evaluator = getEvaluators(employeeSetting.Evaluators),
                reviewer = getEvaluators(employeeSetting.Reviewers),
                management = getEvaluators(employeeSetting.Managements),
                originalTargetTemplateId = employeeSetting.TargetTemplate ? employeeSetting.TargetTemplate.TargetTemplateId : null,
                originalDisciplineStartDate = employeeSetting.DisciplineStartDate ? new Date(employeeSetting.DisciplineStartDate).setHours(0, 0, 0, 0) : null,
                originalDisciplineEndDate = employeeSetting.DisciplineEndDate ? new Date(employeeSetting.DisciplineEndDate).setHours(0, 0, 0, 0) : null,
                disciplineStartDate = employeeOptions.disciplineStartDate ? new Date(employeeOptions.disciplineStartDate).setHours(0, 0, 0, 0) : null,
                disciplineEndDate = employeeOptions.disciplineEndDate ? new Date(employeeOptions.disciplineEndDate).setHours(0, 0, 0, 0) : null;

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
            } else if (JSON.stringify(management) !== JSON.stringify(employeeOptions.management)) {
                hasEdited = true;
            } else if (originalDisciplineStartDate !== disciplineStartDate || originalDisciplineEndDate !== disciplineEndDate) {
                hasEdited = true;
            }

            return hasEdited;
        }

        vm.save = function () {
            var employeeAssignmentSetting;

            if (vm.isEditOnEPAHasMonthly) {
                vm.employeeOptions.selfEvaluator.StartDate = vm.employeeOptions.humanResource.EndDate;
                vm.employeeOptions.selfEvaluator.EndDate = vm.employeeOptions.humanResource.EndDate;
                vm.employeeOptions.evaluator.StartDate = vm.employeeOptions.humanResource.EndDate;
                vm.employeeOptions.evaluator.EndDate = vm.employeeOptions.humanResource.EndDate;
            }

            if (vm.singleMode) {
                if (PeriodModel.validateEmployeeSetting(vm.employeeOptions)) {
                    if (checkChangeEmployeeSetting(vm.employeeOptions, vm.selectedEmployees[0])) {
                        employeeAssignmentSetting = PeriodModel.createAssignmentSettings(
                            vm.employeeOptions,
                            vm.period.PeriodId,
                            vm.selectedEmployees,
                            vm.selectedEmployeeIds);

                        PeriodModel.updateEmployeeSettingOfPeriod(employeeAssignmentSetting).then(function () {
                            $uibModalInstance.close();
                        });
                    } else {
                        $uibModalInstance.dismiss("cancel");
                    }
                }
            } else if (PeriodModel.checkHasFieldEdit(vm.fieldEdit)) {
                var updatedEmployees = PeriodModel.updateEmployeeSettings(vm.employeeOptions, vm.fieldEdit, vm.selectedEmployees);

                if (PeriodModel.validateEmployeeSettingMultipleMode(updatedEmployees)) {
                    employeeAssignmentSetting = PeriodModel.createAssignmentSettingsMultipleMode(updatedEmployees, vm);

                    PeriodModel.updateEmployeeSettingOfPeriod(employeeAssignmentSetting).then(function () {
                        $uibModalInstance.close();
                    });
                }
            }
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss("cancel");
        };
    }
})(angular);