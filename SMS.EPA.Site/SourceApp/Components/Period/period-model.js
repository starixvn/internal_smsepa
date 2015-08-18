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
    periodModel.evaluatorType = {
        SelfEvaluator: 2,
        Evaluator: 3,
        Reviewer: 4,
        Management: 5
    };

    periodModel.calendar = {
        startDateOpened: false,
        dateFormat: periodModel.periodDateFormat,
        minDate: new Date()
    };

    periodModel.openPopupStartDate = function (event) {
        event.preventDefault();
        event.stopPropagation();

        periodModel.calendar.startDateOpened = true;
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
            windowClass: "modal-confirm-center"
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
    }

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

    periodModel.getPeriodEmployeeSettingById = function (periodId, startDate) {
        return periodFactory.getPeriodEmployeeSettingById(periodId, startDate).success(function (employeeSettings) {
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

    periodModel.openPopupUpdateEmployeeSetting = function (employeeSetting) {
        var modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/Period/Templates/editEmployeeSetting.html',
            controller: "UpdateEmployeeSettingPeriodController as updatePeriodSettingCtrl",
            backdrop: 'static',
            resolve: {
                employeeSetting: function () {
                    return angular.copy(employeeSetting);
                }
            },
            windowClass: "modal-confirm-center employee-period-setting"
        });

        modalInstance.result.then(function (editEmployeeSetting) {
            if (editEmployeeSetting) {
                employeeSetting.TargetTemplate = editEmployeeSetting.TargetTemplate;
                employeeSetting.Evaluators = editEmployeeSetting.Evaluators;
                employeeSetting.Reviewers = editEmployeeSetting.Reviewers;
                employeeSetting.Managements = editEmployeeSetting.Managements;
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

    periodModel.createAssignmentSettings = function (employeeOptions, employeeSetting) {
        var employeeAssignments = [],
            periodId = employeeSetting.PeriodId,
            employeeId = employeeSetting.EmployeeId,
            targetTemplateId = employeeOptions.targetTemplateId;

        var selfEvaluatorAssignments = {
            processId: periodModel.evaluatorType.SelfEvaluator,
            duration: employeeOptions.selfEvaluator.Duration,
            remindDay: employeeOptions.selfEvaluator.RemindDay,
            hasSendMail: employeeOptions.selfEvaluator.HasSendMail,
            evaluatorInfo: {
                employeeId: -1
            }
        };
        employeeAssignments.push(selfEvaluatorAssignments);

        if (employeeOptions.evaluator.ids && employeeOptions.evaluator.ids.length > 0) {
            angular.forEach(employeeOptions.evaluator.ids, function (evaluatorId) {
                var evaluatorAssignment = {
                        processId: periodModel.evaluatorType.Evaluator,
                        duration: employeeOptions.evaluator.Duration,
                        remindDay: employeeOptions.evaluator.RemindDay,
                        hasSendMail: employeeOptions.evaluator.HasSendMail,
                        evaluatorInfo: {
                            employeeId: evaluatorId
                        }
                };

                employeeAssignments.push(evaluatorAssignment);
            });
        }

        if (employeeOptions.reviewer.ids && employeeOptions.reviewer.ids.length > 0) {
            angular.forEach(employeeOptions.reviewer.ids, function (reviewerId) {
                var reviewerAssignment = {
                        processId: periodModel.evaluatorType.Reviewer,
                        duration: employeeOptions.reviewer.Duration,
                        remindDay: employeeOptions.reviewer.RemindDay,
                        hasSendMail: employeeOptions.reviewer.HasSendMail,
                        evaluatorInfo: {
                            employeeId: reviewerId
                        }
                };

                employeeAssignments.push(reviewerAssignment);
            });
        }

        if (employeeOptions.management.ids && employeeOptions.management.ids.length > 0) {
            angular.forEach(employeeOptions.management.ids, function (managementId) {
                var managementAssignment = {
                    processId: periodModel.evaluatorType.Management,
                    duration: employeeOptions.management.Duration,
                    remindDay: employeeOptions.management.RemindDay,
                    hasSendMail: employeeOptions.management.HasSendMail,
                    evaluatorInfo: {
                        employeeId: managementId
                    }
                };

                employeeAssignments.push(managementAssignment);
            });
        }

        var employeeSetting = {
            periodId: periodId,
            employeeId: employeeId,
            targetTemplateId: targetTemplateId,
            employeeAssignments: employeeAssignments
        };

        return employeeSetting;
    };

    return periodModel;
}]);