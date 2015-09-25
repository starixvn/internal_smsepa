'use strict';
/*global periodModule*/

periodModule.factory('PeriodModel',
['$modal', 'periodFactory', 'errorFactory', 'notificationFactory', 'utilitiesFactory',
function ($modal, periodFactory, errorFactory, notificationFactory, utilitiesFactory) {
    var periodModel = {};

    periodModel.periods = [];
    periodModel.period = {};
    periodModel.periodDateFormat = 'dd MMM';
    periodModel.shortNameMonth = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    periodModel.employeeSettings = [];
    periodModel.processSettings = [];
    periodModel.hasSelfStarted = false;

    periodModel.evaluatorType = {
        HumanResources: 1,
        SelfEvaluator: 2,
        Evaluator: 3,
        Reviewer: 4,
        Management: 5
    };

    periodModel.processStatus = {
        NotAvailable: 0,
        Open: 1,
        InProgress: 2,
        Closed: 3
    };

    periodModel.permission = 0;

    periodModel.calendar = {
        startDateOpened: false,
        endDateOpened: false,
        attendanceStartDateOpened: false,
        attendanceEndDateOpened: false
    };

    periodModel.flowCalendar = {
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
        },
        management: {
            startDateOpened: false,
            endDateOpened: false
        }
    };

    periodModel.getPermissionOnPeriod = function () {
        return periodFactory.getPermissionOnPeriod().success(function (permisson) {
            periodModel.permission = permisson;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    periodModel.openPopupStartDate = function (event) {
        event.preventDefault();
        event.stopPropagation();

        periodModel.calendar.startDateOpened = true;
        periodModel.calendar.endDateOpened = false;
        periodModel.calendar.attendanceStartDateOpened = false;
        periodModel.calendar.attendanceEndDateOpened = false;
    };

    periodModel.openPopupEndDate = function (event) {
        event.preventDefault();
        event.stopPropagation();

        periodModel.calendar.startDateOpened = false;
        periodModel.calendar.endDateOpened = true;
        periodModel.calendar.attendanceStartDateOpened = false;
        periodModel.calendar.attendanceEndDateOpened = false;
    };

    periodModel.openPopupAttendanceStartDate = function (event) {
        event.preventDefault();
        event.stopPropagation();

        periodModel.calendar.startDateOpened = false;
        periodModel.calendar.endDateOpened = false;
        periodModel.calendar.attendanceStartDateOpened = true;
        periodModel.calendar.attendanceEndDateOpened = false;
    };

    periodModel.openPopupAttendanceEndDate = function (event) {
        event.preventDefault();
        event.stopPropagation();

        periodModel.calendar.startDateOpened = false;
        periodModel.calendar.endDateOpened = false;
        periodModel.calendar.attendanceStartDateOpened = false;
        periodModel.calendar.attendanceEndDateOpened = true;
    };

    periodModel.resetCalendar = function (vm) {
        vm.flowCalendar.humanResource.startDateOpened = false;
        vm.flowCalendar.humanResource.endDateOpened = false;

        vm.flowCalendar.selfEvaluator.startDateOpened = false;
        vm.flowCalendar.selfEvaluator.endDateOpened = false;

        vm.flowCalendar.evaluator.startDateOpened = false;
        vm.flowCalendar.evaluator.endDateOpened = false;

        vm.flowCalendar.reviewer.startDateOpened = false;
        vm.flowCalendar.reviewer.endDateOpened = false;

        vm.flowCalendar.management.startDateOpened = false;
        vm.flowCalendar.management.endDateOpened = false;
    };

    periodModel.openFlowPopupStartDate = function (vm, event, evaluatorType) {
        event.preventDefault();
        event.stopPropagation();
        periodModel.resetCalendar(vm);

        switch (evaluatorType) {
            case periodModel.evaluatorType.HumanResources:
                vm.flowCalendar.humanResource.startDateOpened = true;
                break;
            case periodModel.evaluatorType.SelfEvaluator:
                vm.flowCalendar.selfEvaluator.startDateOpened = true;
                break;
            case periodModel.evaluatorType.Evaluator:
                vm.flowCalendar.evaluator.startDateOpened = true;
                break;
            case periodModel.evaluatorType.Reviewer:
                vm.flowCalendar.reviewer.startDateOpened = true;
                break;
            case periodModel.evaluatorType.Management:
                vm.flowCalendar.management.startDateOpened = true;
                break;
        }
    };

    periodModel.openFlowPopupEndDate = function (vm, event, evaluatorType) {
        event.preventDefault();
        event.stopPropagation();
        periodModel.resetCalendar(vm);

        switch (evaluatorType) {
            case periodModel.evaluatorType.HumanResources:
                vm.flowCalendar.humanResource.endDateOpened = true;
                break;
            case periodModel.evaluatorType.SelfEvaluator:
                vm.flowCalendar.selfEvaluator.endDateOpened = true;
                break;
            case periodModel.evaluatorType.Evaluator:
                vm.flowCalendar.evaluator.endDateOpened = true;
                break;
            case periodModel.evaluatorType.Reviewer:
                vm.flowCalendar.reviewer.endDateOpened = true;
                break;
            case periodModel.evaluatorType.Management:
                vm.flowCalendar.management.endDateOpened = true;
                break;
        }
    };

    periodModel.validateEmployeeSetting = function (employeeOptions) {
        var hasReviewer = false;

        if (employeeOptions.humanResource.ids && employeeOptions.humanResource.ids.length > 0) {
            if (employeeOptions.humanResource.StartDate > employeeOptions.humanResource.EndDate) {
                notificationFactory.error("StartDateGreaterThanEndDateWarning");
                return false;
            }
        }
        else {
            notificationFactory.error("HumanResourcePhaseRequired");
            return false;
        }

        if (employeeOptions.selfEvaluator.ids && employeeOptions.selfEvaluator.ids.length > 0) {
            if (employeeOptions.selfEvaluator.StartDate > employeeOptions.selfEvaluator.EndDate) {
                notificationFactory.error("StartDateGreaterThanEndDateWarning");
                return false;
            }

            if (employeeOptions.humanResource.EndDate > employeeOptions.selfEvaluator.StartDate) {
                notificationFactory.error("HRRangeDateGreaterThanSelfRangeDateWarning");
                return false;
            }
        }

        if (employeeOptions.evaluator.ids && employeeOptions.evaluator.ids.length > 0) {
            if (employeeOptions.evaluator.StartDate > employeeOptions.evaluator.EndDate) {
                notificationFactory.error("StartDateGreaterThanEndDateWarning");
                return false;
            }

            if (employeeOptions.selfEvaluator.EndDate > employeeOptions.evaluator.StartDate) {
                notificationFactory.error("SelfRangeDateGreaterThanEvaluatorRangeDateWarning");
                return false;
            }
        }
        else {
            notificationFactory.error("EvaluatorPhaseRequired");
            return false;
        }

        if (employeeOptions.reviewer.ids && employeeOptions.reviewer.ids.length > 0) {
            hasReviewer = true;

            if (employeeOptions.reviewer.StartDate > employeeOptions.reviewer.EndDate) {
                notificationFactory.error("StartDateGreaterThanEndDateWarning");
                return false;
            }

            if (employeeOptions.evaluator.EndDate > employeeOptions.reviewer.StartDate) {
                notificationFactory.error("EvaluatorRangeDateGreaterThanReviewerRangeDateWarning");
                return false;
            }
        }

        if (employeeOptions.management.ids && employeeOptions.management.ids.length > 0) {
            if (employeeOptions.management.StartDate > employeeOptions.management.EndDate) {
                notificationFactory.error("StartDateGreaterThanEndDateWarning");
                return false;
            }

            if (hasReviewer) {
                if (employeeOptions.reviewer.EndDate > employeeOptions.management.StartDate) {
                    notificationFactory.error("ReviewerRangeDateGreaterThanManagementRangeDateWarning");
                    return false;
                }
            }
            else {
                if (employeeOptions.evaluator.EndDate > employeeOptions.management.StartDate) {
                    notificationFactory.error("EvaluatorRangeDateGreaterThanManagementRangeDateWarning");
                    return false;
                }
            }
        }
        return true;
    };

    periodModel.getPeriod = function (targetTypeId, isActive) {
        return periodFactory.getPeriod(targetTypeId, isActive).success(function (periods) {
            periodModel.periods = periods;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    periodModel.openPopupCreatePeriod = function (vm) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/Period/Templates/create.html',
            controller: "CreatePeriodController as createPeriodCtrl",
            backdrop: 'static',
            windowClass: "modal-confirm-center period-create-popup"
        });

        return vm.modalInstance.result;
    };

    periodModel.validatePeriod = function (period) {
        if (!period) {
            return false;
        }
        else {
            if (!period.Name) {
                notificationFactory.error("NameRequired");
                return false;
            }
            else if (!period.TargetTypeId) {
                notificationFactory.error("TargetTypeRequired");
                return false;
            }
        }

        return true;
    };

    periodModel.createPeriod = function (period, modalInstance) {
        return periodFactory.createPeriod(period).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("CreatedSuccessfully");
                modalInstance.close();
            }
            else {
                notificationFactory.error("", response.Message);
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    periodModel.getShortNameMonth = function (index) {
        return periodModel.shortNameMonth[index];
    };

    periodModel.updateBasicInformationPeriod = function (period) {
        return periodFactory.updateBasicInformationPeriod(period).success(function (response) {
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

    periodModel.getPeriodById = function (periodId) {
        return periodFactory.getPeriodById(periodId).success(function (period) {
            periodModel.period = period;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    periodModel.openPopupUpdateStatusPeriod = function (period) {
        period.IsActive = !period.IsActive;

        var action = period.IsActive ? "activate" : "deactivate",
        newPeriod = {
            PeriodId: period.PeriodId,
            IsActive: period.IsActive
        };

        var modalInstance = $modal.open({
            templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return period.Name;
                },
                action: function () {
                    return action;
                }
            },
            windowClass: "modal-confirm-center"
        });

        var keyMessage = '';

        modalInstance.result.then(function () {
            periodFactory.updateStatusPeriod(newPeriod)
            .success(function (response) {
                if (response.IsSuccessful) {
                    keyMessage = action === "activate" ? "ActivationSuccess" : "DeactivationSuccess";
                    notificationFactory.success(keyMessage);
                }
                else {
                    keyMessage = action === "activate" ? "ActivationFail" : "DeactivationFail";
                    period.IsActive = !newPeriod.IsActive;
                    notificationFactory.error(keyMessage);
                }
            })
            .error(function (response, status) {
                if (status === 405) {
                    errorFactory.errorHandle(status);
                }
                else {
                    keyMessage = action === "activate" ? "ActivationFail" : "DeactivationFail";
                    period.IsActive = !newPeriod.IsActive;
                    notificationFactory.error(keyMessage);
                }
            });
        }, function () {
            period.IsActive = !newPeriod.IsActive;
        })['finally'](function () {
            modalInstance = '';
        });
    };

    periodModel.openPopupDeletePeriod = function (vm, period) {
        var modalInstance = $modal.open({
            templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return period.Name;
                },
                action: function () {
                    return 'delete';
                }
            },
            windowClass: "modal-confirm-center"
        });

        modalInstance.result.then(function () {
            periodFactory.deletePeriod(period.PeriodId).success(function (response) {
                if (response.IsSuccessful) {
                    var index = vm.periods.indexOf(period);
                    vm.periods.splice(index, 1);
                    utilitiesFactory.searchOnPage(vm, vm.periods);
                    notificationFactory.success("DeletedSuccessfully");
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        })['finally'](function () {
            modalInstance = '';
        });
    };

    periodModel.getPeriodEmployeeSettingById = function (periodId, departmentId, isExcludedEmployee) {
        return periodFactory.getPeriodEmployeeSettingById(periodId, departmentId, isExcludedEmployee).success(function (employeeSettings) {
            periodModel.employeeSettings = employeeSettings;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    periodModel.addEmployeeToPeriod = function (employeeInfo) {
        return periodFactory.addEmployeeToPeriod(employeeInfo).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("CreatedSuccessfully");
            }
            else {
                notificationFactory.error("", response.Message);
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    periodModel.openPopupUpdateEmployeeSetting = function (vm, selectedEmployees, employeeSetting) {
        var modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/Period/Templates/editEmployeeSetting.html',
            controller: "UpdateEmployeeSettingPeriodController as updatePeriodSettingCtrl",
            backdrop: 'static',
            resolve: {
                period: function () {
                    return vm.period;
                },
                employeeSetting: function () {
                    return angular.copy(employeeSetting);
                },
                selectedEmployees: function () {
                    return selectedEmployees;
                },
                permission: function () {
                    return vm.permission;
                }
            },
            windowClass: "modal-confirm-center employee-period-setting"
        });

        modalInstance.result.then(function (editEmployeeSetting) {
            if (editEmployeeSetting) {
                employeeSetting.TargetTemplate = editEmployeeSetting.TargetTemplate;
                employeeSetting.HumanResources = editEmployeeSetting.HumanResources;
                employeeSetting.SelfEvaluators = editEmployeeSetting.SelfEvaluators;
                employeeSetting.Evaluators = editEmployeeSetting.Evaluators;
                employeeSetting.Reviewers = editEmployeeSetting.Reviewers;
                employeeSetting.Managements = editEmployeeSetting.Managements;
                employeeSetting.NumberEvaluators = editEmployeeSetting.NumberEvaluators;

            }
            else {
                vm.query(vm.filter);
                vm.selectedAll = false;
            }

        })['finally'](function () {
            modalInstance = null;
        });
    };

    periodModel.updateEmployeeSettingOfPeriod = function (employeeSetting) {
        return periodFactory.updateEmployeeSettingOfPeriod(employeeSetting).success(function (response) {
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

    periodModel.createAssignmentSettings = function (employeeOptions, periodId, selectedEmployee) {
        var employeeAssignments = [],
            targetTemplateId = employeeOptions.targetTemplateId;

        if (employeeOptions.humanResource.ids && employeeOptions.humanResource.ids.length > 0) {
            angular.forEach(employeeOptions.humanResource.ids, function (humanResourceId, index) {
                var humanResourceAssignment = {
                    processId: periodModel.evaluatorType.HumanResources,
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
            processId: periodModel.evaluatorType.SelfEvaluator,
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
                    processId: periodModel.evaluatorType.Evaluator,
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
                    processId: periodModel.evaluatorType.Reviewer,
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

        if (employeeOptions.management.ids && employeeOptions.management.ids.length > 0) {
            angular.forEach(employeeOptions.management.ids, function (managementId, index) {
                var managementAssignment = {
                    processId: periodModel.evaluatorType.Management,
                    startDate: employeeOptions.management.StartDate.toDateString(),
                    endDate: employeeOptions.management.EndDate.toDateString(),
                    priority: index + 1,
                    evaluatorInfo: {
                        employeeId: managementId
                    }
                };

                employeeAssignments.push(managementAssignment);
            });
        }

        var employeeSetting = {
            periodId: periodId,
            employeeIds: selectedEmployee,
            targetTemplateId: targetTemplateId,
            employeeAssignments: employeeAssignments
        };

        return employeeSetting;
    };

    periodModel.getUpdateTargetTemplate = function (targetTemplateId, targetTemplates) {
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
            }
        }

        return employee;
    };

    var getProcessStatus = function (startDate, endDate) {
        var processStatus = 0,
            date = new Date(),
            today = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0, 0),
            startDateNoTime = new Date(startDate.getFullYear(), startDate.getMonth(), startDate.getDate(), 0, 0, 0, 0),
            endDateNoTime = new Date(endDate.getFullYear(), endDate.getMonth(), endDate.getDate(), 0, 0, 0, 0);

        if (startDateNoTime > today) {
            processStatus = 1;
        }
        else if (startDateNoTime <= today && today <= endDateNoTime) {
            processStatus = 2;
        }
        else {
            processStatus = 3;
        }

        return processStatus;
    };

    periodModel.getUpdateEvaluators = function (evaluator, evaluatorType, employees) {
        var evaluators = [];

        if (evaluator && evaluator.ids != null && evaluator.ids.length > 0) {
            angular.forEach(evaluator.ids, function (id) {
                var employee = getEmployeeById(employees, id),
                    evaluatorItem = {
                        ProcessId: evaluatorType,
                        ProcessStatus: getProcessStatus(evaluator.StartDate, evaluator.EndDate),
                        StartDate: evaluator.StartDate,
                        EndDate: evaluator.EndDate,
                        EvaluatorInfo: employee ? employee : {
                            EmployeeId: -1
                        },
                        HasEvaluationGenerated: evaluator.HasEvaluationGenerated
                    };

                evaluators.push(evaluatorItem);
            });
        }

        return evaluators;
    };

    periodModel.openPopupIncludeExcludeEmployee = function (periodId, employee) {
        employee.IsIncluded = !employee.IsIncluded;

        var action = employee.IsIncluded ? "include" : "exclude",
        newEmployeeStatus = {
            PeriodId: periodId,
            EmployeeId: employee.EmployeeId,
            IsIncluded: employee.IsIncluded
        };

        var modalInstance = $modal.open({
            templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return employee.NickName;
                },
                action: function () {
                    return action;
                }
            },
            windowClass: "modal-confirm-center"
        });

        modalInstance.result.then(function () {
            periodFactory.updateIncludeStatusEmployeePeriod(newEmployeeStatus)
            .success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                }
                else {
                    employee.IsIncluded = !newEmployeeStatus.IsIncluded;
                    notificationFactory.error("UpdatedUnsuccessfully");
                }
            })
            .error(function (response, status) {
                if (status === 405) {
                    errorFactory.errorHandle(status);
                }
                else {
                    employee.IsIncluded = !newEmployeeStatus.IsIncluded;
                    notificationFactory.error("UpdatedUnsuccessfully");
                }
            });
        }, function () {
            employee.IsIncluded = !newEmployeeStatus.IsIncluded;
        })['finally'](function () {
            modalInstance = '';
        });
    };

    periodModel.getSelectedEmployee = function (employeeSettings) {
        var selectedEmployeeIds = '',
            selectedEmployeeNames = '';

        angular.forEach(employeeSettings, function (employeeSetting) {
            if (employeeSetting.Selected) {
                selectedEmployeeIds += selectedEmployeeIds ? ', ' + employeeSetting.EmployeeId : employeeSetting.EmployeeId;
                selectedEmployeeNames += selectedEmployeeNames ? ', ' + employeeSetting.NickName : employeeSetting.NickName;
            }
        });

        return {
            selectedEmployeeIds: selectedEmployeeIds,
            selectedEmployeeNames: selectedEmployeeNames
        };
    };

    periodModel.openPopupGenerateEvaluation = function (vm, period) {
        var modalInstance = $modal.open({
            templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return "EPA";
                },
                action: function () {
                    return 'generate';
                }
            },
            windowClass: "modal-confirm-center"
        });

        modalInstance.result.then(function () {
            return periodFactory.generateEvaluation(period).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("CreatedSuccessfully");
                    vm.query(vm.filter);
                }
                else {
                    notificationFactory.error("CreatedUnsuccessfully");
                }
            })
            .error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        })['finally'](function () {
            modalInstance = '';
        });
    };

    periodModel.getProcessSettingByPeriodId = function (periodId) {
        return periodFactory.getProcessSettingByPeriodId(periodId).success(function (processSettings) {
            periodModel.processSettings = processSettings;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    periodModel.updatePeriodProcessSetting = function (periodProcessSetting) {
        return periodFactory.updatePeriodProcessSetting(periodProcessSetting).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
            }
            else {
                employee.IsIncluded = !newEmployeeStatus.IsIncluded;
                notificationFactory.error("UpdatedUnsuccessfully");
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    periodModel.checkSelfProcessStarted = function (periodId, employeeIds) {
        return periodFactory.checkSelfProcessStarted(periodId, employeeIds).success(function (hasSelfStarted) {
            periodModel.hasSelfStarted = hasSelfStarted;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return periodModel;
}]);