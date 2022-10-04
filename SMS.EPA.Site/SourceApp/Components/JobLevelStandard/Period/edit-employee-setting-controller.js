(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('EditEmployeeSettingController', editEmployeeSettingController);
    editEmployeeSettingController.$inject = ['$scope', '$uibModalInstance', 'JobLevelStandardPeriodModel', 'CommonModel', 'utilitiesFactory', 'employees'];

    function editEmployeeSettingController($scope, $uibModalInstance, PeriodModel, CommonModel, utilitiesFactory, selectedEmployees) {
        let vm = this,
            date = new Date();
        vm.singleMode = selectedEmployees.length === 1;
        vm.employeeInfo = '';

        vm.fieldEdit = {
            evaluator: false,
            date: false
        };

        vm.process = utilitiesFactory.jobLevelAssessmentProcess;
        vm.calendar = angular.copy(PeriodModel.calendar);

        vm.openFromDate = function (event, processId) {
            PeriodModel.openFromDate(event, vm.calendar, processId);
        };

        vm.openToDate = function (event, processId) {
            PeriodModel.openToDate(event, vm.calendar, processId);
        };

        vm.employeeSettings = {
            evaluator: {
                ids: null,
                fromDate: null,
                toDate: null
            },
            employee: {
                ids: null,
                fromDate: null,
                toDate: null
            },
            management: {
                ids: null,
                fromDate: null,
                toDate: null
            },
            evaluatorOptions: [],
            managementOptions: []
        };

        function getEvaluators(evaluators) {
            const evaluator = {
                fromDate: date,
                toDate: date,
                ids: [],
                processId: 0
            };

            if (evaluators && evaluators.length > 0) {
                evaluator.fromDate = new Date(evaluators[0].FromDate);
                evaluator.toDate = new Date(evaluators[0].ToDate);
                angular.forEach(evaluators, function (item) {
                    evaluator.ids.push(item.EvaluatorId);
                });
                evaluator.processId = evaluators[0].ProcessId;
            }

            return evaluator;
        }

        function initialize() {
            for (const employee of selectedEmployees) {
                const employeeInfo = employee.FullName + ' (' + employee.Nickname + ')';
                vm.employeeInfo += vm.employeeInfo ? ', ' + employeeInfo : employeeInfo;
            }

            CommonModel.searchEmployee().then(function () {
                vm.employees = CommonModel.employees;
                vm.employeeSettings.evaluatorOptions = angular.copy(vm.employees);
                vm.employeeSettings.managementOptions = angular.copy(vm.employees);

                vm.employeeSettings.employee = getEvaluators([selectedEmployees[0].Employee]);
                vm.employeeSettings.evaluator = getEvaluators(selectedEmployees[0].Evaluators);
                vm.employeeSettings.management = getEvaluators(selectedEmployees[0].Managements);
            });
        }

        vm.updateTrigger = function () {
            $scope.$broadcast('updatedChosen');
        };

        vm.openFlowPopupStartDate = function (event, evaluatorType) {
            PeriodModel.openFlowPopupStartDate(vm, event, evaluatorType);
        };

        vm.openFlowPopupEndDate = function (event, evaluatorType) {
            PeriodModel.openFlowPopupEndDate(vm, event, evaluatorType);
        };

        function getDate(oldValue, newValue) {
            return new Date(vm.singleMode || vm.fieldEdit.date ? newValue : oldValue).toDateString();
        }

        vm.save = function () {
            if (PeriodModel.validateEditEmployeeSetting(vm.singleMode, vm.fieldEdit, vm.employeeSettings)) {
                const employeeSettings = [];
                for (const employee of selectedEmployees) {
                    const employeeSetting = {
                        periodId: employee.PeriodId,
                        employee: {
                            processId: vm.process.Employee,
                            employeeId: employee.EmployeeId,
                            evaluatorId: employee.EmployeeId,
                            fromDate: getDate(employee.Employee.FromDate, vm.employeeSettings.employee.fromDate),
                            toDate: getDate(employee.Employee.ToDate, vm.employeeSettings.employee.toDate)
                        },
                        evaluators: [],
                        managements: []
                    };
                    if (vm.fieldEdit.evaluator) {
                        angular.forEach(vm.employeeSettings.evaluator.ids, function (id) {
                            employeeSetting.evaluators.push({
                                processId: vm.process.Evaluator,
                                employeeId: employee.EmployeeId,
                                evaluatorId: id,
                                fromDate: getDate(employee.Evaluators[0].FromDate, vm.employeeSettings.evaluator.fromDate),
                                toDate: getDate(employee.Evaluators[0].ToDate, vm.employeeSettings.evaluator.toDate)
                            });
                        });
                        angular.forEach(vm.employeeSettings.management.ids, function (id) {
                            employeeSetting.managements.push({
                                processId: vm.process.Management,
                                employeeId: employee.EmployeeId,
                                evaluatorId: id,
                                fromDate: getDate(employee.Managements[0].FromDate, vm.employeeSettings.management.fromDate),
                                toDate: getDate(employee.Managements[0].ToDate, vm.employeeSettings.management.toDate)
                            });
                        });
                    } else {
                        angular.forEach(employee.Evaluators, function (evaluator) {
                            employeeSetting.evaluators.push({
                                processId: vm.process.Evaluator,
                                employeeId: employee.EmployeeId,
                                evaluatorId: evaluator.EvaluatorId,
                                fromDate: getDate(evaluator.FromDate, vm.employeeSettings.evaluator.fromDate),
                                toDate: getDate(evaluator.ToDate, vm.employeeSettings.evaluator.toDate)
                            });
                        });
                        angular.forEach(employee.Managements, function (management) {
                            employeeSetting.managements.push({
                                processId: vm.process.Management,
                                employeeId: employee.EmployeeId,
                                evaluatorId: management.EvaluatorId,
                                fromDate: getDate(management.FromDate, vm.employeeSettings.management.fromDate),
                                toDate: getDate(management.ToDate, vm.employeeSettings.management.toDate)
                            });
                        });
                    }

                    employeeSettings.push(employeeSetting);
                }

                PeriodModel.editEmployeeSetting(employeeSettings).then(function () {
                    $uibModalInstance.close();
                });
            }
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

        initialize();
    }
})(angular);