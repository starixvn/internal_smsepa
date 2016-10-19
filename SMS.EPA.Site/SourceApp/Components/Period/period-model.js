'use strict';
/*global periodModule*/

periodModule.factory('PeriodModel',
['$modal', 'periodFactory', 'errorFactory', 'notificationFactory', 'utilitiesFactory',
function ($modal, periodFactory, errorFactory, notificationFactory, utilitiesFactory) {
    var periodModel = {};

    periodModel.periods = [];
    periodModel.period = {};
    periodModel.periodDateFormat = 'dd MMM';
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

    periodModel.targetType = {
        Probation: 1,
        SemiAnnual: 3,
        Yearly: 4,
        Promotion: 5
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
            if (new Date(employeeOptions.humanResource.StartDate).setHours(0, 0, 0, 0) > new Date(employeeOptions.humanResource.EndDate).setHours(0, 0, 0, 0)) {
                notificationFactory.error("StartDateGreaterThanEndDateWarning");
                return false;
            }
        } else {
            notificationFactory.error("HumanResourcePhaseRequired");
            return false;
        }

        if (employeeOptions.selfEvaluator.ids && employeeOptions.selfEvaluator.ids.length > 0) {
            if (new Date(employeeOptions.selfEvaluator.StartDate).setHours(0, 0, 0, 0) > new Date(employeeOptions.selfEvaluator.EndDate).setHours(0, 0, 0, 0)) {
                notificationFactory.error("StartDateGreaterThanEndDateWarning");
                return false;
            }

            if (new Date(employeeOptions.humanResource.EndDate).setHours(0, 0, 0, 0) > new Date(employeeOptions.selfEvaluator.StartDate).setHours(0, 0, 0, 0)) {
                notificationFactory.error("HRRangeDateGreaterThanSelfRangeDateWarning");
                return false;
            }
        }

        if (employeeOptions.evaluator.ids && employeeOptions.evaluator.ids.length > 0) {
            if (new Date(employeeOptions.evaluator.StartDate).setHours(0, 0, 0, 0) > new Date(employeeOptions.evaluator.EndDate).setHours(0, 0, 0, 0)) {
                notificationFactory.error("StartDateGreaterThanEndDateWarning");
                return false;
            }

            if (new Date(employeeOptions.selfEvaluator.EndDate).setHours(0, 0, 0, 0) > new Date(employeeOptions.evaluator.StartDate).setHours(0, 0, 0, 0)) {
                notificationFactory.error("SelfRangeDateGreaterThanEvaluatorRangeDateWarning");
                return false;
            }
        } else {
            notificationFactory.error("EvaluatorPhaseRequired");
            return false;
        }

        if (employeeOptions.reviewer.ids && employeeOptions.reviewer.ids.length > 0) {
            hasReviewer = true;

            if (new Date(employeeOptions.reviewer.StartDate).setHours(0, 0, 0, 0) > new Date(employeeOptions.reviewer.EndDate).setHours(0, 0, 0, 0)) {
                notificationFactory.error("StartDateGreaterThanEndDateWarning");
                return false;
            }

            if (new Date(employeeOptions.evaluator.EndDate).setHours(0, 0, 0, 0) > new Date(employeeOptions.reviewer.StartDate).setHours(0, 0, 0, 0)) {
                notificationFactory.error("EvaluatorRangeDateGreaterThanReviewerRangeDateWarning");
                return false;
            }
        }

        if (employeeOptions.management.ids && employeeOptions.management.ids.length > 0) {
            if (new Date(employeeOptions.management.StartDate).setHours(0, 0, 0, 0) > new Date(employeeOptions.management.EndDate).setHours(0, 0, 0, 0)) {
                notificationFactory.error("StartDateGreaterThanEndDateWarning");
                return false;
            }

            if (hasReviewer) {
                if (new Date(employeeOptions.reviewer.EndDate).setHours(0, 0, 0, 0) > new Date(employeeOptions.management.StartDate).setHours(0, 0, 0, 0)) {
                    notificationFactory.error("ReviewerRangeDateGreaterThanManagementRangeDateWarning");
                    return false;
                }
            } else {
                if (new Date(employeeOptions.evaluator.EndDate).setHours(0, 0, 0, 0) > new Date(employeeOptions.management.StartDate).setHours(0, 0, 0, 0)) {
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
        }).error(function (response, status) {
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
        } else {
            if (!period.Name) {
                notificationFactory.error("NameRequired");
                return false;
            } else if (!period.TargetTypeId) {
                notificationFactory.error("TargetTypeRequired");
                return false;
            } else if (period.TargetTypeId === periodModel.targetType.SemiAnnual || period.TargetTypeId === periodModel.targetType.Promotion) {
                if (!period.RawStartDate) {
                    notificationFactory.error("StartDatePeriodRequired");
                    return false;
                }

                if (period.TargetTypeId === periodModel.targetType.SemiAnnual) {
                    if (!period.RawEndDate) {
                        notificationFactory.error("EndDatePeriodRequired");
                        return false;
                    } else if (!period.RawAttendanceStartDate) {
                        notificationFactory.error("DisciplineStartDatePeriodRequired");
                        return false;
                    } else if (!period.RawAttendanceEndDate) {
                        notificationFactory.error("DisciplineEndDatePeriodRequired");
                        return false;
                    } else if (new Date(period.RawStartDate).setHours(0, 0, 0, 0) > new Date(period.RawEndDate).setHours(0, 0, 0, 0)) {
                        notificationFactory.error("StartDateGreaterThanEndDateWarning");
                        return false;
                    } else if (new Date(period.RawAttendanceStartDate).setHours(0, 0, 0, 0) > new Date(period.RawAttendanceEndDate).setHours(0, 0, 0, 0)) {
                        notificationFactory.error("DisciplineStartDateGreaterThanEndDateWarning");
                        return false;
                    }
                }
            }
        }

        return true;
    };

    periodModel.createPeriod = function (period, modalInstance) {
        return periodFactory.createPeriod(period).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("CreatedSuccessfully");
                modalInstance.close();
            } else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    periodModel.updateBasicInformationPeriod = function (period) {
        return periodFactory.updateBasicInformationPeriod(period).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
            } else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    periodModel.getPeriodById = function (periodId) {
        return periodFactory.getPeriodById(periodId).success(function (period) {
            periodModel.period = period;
        }).error(function (response, status) {
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
            periodFactory.updateStatusPeriod(newPeriod).success(function (response) {
                if (response.IsSuccessful) {
                    keyMessage = action === "activate" ? "ActivationSuccess" : "DeactivationSuccess";
                    notificationFactory.success(keyMessage);
                } else {
                    keyMessage = action === "activate" ? "ActivationFail" : "DeactivationFail";
                    period.IsActive = !newPeriod.IsActive;
                    notificationFactory.error(keyMessage);
                }
            }).error(function (response, status) {
                if (status === 405) {
                    errorFactory.errorHandle(status);
                } else {
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
                } else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        })['finally'](function () {
            modalInstance = '';
        });
    };

    periodModel.getPeriodEmployeeSettingById = function (periodId, departmentId, isLocal, isExcludedEmployee, isGeneratedEPA, isEvaluated, hasMonthlyEPA, epaStatus) {
        return periodFactory.getPeriodEmployeeSettingById(periodId, departmentId, isLocal, isExcludedEmployee, isGeneratedEPA, isEvaluated, hasMonthlyEPA, epaStatus)
        .success(function (employeeSettings) {
            periodModel.employeeSettings = employeeSettings;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    periodModel.addEmployeeToPeriod = function (employeeInfo) {
        return periodFactory.addEmployeeToPeriod(employeeInfo).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("CreatedSuccessfully");
            } else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    periodModel.openPopupUpdateEmployeeSetting = function (vm, selectedEmployees) {
        var modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/Period/Templates/editEmployeeSetting.html',
            controller: "UpdateEmployeeSettingPeriodController as updatePeriodSettingCtrl",
            backdrop: 'static',
            resolve: {
                period: function () {
                    return vm.period;
                },
                selectedEmployees: function () {
                    return angular.copy(selectedEmployees);
                },
                permission: function () {
                    return vm.permission;
                }
            },
            windowClass: "modal-confirm-center employee-period-setting"
        });

        modalInstance.result.then(function () {
            vm.query(vm.filter);
            vm.selectedAll = false;
        })['finally'](function () {
            modalInstance = null;
        });
    };

    periodModel.updateEmployeeSettingOfPeriod = function (employeeSetting) {
        return periodFactory.updateEmployeeSettingOfPeriod(employeeSetting).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
            } else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    periodModel.createAssignmentSettings = function (employeeOptions, periodId, selectedEmployees, employeeIds) {
        var employeeAssignments = [],
            targetTemplateId = employeeOptions.targetTemplateId;

        angular.forEach(selectedEmployees, function (employee) {
            if (employeeOptions.humanResource.ids && employeeOptions.humanResource.ids.length > 0) {
                angular.forEach(employeeOptions.humanResource.ids, function (humanResourceId, index) {
                    var humanResourceAssignment = {
                        employeeId: employee.Employee.EmployeeId,
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
                employeeId: employee.Employee.EmployeeId,
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
                        employeeId: employee.Employee.EmployeeId,
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
                        employeeId: employee.Employee.EmployeeId,
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
                        employeeId: employee.Employee.EmployeeId,
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
        });

        var employeeSetting = {
            periodId: periodId,
            targetTemplateId: targetTemplateId,
            employeeAssignments: employeeAssignments,
            employeeIds: employeeIds
        };

        return employeeSetting;
    };

    periodModel.openPopupIncludeExcludeEmployee = function (vm, periodId, employee) {
        employee.IsIncluded = !employee.IsIncluded;

        var action = employee.IsIncluded ? "include" : "exclude",
        newEmployeeStatus = {
            PeriodId: periodId,
            EmployeeId: employee.Employee.EmployeeId,
            IsIncluded: employee.IsIncluded
        };

        var modalInstance = $modal.open({
            templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return employee.Employee.NickName;
                },
                action: function () {
                    return action;
                }
            },
            windowClass: "modal-confirm-center"
        });

        modalInstance.result.then(function () {
            periodFactory.updateIncludeStatusEmployeePeriod(newEmployeeStatus).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                } else {
                    employee.IsIncluded = !newEmployeeStatus.IsIncluded;
                    notificationFactory.error("UpdatedUnsuccessfully");
                }

                vm.query(vm.filter);
            }).error(function (response, status) {
                if (status === 405) {
                    errorFactory.errorHandle(status);
                } else {
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
        var selectedEmployees = [];

        angular.forEach(employeeSettings, function (employeeSetting) {
            if (employeeSetting.Selected) {
                selectedEmployees.push(employeeSetting);
            }
        });

        return selectedEmployees;
    };

    periodModel.getSelectedEmployeeInfo = function (employeeSettings) {
        var selectedEmployeeIds = '',
            selectedEmployeeNames = '';

        angular.forEach(employeeSettings, function (employeeSetting) {
            selectedEmployeeIds += selectedEmployeeIds ? ', ' + employeeSetting.Employee.EmployeeId : employeeSetting.Employee.EmployeeId;
            selectedEmployeeNames += selectedEmployeeNames ? ', ' + employeeSetting.Employee.NickName : employeeSetting.Employee.NickName;
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

        return modalInstance.result.then(function () {
            return periodFactory.generateEvaluation(period).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("CreatedSuccessfully");
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        })['finally'](function () {
            modalInstance = '';
        });
    };

    periodModel.getProcessSettingByPeriodId = function (periodId) {
        return periodFactory.getProcessSettingByPeriodId(periodId).success(function (processSettings) {
            periodModel.processSettings = processSettings;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    periodModel.updatePeriodProcessSetting = function (periodProcessSetting) {
        return periodFactory.updatePeriodProcessSetting(periodProcessSetting).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
            } else {
                notificationFactory.error("UpdatedUnsuccessfully");
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    periodModel.checkSelfProcessStarted = function (periodId, employeeIds) {
        return periodFactory.checkSelfProcessStarted(periodId, employeeIds).success(function (hasSelfStarted) {
            periodModel.hasSelfStarted = hasSelfStarted;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

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
                        ProcessId: periodModel.evaluatorType.HumanResources,
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
                        ProcessId: periodModel.evaluatorType.Evaluator,
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
                        ProcessId: periodModel.evaluatorType.Reviewer,
                        EvaluatorInfo: {
                            EmployeeId: id
                        }
                    };

                    employee.Reviewers.push(reviewer);
                });
            }
        }

        if (employeeOptions.management.ids && employeeOptions.management.ids.length > 0) {
            if (employee.Managements && employee.Managements.length > 0) {
                var originalManagement = employee.Managements[0];
                employee.Managements = [];

                angular.forEach(employeeOptions.management.ids, function (id) {
                    var management = {
                        ProcessId: originalManagement.ProcessId,
                        StartDate: originalManagement.StartDate,
                        EndDate: originalManagement.EndDate,
                        EvaluatorInfo: {
                            EmployeeId: id
                        }
                    };

                    employee.Managements.push(management);
                });
            } else {
                angular.forEach(employeeOptions.management.ids, function (id) {
                    var management = {
                        ProcessId: periodModel.evaluatorType.Management,
                        EvaluatorInfo: {
                            EmployeeId: id
                        }
                    };

                    employee.Managements.push(management);
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

        angular.forEach(employee.Managements, function (management) {
            management.StartDate = employeeOptions.management.StartDate;
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

        angular.forEach(employee.Managements, function (management) {
            management.EndDate = employeeOptions.management.EndDate;
        });

        return employee;
    }

    periodModel.updateEmployeeSettings = function (employeeOptions, fieldEdit, selectedEmployees) {
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
    };

    periodModel.validateEmployeeSettingMultipleMode = function (updatedEmployees) {
        for (var i = 0; i < updatedEmployees.length; i++) {
            var employee = updatedEmployees[i],
                hasReviewer = false;

            if (employee.HumanResources && employee.HumanResources.length > 0) {
                if (new Date(employee.HumanResources[0].StartDate).setHours(0, 0, 0, 0) > new Date(employee.HumanResources[0].EndDate).setHours(0, 0, 0, 0)) {
                    notificationFactory.error("StartDateGreaterThanEndDateWarning");
                    return false;
                }
            } else {
                notificationFactory.error("HumanResourcePhaseRequired");
                return false;
            }

            if (employee.SelfEvaluators && employee.SelfEvaluators.length > 0) {
                if (new Date(employee.SelfEvaluators[0].StartDate).setHours(0, 0, 0, 0) > new Date(employee.SelfEvaluators[0].EndDate).setHours(0, 0, 0, 0)) {
                    notificationFactory.error("StartDateGreaterThanEndDateWarning");
                    return false;
                }

                if (new Date(employee.HumanResources[0].EndDate).setHours(0, 0, 0, 0) > new Date(employee.SelfEvaluators[0].StartDate).setHours(0, 0, 0, 0)) {
                    notificationFactory.error("HRRangeDateGreaterThanSelfRangeDateWarning");
                    return false;
                }
            }

            if (employee.Evaluators && employee.Evaluators.length > 0) {
                if (new Date(employee.Evaluators[0].StartDate).setHours(0, 0, 0, 0) > new Date(employee.Evaluators[0].EndDate).setHours(0, 0, 0, 0)) {
                    notificationFactory.error("StartDateGreaterThanEndDateWarning");
                    return false;
                }

                if (new Date(employee.SelfEvaluators[0].EndDate).setHours(0, 0, 0, 0) > new Date(employee.Evaluators[0].StartDate).setHours(0, 0, 0, 0)) {
                    notificationFactory.error("SelfRangeDateGreaterThanEvaluatorRangeDateWarning");
                    return false;
                }
            } else {
                notificationFactory.error("EvaluatorPhaseRequired");
                return false;
            }

            if (employee.Reviewers && employee.Reviewers.length > 0) {
                hasReviewer = true;

                if (new Date(employee.Reviewers[0].StartDate).setHours(0, 0, 0, 0) > new Date(employee.Reviewers[0].EndDate).setHours(0, 0, 0, 0)) {
                    notificationFactory.error("StartDateGreaterThanEndDateWarning");
                    return false;
                }

                if (new Date(employee.Evaluators[0].EndDate).setHours(0, 0, 0, 0) > new Date(employee.Reviewers[0].StartDate).setHours(0, 0, 0, 0)) {
                    notificationFactory.error("EvaluatorRangeDateGreaterThanReviewerRangeDateWarning");
                    return false;
                }
            }

            if (employee.Managements && employee.Managements.length > 0) {
                if (new Date(employee.Managements[0].StartDate).setHours(0, 0, 0, 0) > new Date(employee.Managements[0].EndDate).setHours(0, 0, 0, 0)) {
                    notificationFactory.error("StartDateGreaterThanEndDateWarning");
                    return false;
                }

                if (hasReviewer) {
                    if (new Date(employee.Reviewers[0].EndDate).setHours(0, 0, 0, 0) > new Date(employee.Managements[0].StartDate).setHours(0, 0, 0, 0)) {
                        notificationFactory.error("ReviewerRangeDateGreaterThanManagementRangeDateWarning");
                        return false;
                    }
                } else {
                    if (new Date(employee.Evaluators[0].EndDate).setHours(0, 0, 0, 0) > (employee.Managements[0].StartDate).setHours(0, 0, 0, 0)) {
                        notificationFactory.error("EvaluatorRangeDateGreaterThanManagementRangeDateWarning");
                        return false;
                    }
                }
            }

            return true;
        }
    };

    periodModel.createAssignmentSettingsMultipleMode = function (updatedEmployees, vm) {
        var employeeAssignments = [];

        angular.forEach(updatedEmployees, function (employee) {
            if (employee.HumanResources && employee.HumanResources.length > 0) {
                angular.forEach(employee.HumanResources, function (humanResource, index) {
                    var humanResourceAssignment = {
                        employeeId: employee.Employee.EmployeeId,
                        processId: periodModel.evaluatorType.HumanResources,
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
                processId: periodModel.evaluatorType.SelfEvaluator,
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
                        processId: periodModel.evaluatorType.Evaluator,
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
                        processId: periodModel.evaluatorType.Reviewer,
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

            if (employee.Managements && employee.Managements.length > 0) {
                angular.forEach(employee.Managements, function (management, index) {
                    var managementAssignment = {
                        employeeId: employee.Employee.EmployeeId,
                        processId: periodModel.evaluatorType.Management,
                        startDate: new Date(management.StartDate).toDateString(),
                        endDate: new Date(management.EndDate).toDateString(),
                        priority: index + 1,
                        evaluatorInfo: {
                            employeeId: management.EvaluatorInfo.EmployeeId
                        }
                    };

                    employeeAssignments.push(managementAssignment);
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
    };

    periodModel.checkHasFieldEdit = function (fieldEdit) {
        var hasEdited = false;

        if (fieldEdit.targetTemplate) {
            hasEdited = true;
        } else if (fieldEdit.evaluator) {
            hasEdited = true;
        } else if (fieldEdit.startDate) {
            hasEdited = true;
        } else if (fieldEdit.endDate) {
            hasEdited = true;
        } else {
            notificationFactory.error("PeriodEditSettingWarning");
        }

        return hasEdited;
    };

    periodModel.validateEmployeeSettingGenerateEPA = function (employeeSettings) {
        for (var i = 0; i < employeeSettings.length; i++) {
            var employeeSetting = employeeSettings[i];

            if (!employeeSetting.TargetTemplate) {
                notificationFactory.error("NoneTargetTemplateSettingWarning");
                return false;
            } else if (employeeSetting.HumanResources && employeeSetting.HumanResources.length > 0
                && (!employeeSetting.HumanResources[0].StartDate || !employeeSetting.HumanResources[0].EndDate)) {
                notificationFactory.error("NoneDateSettingWarning");
                return false;
            } else if (employeeSetting.SelfEvaluators && employeeSetting.SelfEvaluators.length > 0
                && (!employeeSetting.SelfEvaluators[0].StartDate || !employeeSetting.SelfEvaluators[0].EndDate)) {
                notificationFactory.error("NoneDateSettingWarning");
                return false;
            } else if (employeeSetting.Evaluators && employeeSetting.Evaluators.length > 0
                && (!employeeSetting.Evaluators[0].StartDate || !employeeSetting.Evaluators[0].EndDate)) {
                notificationFactory.error("NoneDateSettingWarning");
                return false;
            } else if (employeeSetting.Reviewers && employeeSetting.Reviewers.length > 0
                && (!employeeSetting.Reviewers[0].StartDate || !employeeSetting.Reviewers[0].EndDate)) {
                notificationFactory.error("NoneDateSettingWarning");
                return false;
            } else if (employeeSetting.Managements && employeeSetting.Managements.length > 0
                && (!employeeSetting.Managements[0].StartDate || !employeeSetting.Managements[0].EndDate)) {
                notificationFactory.error("NoneDateSettingWarning");
                return false;
            }
        }

        return true;
    };

    return periodModel;
}]);