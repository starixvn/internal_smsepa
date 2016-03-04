'use strict';
/*global monthlyPeriodModule*/

monthlyPeriodModule.factory('MonthlyPeriodModel',
['$modal', 'monthlyPeriodFactory', 'errorFactory', 'notificationFactory', 'utilitiesFactory',
function ($modal, monthlyPeriodFactory, errorFactory, notificationFactory, utilitiesFactory) {
    var monthlyPeriodModel = {};

    monthlyPeriodModel.periods = [];
    monthlyPeriodModel.period = {};
    monthlyPeriodModel.processSettings = [];
    monthlyPeriodModel.employeeSettings = [];
    monthlyPeriodModel.permission = 0;

    monthlyPeriodModel.getPermissionOnMonthlyPeriod = function () {
        return monthlyPeriodFactory.getPermissionOnMonthlyPeriod()
        .success(function (permisson) {
            monthlyPeriodModel.permission = parseInt(permisson);
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    monthlyPeriodModel.openPopupStartDate = function (event, vm) {
        event.preventDefault();
        event.stopPropagation();

        vm.calendar.startDateOpened = true;
        vm.calendar.endDateOpened = false;
    };

    monthlyPeriodModel.openPopupEndDate = function (event, vm) {
        event.preventDefault();
        event.stopPropagation();

        vm.calendar.startDateOpened = false;
        vm.calendar.endDateOpened = true;
    };

    monthlyPeriodModel.getMonthlyPeriod = function (fromDate, toDate) {
        return monthlyPeriodFactory.getMonthlyPeriod(fromDate, toDate)
        .success(function (periods) {
            monthlyPeriodModel.periods = periods;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    monthlyPeriodModel.getMonthlyPeriodById = function (periodId) {
        return monthlyPeriodFactory.getMonthlyPeriodById(periodId)
        .success(function (period) {
            monthlyPeriodModel.period = period;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    monthlyPeriodModel.openPopupCreateMonthlyPeriod = function (vm) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/MonthlyPeriod/Templates/create.html',
            controller: "CreateMonthlyPeriodController as createMonthlyPeriodCtrl",
            backdrop: 'static',
            size: 'lg',
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    monthlyPeriodModel.validateMonthlyPeriod = function (period) {
        if (!period) {
            return false;
        }
        else {
            if (!period.Name) {
                notificationFactory.error("NameRequired");
                return false;
            }
            else if (!period.StartDate) {
                notificationFactory.error("StartDatePeriodRequired");
                return false;
            } else if (!period.EndDate) {
                notificationFactory.error("EndDatePeriodRequired");
                return false;
            }
            else if (new Date(period.StartDate).setHours(0, 0, 0, 0) > new Date(period.EndDate).setHours(0, 0, 0, 0)) {
                notificationFactory.error("StartDateGreaterThanEndDateWarning");
                return false;
            }
        }

        return true;
    };

    monthlyPeriodModel.createMonthlyPeriod = function (period, modalInstance) {
        return monthlyPeriodFactory.createMonthlyPeriod(period)
        .success(function (response) {
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

    monthlyPeriodModel.updateBasicInfoMonthlyPeriod = function (period) {
        return monthlyPeriodFactory.updateBasicInfoMonthlyPeriod(period).success(function (response) {
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

    monthlyPeriodModel.getProcessSettingByPeriodId = function (periodId) {
        return monthlyPeriodFactory.getProcessSettingByPeriodId(periodId)
        .success(function (processSettings) {
            monthlyPeriodModel.processSettings = processSettings;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    monthlyPeriodModel.updateProcessSettingMonthlyPeriod = function (processSetting) {
        return monthlyPeriodFactory.updateProcessSettingMonthlyPeriod(processSetting)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
            }
            else {
                notificationFactory.error("UpdatedUnsuccessfully");
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    monthlyPeriodModel.openPopupDeleteMonthlyPeriod = function (vm, period) {
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
            monthlyPeriodFactory.deleteMonthlyPeriod(period.PeriodId).success(function (response) {
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

    monthlyPeriodModel.getEmployeeSettingMonthlyPeriodById = function (periodId, departmentId, isLocal, isExcludedEmployee, isGeneratedEPA, isEvaluated) {
        return monthlyPeriodFactory.getEmployeeSettingMonthlyPeriodById(periodId, departmentId, isLocal, isExcludedEmployee, isGeneratedEPA, isEvaluated)
        .success(function (employeeSettings) {
            monthlyPeriodModel.employeeSettings = employeeSettings;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    monthlyPeriodModel.addEmployeeToMonthlyPeriod = function (employeeInfo) {
        return monthlyPeriodFactory.addEmployeeToMonthlyPeriod(employeeInfo).success(function (response) {
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

    monthlyPeriodModel.openPopupUpdateEmployeeSetting = function (vm, selectedEmployees) {
        var modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/MonthlyPeriod/Templates/editEmployeeSetting.html',
            controller: "UpdateEmployeeSettingMonthlyPeriodController as updatePeriodSettingCtrl",
            backdrop: 'static',
            size: 'lg',
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
            windowClass: "modal-confirm-center"
        });

        modalInstance.result.then(function () {
            vm.query(vm.filter);
            vm.selectedAll = false;
        })['finally'](function () {
            modalInstance = null;
        });
    };

    monthlyPeriodModel.validateEmployeeSettingGenerateEPA = function (employeeSettings) {
        for (var i = 0; i < employeeSettings.length; i++) {
            var employeeSetting = employeeSettings[i];

            if (!employeeSetting.TargetTemplate) {
                notificationFactory.error("NoneTargetTemplateSettingWarning");
                return false;
            }
            else if (employeeSetting.HumanResources && employeeSetting.HumanResources.length > 0
                && (!employeeSetting.HumanResources[0].StartDate || !employeeSetting.HumanResources[0].EndDate)) {
                notificationFactory.error("NoneDateSettingWarning");
                return false;
            }
            else if (employeeSetting.SelfEvaluators && employeeSetting.SelfEvaluators.length > 0
                && (!employeeSetting.SelfEvaluators[0].StartDate || !employeeSetting.SelfEvaluators[0].EndDate)) {
                notificationFactory.error("NoneDateSettingWarning");
                return false;
            }
            else if (employeeSetting.Evaluators && employeeSetting.Evaluators.length > 0
                && (!employeeSetting.Evaluators[0].StartDate || !employeeSetting.Evaluators[0].EndDate)) {
                notificationFactory.error("NoneDateSettingWarning");
                return false;
            }
            else if (employeeSetting.Reviewers && employeeSetting.Reviewers.length > 0
                && (!employeeSetting.Reviewers[0].StartDate || !employeeSetting.Reviewers[0].EndDate)) {
                notificationFactory.error("NoneDateSettingWarning");
                return false;
            }
        }

        return true;
    };

    monthlyPeriodModel.openPopupGenerateEvaluation = function (vm, period) {
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
            return monthlyPeriodFactory.generateEvaluation(period)
            .success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("CreatedSuccessfully");
                }
                else {
                    notificationFactory.error('', response.Message);
                }
            })
            .error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        })['finally'](function () {
            modalInstance = '';
        });
    };

    monthlyPeriodModel.getSelectedEmployee = function (employeeSettings) {
        var selectedEmployees = [];

        angular.forEach(employeeSettings, function (employeeSetting) {
            if (employeeSetting.Selected) {
                selectedEmployees.push(employeeSetting);
            }
        });

        return selectedEmployees;
    };

    monthlyPeriodModel.getSelectedEmployeeInfo = function (employeeSettings) {
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

    monthlyPeriodModel.validateEmployeeSetting = function (employeeOptions) {
        var hasReviewer = false;

        if (employeeOptions.humanResource.ids && employeeOptions.humanResource.ids.length > 0) {
            if (new Date(employeeOptions.humanResource.StartDate).setHours(0, 0, 0, 0) > new Date(employeeOptions.humanResource.EndDate).setHours(0, 0, 0, 0)) {
                notificationFactory.error("StartDateGreaterThanEndDateWarning");
                return false;
            }
        }
        else {
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
        }
        else {
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

        return true;
    };

    monthlyPeriodModel.validateEmployeeSettingMultipleMode = function (updatedEmployees) {
        for (var i = 0; i < updatedEmployees.length; i++) {
            var employee = updatedEmployees[i],
                hasReviewer = false;

            if (employee.HumanResources && employee.HumanResources.length > 0) {
                if (new Date(employee.HumanResources[0].StartDate).setHours(0, 0, 0, 0) > new Date(employee.HumanResources[0].EndDate).setHours(0, 0, 0, 0)) {
                    notificationFactory.error("StartDateGreaterThanEndDateWarning");
                    return false;
                }
            }
            else {
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
            }
            else {
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

            return true;
        }
    };

    monthlyPeriodModel.checkHasFieldEdit = function (fieldEdit) {
        var hasEdited = false;

        if (fieldEdit.targetTemplate) {
            hasEdited = true;
        }
        else if (fieldEdit.evaluator) {
            hasEdited = true;
        }
        else if (fieldEdit.startDate) {
            hasEdited = true;
        }
        else if (fieldEdit.endDate) {
            hasEdited = true;
        }
        else {
            notificationFactory.error("PeriodEditSettingWarning");
        }

        return hasEdited;
    };

    monthlyPeriodModel.updateEmployeeSettingOfPeriod = function (employeeSetting) {
        return monthlyPeriodFactory.updateEmployeeSettingOfPeriod(employeeSetting)
        .success(function (response) {
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

    monthlyPeriodModel.openPopupIncludeExcludeEmployee = function (vm, periodId, employee) {
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
            monthlyPeriodFactory.updateIncludeStatusEmployeePeriod(newEmployeeStatus)
            .success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                }
                else {
                    employee.IsIncluded = !newEmployeeStatus.IsIncluded;
                    notificationFactory.error("UpdatedUnsuccessfully");
                }

                vm.query(vm.filter);
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

    return monthlyPeriodModel;
}]);