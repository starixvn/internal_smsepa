(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('EditEmployeeSettingController', editEmployeeSettingController);
    editEmployeeSettingController.$inject = ['$scope', '$uibModalInstance', 'JobLevelStandardPeriodModel', 'CommonModel', 'utilitiesFactory', 'employee'];

    function editEmployeeSettingController($scope, $uibModalInstance, PeriodModel, CommonModel, utilitiesFactory, selectedEmployee) {
        let vm = this,
            date = new Date();

        vm.employee = selectedEmployee.FullName + ' (' + selectedEmployee.Nickname + ')';

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
            CommonModel.searchEmployee().then(function () {
                vm.employees = CommonModel.employees;
                vm.employeeSettings.evaluatorOptions = angular.copy(vm.employees);
                vm.employeeSettings.managementOptions = angular.copy(vm.employees);

                vm.employeeSettings.employee = getEvaluators([selectedEmployee.Employee]);
                vm.employeeSettings.evaluator = getEvaluators(selectedEmployee.Evaluators);
                vm.employeeSettings.management = getEvaluators(selectedEmployee.Managements);
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

        function checkDataChanged() {
            let isEdited = false,
                employee = getEvaluators([selectedEmployee.Employee]),
                evaluator = getEvaluators(selectedEmployee.Evaluators),
                management = getEvaluators(selectedEmployee.Managements);

            if (JSON.stringify(employee) !== JSON.stringify(vm.employeeSettings.employee)
                || JSON.stringify(evaluator) !== JSON.stringify(vm.employeeSettings.evaluator)
                || JSON.stringify(management) !== JSON.stringify(vm.employeeSettings.management)) {
                isEdited = true;
            }

            return isEdited;
        }

        vm.save = function () {
            if (PeriodModel.validateEditEmployeeSetting(vm.employeeSettings)) {
                if (checkDataChanged()) {
                    const params = {
                        periodId: selectedEmployee.PeriodId,
                        employee: {
                            processId: vm.process.Employee,
                            employeeId: selectedEmployee.EmployeeId,
                            evaluatorId: selectedEmployee.EmployeeId,
                            fromDate: vm.employeeSettings.employee.fromDate.toDateString(),
                            toDate: vm.employeeSettings.employee.toDate.toDateString()
                        },
                        evaluators: [],
                        managements: []
                    };
                    angular.forEach(vm.employeeSettings.evaluator.ids, function (id) {
                        params.evaluators.push({
                            processId: vm.process.Evaluator,
                            employeeId: selectedEmployee.EmployeeId,
                            evaluatorId: id,
                            fromDate: vm.employeeSettings.evaluator.fromDate.toDateString(),
                            toDate: vm.employeeSettings.evaluator.toDate.toDateString()
                        });
                    });
                    angular.forEach(vm.employeeSettings.management.ids, function (id) {
                        params.managements.push({
                            processId: vm.process.Management,
                            employeeId: selectedEmployee.EmployeeId,
                            evaluatorId: id,
                            fromDate: vm.employeeSettings.management.fromDate.toDateString(),
                            toDate: vm.employeeSettings.management.toDate.toDateString()
                        });
                    });

                    PeriodModel.editEmployeeSetting(params).then(function () {
                        $uibModalInstance.close();
                    });
                } else {
                    $uibModalInstance.dismiss('cancel');
                }
            }
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

        initialize();
    }
})(angular);