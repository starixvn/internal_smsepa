'use strict';
/*global checkFlowAssignmentModule*/

checkFlowAssignmentModule.factory('CheckFlowModel',
['$modal', 'checkFlowFactory', 'errorFactory', 'notificationFactory',
function ($modal, checkFlowFactory, errorFactory, notificationFactory) {
    var checkFlowModel = {};

    checkFlowModel.employeeSettings = [];
    checkFlowModel.date = new Date();

    checkFlowModel.evaluatorType = {
        Evaluator: 2,
        Reviewer: 3,
        Management: 4
    };

    checkFlowModel.filterAssign = {
        evaluator: {
            ids: null,
            startDate: checkFlowModel.date,
            endDate: checkFlowModel.date,
            hasSendMail: true
        },
        reviewer: {
            ids: null,
            startDate: checkFlowModel.date,
            endDate: checkFlowModel.date,
            hasSendMail: true
        },
        management: {
            ids: null,
            startDate: checkFlowModel.date,
            endDate: checkFlowModel.date,
            hasSendMail: true
        },
        targetTemplateId: null,
        evaluatorEmployees: [],
        reviewerEmployees: [],
        managementEmployees: []
    };

    checkFlowModel.calendar = {
        evaluator: {
            startDateOpened: false,
            endDateOpened: false
        },
        reviewer: {
            startDateOpened: false,
            endDateOpened: false
        },
        management: {
            startDateOpened: false,
            endDateOpened: false
        }
    };

    checkFlowModel.getEmployeeFlowTargetSetting = function (groupId, employeeId, targetTypeId) {
        return checkFlowFactory.getEmployeeFlowTargetSetting(groupId, employeeId, targetTypeId).success(function (employeeSettings) {
            checkFlowModel.employeeSettings = employeeSettings;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    checkFlowModel.validateCheckAssignments = function (selectedEmployees, employeeAssignmentSettings, employeeOptions) {
        if (!employeeAssignmentSettings) {
            return false;
        }
        else if (selectedEmployees && selectedEmployees.length === 0) {
            notificationFactory.error("SelectEmployeeBeforeAssingFlowSettingWarning");
            return false;
        }

        if (employeeOptions.evaluator.startDate <= employeeOptions.evaluator.endDate
            && employeeOptions.reviewer.startDate <= employeeOptions.reviewer.endDate
            && employeeOptions.management.startDate <= employeeOptions.management.endDate) {
        }
        else {
            notificationFactory.error("StartDateGreaterThanEndDateWarning");
            return false;
        }

        if (employeeOptions.evaluator.endDate > employeeOptions.reviewer.startDate) {
            notificationFactory.error("EvaluatorRangeDateGreaterThanReviewerRangeDateWarning");
            return false;
        }
        else if (employeeOptions.reviewer.endDate > employeeOptions.management.startDate) {
            notificationFactory.error("ReviewerRangeDateGreaterThanManagementRangeDateWarning");
            return false;
        }

        return true;
    };

    checkFlowModel.updateEmployeeFlowTargetSetting = function (employeeAssignments) {
        return checkFlowFactory.updateEmployeeFlowTargetSetting(employeeAssignments).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
            }
            else {
                notificationFactory.error("", response.Message);
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    checkFlowModel.resetCalendar = function (vm) {
        vm.calendar.evaluator.startDateOpened = false;
        vm.calendar.evaluator.endDateOpened = false;

        vm.calendar.reviewer.startDateOpened = false;
        vm.calendar.reviewer.endDateOpened = false;

        vm.calendar.management.startDateOpened = false;
        vm.calendar.management.endDateOpened = false;
    };

    checkFlowModel.openPopupStartDate = function (vm, event, evaluatorType) {
        event.preventDefault();
        event.stopPropagation();
        checkFlowModel.resetCalendar(vm);

        switch (evaluatorType) {
            case checkFlowModel.evaluatorType.Evaluator:
                vm.calendar.evaluator.startDateOpened = true;
                break;
            case checkFlowModel.evaluatorType.Reviewer:
                vm.calendar.reviewer.startDateOpened = true;
                break;
            case checkFlowModel.evaluatorType.Management:
                vm.calendar.management.startDateOpened = true;
                break;
        }
    };

    checkFlowModel.openPopupEndDate = function (vm, event, evaluatorType) {
        event.preventDefault();
        event.stopPropagation();
        checkFlowModel.resetCalendar(vm);

        switch (evaluatorType) {
            case checkFlowModel.evaluatorType.Evaluator:
                vm.calendar.evaluator.endDateOpened = true;
                break;
            case checkFlowModel.evaluatorType.Reviewer:
                vm.calendar.reviewer.endDateOpened = true;
                break;
            case checkFlowModel.evaluatorType.Management:
                vm.calendar.management.endDateOpened = true;
                break;
        }
    };

    checkFlowModel.getSelectedEmployee = function (employeeSettings) {
        var selectedEmployees = [];

        angular.forEach(employeeSettings, function (employeeSetting) {
            if (employeeSetting.Selected) {
                selectedEmployees.push(employeeSetting.EmployeeId);
            }
        });

        return selectedEmployees;
    };

    checkFlowModel.createAssignmentSettings = function (filterAssign, selectedEmployees) {
        var employeeAssignments = [];

        angular.forEach(selectedEmployees, function (selectedEmployeeId) {
            if (filterAssign.evaluator.ids && filterAssign.evaluator.ids.length > 0) {
                angular.forEach(filterAssign.evaluator.ids, function (evaluatorId) {
                    var evaluatorAssignment = {
                        employeeId: selectedEmployeeId,
                        targetTemplateId: filterAssign.targetTemplateId,
                        processId: checkFlowModel.evaluatorType.Evaluator,
                        startDate: filterAssign.evaluator.startDate,
                        endDate: filterAssign.evaluator.endDate,
                        hasSendMail: filterAssign.evaluator.hasSendMail,
                        evaluatorInfo: {
                            employeeId: evaluatorId
                        }
                    };

                    employeeAssignments.push(evaluatorAssignment);
                });
            }
            else {
                var evaluatorAssignment = {
                    employeeId: selectedEmployeeId,
                    targetTemplateId: filterAssign.targetTemplateId,
                    processId: checkFlowModel.evaluatorType.Evaluator,
                    startDate: filterAssign.evaluator.startDate,
                    endDate: filterAssign.evaluator.endDate,
                    hasSendMail: filterAssign.evaluator.hasSendMail,
                    evaluatorInfo: {
                        employeeId: -1
                    }
                };
                employeeAssignments.push(evaluatorAssignment);
            }

            if (filterAssign.reviewer.ids && filterAssign.reviewer.ids.length > 0) {
                angular.forEach(filterAssign.reviewer.ids, function (reviewerId) {
                    var reviewerAssignment = {
                        employeeId: selectedEmployeeId,
                        targetTemplateId: filterAssign.targetTemplateId,
                        processId: checkFlowModel.evaluatorType.Reviewer,
                        startDate: filterAssign.reviewer.startDate,
                        endDate: filterAssign.reviewer.endDate,
                        hasSendMail: filterAssign.reviewer.hasSendMail,
                        evaluatorInfo: {
                            employeeId: reviewerId
                        }
                    };

                    employeeAssignments.push(reviewerAssignment);
                });
            }
            else {
                var reviewerAssignment = {
                    employeeId: selectedEmployeeId,
                    targetTemplateId: filterAssign.targetTemplateId,
                    processId: checkFlowModel.evaluatorType.Reviewer,
                    startDate: filterAssign.reviewer.startDate,
                    endDate: filterAssign.reviewer.endDate,
                    hasSendMail: filterAssign.reviewer.hasSendMail,
                    evaluatorInfo: {
                        employeeId: -1
                    }
                };
                employeeAssignments.push(reviewerAssignment);
            }

            if (filterAssign.management.ids && filterAssign.management.ids.length > 0) {
                angular.forEach(filterAssign.management.ids, function (managementId) {
                    var managementAssignment = {
                        employeeId: selectedEmployeeId,
                        targetTemplateId: filterAssign.targetTemplateId,
                        processId: checkFlowModel.evaluatorType.Management,
                        startDate: filterAssign.management.startDate,
                        endDate: filterAssign.management.endDate,
                        hasSendMail: filterAssign.management.hasSendMail,
                        evaluatorInfo: {
                            employeeId: managementId
                        }
                    };

                    employeeAssignments.push(managementAssignment);
                });
            }
            else {
                var managementAssignment = {
                    employeeId: selectedEmployeeId,
                    targetTemplateId: filterAssign.targetTemplateId,
                    processId: checkFlowModel.evaluatorType.Management,
                    startDate: filterAssign.management.startDate,
                    endDate: filterAssign.management.endDate,
                    hasSendMail: filterAssign.management.hasSendMail,
                    evaluatorInfo: {
                        employeeId: -1
                    }
                };
                employeeAssignments.push(managementAssignment);
            }
        });

        return employeeAssignments;
    };

    checkFlowModel.openPopupEditEmployeeSetting = function (vm, selectedEmployees, employeeSetting) {
        var modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/CheckFlowAssignment/Templates/editFlowSetting.html',
            controller: "EditEmployeeSettingController as editFlowCtrl",
            backdrop: 'static',
            resolve: {
                selectedEmployees: function () {
                    return selectedEmployees;
                },
                employeeSetting: function () {
                    return angular.copy(employeeSetting);
                }
            },
            windowClass: "modal-confirm-center employee-flow-setting"
        });

        modalInstance.result.then(function (editEmployeeSetting) {
            if (editEmployeeSetting) {
                employeeSetting.TargetTemplate = editEmployeeSetting.TargetTemplate;
                employeeSetting.Evaluators = editEmployeeSetting.Evaluators;
                employeeSetting.Reviewers = editEmployeeSetting.Reviewers;
                employeeSetting.Managements = editEmployeeSetting.Managements;
            }
            else {
                vm.query(vm.filter);
                vm.selectedAll = false;
            }
        })['finally'](function () {
            modalInstance = null;
            checkFlowModel.resetFilterAssign(vm);
        });
    };

    checkFlowModel.resetFilterAssign = function (vm) {
        vm.filterAssign = {
            evaluator: {
                ids: null,
                startDate: checkFlowModel.date,
                endDate: checkFlowModel.date,
                hasSendMail: true
            },
            reviewer: {
                ids: null,
                startDate: checkFlowModel.date,
                endDate: checkFlowModel.date,
                hasSendMail: true
            },
            management: {
                ids: null,
                startDate: checkFlowModel.date,
                endDate: checkFlowModel.date,
                hasSendMail: true
            },
            targetTemplateId: null,
            evaluatorEmployees: [],
            reviewerEmployees: [],
            managementEmployees: []
        };
    };

    checkFlowModel.uncheckPersonalSetting = function (employeeOptions) {
        var uncheck = true;

        angular.forEach(employeeOptions.evaluator.ids, function (id) {
            if (id !== -1) {
                uncheck = false;
            }
        });
        angular.forEach(employeeOptions.reviewer.ids, function (id) {
            if (id !== -1) {
                uncheck = false;
            }
        });
        angular.forEach(employeeOptions.management.ids, function (id) {
            if (id !== -1) {
                uncheck = false;
            }
        });
        return uncheck;
    };

    return checkFlowModel;
}]);