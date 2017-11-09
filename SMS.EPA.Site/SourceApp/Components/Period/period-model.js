(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('periodModule').factory('PeriodModel', periodModel);
    periodModel.$inject = ['$uibModal', 'periodFactory', 'errorFactory', 'notificationFactory', 'utilitiesFactory'];

    function periodModel($uibModal, periodFactory, errorFactory, notificationFactory, utilitiesFactory) {
        var model = {};

        model.periods = [];
        model.period = {};
        model.periodDateFormat = 'dd MMM';
        model.employeeSettings = [];
        model.processSettings = [];
        model.hasSelfStarted = false;

        model.evaluatorType = {
            HumanResources: 1,
            SelfEvaluator: 2,
            Evaluator: 3,
            Reviewer: 4,
            Management: 5
        };

        model.processStatus = {
            NotAvailable: 0,
            Open: 1,
            InProgress: 2,
            Closed: 3
        };

        model.targetType = {
            Probation: 1,
            SemiAnnual: 3,
            Yearly: 4,
            Promotion: 5
        };

        model.permission = 0;

        model.calendar = {
            startDateOpened: false,
            endDateOpened: false,
            attendanceStartDateOpened: false,
            attendanceEndDateOpened: false
        };

        model.flowCalendar = {
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

        model.getPermissionOnPeriod = function () {
            return periodFactory.getPermissionOnPeriod().success(function (permisson) {
                model.permission = permisson;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.openPopupStartDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            model.calendar.startDateOpened = true;
            model.calendar.endDateOpened = false;
            model.calendar.attendanceStartDateOpened = false;
            model.calendar.attendanceEndDateOpened = false;
        };

        model.openPopupEndDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            model.calendar.startDateOpened = false;
            model.calendar.endDateOpened = true;
            model.calendar.attendanceStartDateOpened = false;
            model.calendar.attendanceEndDateOpened = false;
        };

        model.openPopupAttendanceStartDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            model.calendar.startDateOpened = false;
            model.calendar.endDateOpened = false;
            model.calendar.attendanceStartDateOpened = true;
            model.calendar.attendanceEndDateOpened = false;
        };

        model.openPopupAttendanceEndDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            model.calendar.startDateOpened = false;
            model.calendar.endDateOpened = false;
            model.calendar.attendanceStartDateOpened = false;
            model.calendar.attendanceEndDateOpened = true;
        };

        model.resetCalendar = function (vm) {
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

        model.openFlowPopupStartDate = function (vm, event, evaluatorType) {
            event.preventDefault();
            event.stopPropagation();
            model.resetCalendar(vm);

            switch (evaluatorType) {
                case model.evaluatorType.HumanResources:
                    vm.flowCalendar.humanResource.startDateOpened = true;
                    break;
                case model.evaluatorType.SelfEvaluator:
                    vm.flowCalendar.selfEvaluator.startDateOpened = true;
                    break;
                case model.evaluatorType.Evaluator:
                    vm.flowCalendar.evaluator.startDateOpened = true;
                    break;
                case model.evaluatorType.Reviewer:
                    vm.flowCalendar.reviewer.startDateOpened = true;
                    break;
                case model.evaluatorType.Management:
                    vm.flowCalendar.management.startDateOpened = true;
                    break;
            }
        };

        model.openFlowPopupEndDate = function (vm, event, evaluatorType) {
            event.preventDefault();
            event.stopPropagation();
            model.resetCalendar(vm);

            switch (evaluatorType) {
                case model.evaluatorType.HumanResources:
                    vm.flowCalendar.humanResource.endDateOpened = true;
                    break;
                case model.evaluatorType.SelfEvaluator:
                    vm.flowCalendar.selfEvaluator.endDateOpened = true;
                    break;
                case model.evaluatorType.Evaluator:
                    vm.flowCalendar.evaluator.endDateOpened = true;
                    break;
                case model.evaluatorType.Reviewer:
                    vm.flowCalendar.reviewer.endDateOpened = true;
                    break;
                case model.evaluatorType.Management:
                    vm.flowCalendar.management.endDateOpened = true;
                    break;
            }
        };

        model.validateEmployeeSetting = function (employeeOptions) {
            var startDate, endDate;

            if (employeeOptions.humanResource.ids && employeeOptions.humanResource.ids.length > 0) {
                startDate = new Date(employeeOptions.humanResource.StartDate).setHours(0, 0, 0, 0);
                endDate = new Date(employeeOptions.humanResource.EndDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("StartDateGreaterThanEndDateWarning");
                    return false;
                }
            } else {
                notificationFactory.error("HumanResourcePhaseRequired");
                return false;
            }

            if (employeeOptions.selfEvaluator.ids && employeeOptions.selfEvaluator.ids.length > 0) {
                startDate = new Date(employeeOptions.selfEvaluator.StartDate).setHours(0, 0, 0, 0);
                endDate = new Date(employeeOptions.selfEvaluator.EndDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("StartDateGreaterThanEndDateWarning");
                    return false;
                }

                startDate = new Date(employeeOptions.humanResource.EndDate).setHours(0, 0, 0, 0);
                endDate = new Date(employeeOptions.selfEvaluator.StartDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("HRRangeDateGreaterThanSelfRangeDateWarning");
                    return false;
                }
            }

            if (employeeOptions.evaluator.ids && employeeOptions.evaluator.ids.length > 0) {
                startDate = new Date(employeeOptions.evaluator.StartDate).setHours(0, 0, 0, 0);
                endDate = new Date(employeeOptions.evaluator.EndDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("StartDateGreaterThanEndDateWarning");
                    return false;
                }

                startDate = new Date(employeeOptions.selfEvaluator.EndDate).setHours(0, 0, 0, 0);
                endDate = new Date(employeeOptions.evaluator.StartDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("SelfRangeDateGreaterThanEvaluatorRangeDateWarning");
                    return false;
                }
            } else {
                notificationFactory.error("EvaluatorPhaseRequired");
                return false;
            }

            if (employeeOptions.reviewer.ids && employeeOptions.reviewer.ids.length > 0) {
                startDate = new Date(employeeOptions.reviewer.StartDate).setHours(0, 0, 0, 0);
                endDate = new Date(employeeOptions.reviewer.EndDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("StartDateGreaterThanEndDateWarning");
                    return false;
                }

                startDate = new Date(employeeOptions.evaluator.EndDate).setHours(0, 0, 0, 0);
                endDate = new Date(employeeOptions.reviewer.StartDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("EvaluatorRangeDateGreaterThanReviewerRangeDateWarning");
                    return false;
                }
            } else {
                notificationFactory.error("ReviewerPhaseRequired");
                return false;
            }

            if (employeeOptions.management.ids && employeeOptions.management.ids.length > 0) {
                startDate = new Date(employeeOptions.management.StartDate).setHours(0, 0, 0, 0);
                endDate = new Date(employeeOptions.management.EndDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("StartDateGreaterThanEndDateWarning");
                    return false;
                }

                startDate = new Date(employeeOptions.reviewer.EndDate).setHours(0, 0, 0, 0);
                endDate = new Date(employeeOptions.management.StartDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("ReviewerRangeDateGreaterThanManagementRangeDateWarning");
                    return false;
                }
            }

            if (employeeOptions.disciplineStartDate && employeeOptions.disciplineEndDate) {
                startDate = new Date(employeeOptions.disciplineStartDate).setHours(0, 0, 0, 0);
                endDate = new Date(employeeOptions.disciplineStartDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("DisciplineStartDateGreaterThanEndDateWarning");
                    return false;
                }
            }

            return true;
        };

        model.getPeriod = function (targetTypeId, isActive) {
            return periodFactory.getPeriod(targetTypeId, isActive).success(function (periods) {
                model.periods = periods;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.openPopupCreatePeriod = function (vm) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/Period/Templates/create.html',
                controller: "CreatePeriodController as createPeriodCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center period-create-popup"
            });

            return vm.modalInstance.result;
        };

        model.validatePeriod = function (period) {
            if (!period) {
                return false;
            } else {
                if (!period.Name) {
                    notificationFactory.error("NameRequired");
                    return false;
                } else if (!period.TargetTypeId) {
                    notificationFactory.error("TargetTypeRequired");
                    return false;
                } else if (period.TargetTypeId === model.targetType.SemiAnnual || period.TargetTypeId === model.targetType.Promotion) {
                    if (!period.RawStartDate) {
                        notificationFactory.error("StartDatePeriodRequired");
                        return false;
                    }

                    if (period.TargetTypeId === model.targetType.SemiAnnual) {
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

        model.createPeriod = function (period, modalInstance) {
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

        model.updateBasicInformationPeriod = function (period) {
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

        model.getPeriodById = function (periodId) {
            return periodFactory.getPeriodById(periodId).success(function (period) {
                model.period = period;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.openPopupUpdateStatusPeriod = function (period) {
            period.IsActive = !period.IsActive;

            var action = period.IsActive ? "activate" : "deactivate",
            newPeriod = {
                PeriodId: period.PeriodId,
                IsActive: period.IsActive
            };

            var modalInstance = $uibModal.open({
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

        model.openPopupDeletePeriod = function (vm, period) {
            var modalInstance = $uibModal.open({
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

        model.getPeriodEmployeeSettingById = function (
            periodId,
            departmentId,
            isLocal,
            isExcludedEmployee,
            isGeneratedEPA,
            isEvaluated,
            hasMonthlyEPA,
            epaStatus) {
            return periodFactory.getPeriodEmployeeSettingById(
                periodId,
                departmentId,
                isLocal,
                isExcludedEmployee,
                isGeneratedEPA,
                isEvaluated,
                hasMonthlyEPA,
                epaStatus)
            .success(function (employeeSettings) {
                model.employeeSettings = employeeSettings;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.addEmployeeToPeriod = function (employeeInfo) {
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

        model.openPopupUpdateEmployeeSetting = function (vm, selectedEmployees) {
            var modalInstance = $uibModal.open({
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

        model.updateEmployeeSettingOfPeriod = function (employeeSetting) {
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

        model.createAssignmentSettings = function (employeeOptions, periodId, selectedEmployees, employeeIds) {
            var employeeAssignments = [],
                targetTemplateId = employeeOptions.targetTemplateId;

            angular.forEach(selectedEmployees, function (employee) {
                if (employeeOptions.humanResource.ids && employeeOptions.humanResource.ids.length > 0) {
                    angular.forEach(employeeOptions.humanResource.ids, function (humanResourceId, index) {
                        var humanResourceAssignment = {
                            employeeId: employee.Employee.EmployeeId,
                            processId: model.evaluatorType.HumanResources,
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
                    processId: model.evaluatorType.SelfEvaluator,
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
                            processId: model.evaluatorType.Evaluator,
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
                            processId: model.evaluatorType.Reviewer,
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
                            processId: model.evaluatorType.Management,
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
                employeeIds: employeeIds,
                disciplineStartDate: employeeOptions.disciplineStartDate ? employeeOptions.disciplineStartDate.toDateString() : null,
                disciplineEndDate: employeeOptions.disciplineEndDate ? employeeOptions.disciplineEndDate.toDateString() : null
            };

            return employeeSetting;
        };

        model.openPopupIncludeExcludeEmployee = function (vm, periodId, employee) {
            employee.IsIncluded = !employee.IsIncluded;

            var action = employee.IsIncluded ? "include" : "exclude",
            newEmployeeStatus = {
                PeriodId: periodId,
                EmployeeId: employee.Employee.EmployeeId,
                IsIncluded: employee.IsIncluded
            };

            var modalInstance = $uibModal.open({
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

        model.getSelectedEmployee = function (employeeSettings) {
            var selectedEmployees = [];

            angular.forEach(employeeSettings, function (employeeSetting) {
                if (employeeSetting.Selected) {
                    selectedEmployees.push(employeeSetting);
                }
            });

            return selectedEmployees;
        };

        model.getSelectedEmployeeInfo = function (employeeSettings) {
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

        model.openPopupGenerateEvaluation = function (vm, period) {
            var modalInstance = $uibModal.open({
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

        model.getProcessSettingByPeriodId = function (periodId) {
            return periodFactory.getProcessSettingByPeriodId(periodId).success(function (processSettings) {
                model.processSettings = processSettings;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.updatePeriodProcessSetting = function (periodProcessSetting) {
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

        model.checkSelfProcessStarted = function (periodId, employeeIds) {
            return periodFactory.checkSelfProcessStarted(periodId, employeeIds).success(function (hasSelfStarted) {
                model.hasSelfStarted = hasSelfStarted;
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
                            ProcessId: model.evaluatorType.HumanResources,
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
                            ProcessId: model.evaluatorType.Evaluator,
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
                            ProcessId: model.evaluatorType.Reviewer,
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
                            ProcessId: model.evaluatorType.Management,
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

        model.updateEmployeeSettings = function (employeeOptions, fieldEdit, selectedEmployees) {
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

                if (fieldEdit.disciplineStartDate) {
                    employee.DisciplineStartDate = employeeOptions.disciplineStartDate;
                }

                if (fieldEdit.disciplineEndDate) {
                    employee.DisciplineEndDate = employeeOptions.disciplineEndDate;
                }
            });

            return selectedEmployees;
        };

        model.validateEmployeeSettingMultipleMode = function (updatedEmployees) {
            var employee = updatedEmployees[0],
                startDate, endDate;

            if (employee.HumanResources && employee.HumanResources.length > 0) {
                startDate = new Date(employee.HumanResources[0].StartDate).setHours(0, 0, 0, 0);
                endDate = new Date(employee.HumanResources[0].EndDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("StartDateGreaterThanEndDateWarning");
                    return false;
                }
            } else {
                notificationFactory.error("HumanResourcePhaseRequired");
                return false;
            }

            if (employee.SelfEvaluators && employee.SelfEvaluators.length > 0) {
                startDate = new Date(employee.SelfEvaluators[0].StartDate).setHours(0, 0, 0, 0);
                endDate = new Date(employee.SelfEvaluators[0].EndDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("StartDateGreaterThanEndDateWarning");
                    return false;
                }

                startDate = new Date(employee.HumanResources[0].EndDate).setHours(0, 0, 0, 0);
                endDate = new Date(employee.SelfEvaluators[0].StartDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("HRRangeDateGreaterThanSelfRangeDateWarning");
                    return false;
                }
            }

            if (employee.Evaluators && employee.Evaluators.length > 0) {
                startDate = new Date(employee.Evaluators[0].StartDate).setHours(0, 0, 0, 0);
                endDate = new Date(employee.Evaluators[0].EndDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("StartDateGreaterThanEndDateWarning");
                    return false;
                }

                startDate = new Date(employee.SelfEvaluators[0].EndDate).setHours(0, 0, 0, 0);
                endDate = new Date(employee.Evaluators[0].StartDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("SelfRangeDateGreaterThanEvaluatorRangeDateWarning");
                    return false;
                }
            } else {
                notificationFactory.error("EvaluatorPhaseRequired");
                return false;
            }

            if (employee.Reviewers && employee.Reviewers.length > 0) {
                startDate = new Date(employee.Reviewers[0].StartDate).setHours(0, 0, 0, 0);
                endDate = new Date(employee.Reviewers[0].EndDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("StartDateGreaterThanEndDateWarning");
                    return false;
                }

                startDate = new Date(employee.Evaluators[0].EndDate).setHours(0, 0, 0, 0);
                endDate = new Date(employee.Reviewers[0].StartDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("EvaluatorRangeDateGreaterThanReviewerRangeDateWarning");
                    return false;
                }
            } else {
                notificationFactory.error("ReviewerPhaseRequired");
                return false;
            }

            if (employee.Managements && employee.Managements.length > 0) {
                startDate = new Date(employee.Managements[0].StartDate).setHours(0, 0, 0, 0);
                endDate = new Date(employee.Managements[0].EndDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("StartDateGreaterThanEndDateWarning");
                    return false;
                }

                startDate = new Date(employee.Reviewers[0].EndDate).setHours(0, 0, 0, 0);
                endDate = new Date(employee.Managements[0].StartDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("ReviewerRangeDateGreaterThanManagementRangeDateWarning");
                    return false;
                }
            }

            if (employee.DisciplineStartDate && employee.DisciplineEndDate) {
                startDate = new Date(employee.DisciplineStartDate).setHours(0, 0, 0, 0);
                endDate = new Date(employee.DisciplineStartDate).setHours(0, 0, 0, 0);

                if (startDate > endDate) {
                    notificationFactory.error("DisciplineStartDateGreaterThanEndDateWarning");
                    return false;
                }
            }

            return true;
        };

        model.createAssignmentSettingsMultipleMode = function (updatedEmployees, vm) {
            var employeeAssignments = [];

            angular.forEach(updatedEmployees, function (employee) {
                if (employee.HumanResources && employee.HumanResources.length > 0) {
                    angular.forEach(employee.HumanResources, function (humanResource, index) {
                        var humanResourceAssignment = {
                            employeeId: employee.Employee.EmployeeId,
                            processId: model.evaluatorType.HumanResources,
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
                    processId: model.evaluatorType.SelfEvaluator,
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
                            processId: model.evaluatorType.Evaluator,
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
                            processId: model.evaluatorType.Reviewer,
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
                            processId: model.evaluatorType.Management,
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
                disciplineStartDate: vm.fieldEdit.disciplineStartDate && vm.employeeOptions.disciplineStartDate ? vm.employeeOptions.disciplineStartDate.toDateString() : null,
                disciplineEndDate: vm.fieldEdit.disciplineEndDate && vm.employeeOptions.disciplineEndDate ? vm.employeeOptions.disciplineEndDate.toDateString() : null,
                employeeAssignments: employeeAssignments
            };

            return employeeSetting;
        };

        model.checkHasFieldEdit = function (fieldEdit) {
            var hasEdited = false;

            if (fieldEdit.targetTemplate
                || fieldEdit.evaluator
                || fieldEdit.startDate
                || fieldEdit.endDate
                || fieldEdit.disciplineStartDate
                || fieldEdit.disciplineEndDate) {
                hasEdited = true;
            } else  {
                notificationFactory.error("PeriodEditSettingWarning");
            }

            return hasEdited;
        };

        model.validateEmployeeSettingGenerateEPA = function (employeeSettings) {
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

        return model;
    }
})(angular);