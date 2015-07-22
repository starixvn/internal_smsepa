'use strict';
/*global moment, flowMngModule*/

flowMngModule.factory('FlowMngModel',
['$modal', 'flowMngFactory', 'notificationFactory', 'errorFactory',
function ($modal, flowMngFactory, notificationFactory, errorFactory) {
    var flowMngModel = {};
    var date = moment(new Date()).format("DD MMM, YYYY");

    flowMngModel.selfs = {
        startDate: date,
        endDate: date,
        hasSendMail: true,
        includeSelfEvaluation: true,
        employees: []
    };
    flowMngModel.evaluators = {
        startDate: date,
        endDate: date,
        hasSendMail: true,
        includeSelfEvaluation: true,
        employees: []
    };
    flowMngModel.reviewers = {
        startDate: date,
        endDate: date,
        hasSendMail: true,
        includeSelfEvaluation: true,
        employees: []
    };
    flowMngModel.managements = {
        startDate: date,
        endDate: date,
        hasSendMail: true,
        includeSelfEvaluation: true,
        employees: []
    };
    flowMngModel.humanResources = {
        startDate: date,
        endDate: date,
        hasSendMail: true,
        includeSelfEvaluation: true,
        employees: []
    };

    flowMngModel.employeeSettings = [];

    flowMngModel.resetCurrentFlow = function (vm) {
        vm.selfs.startDate = date;
        vm.selfs.endDate = date;
        vm.selfs.hasSendMail = true;
        vm.selfs.includeSelfEvaluation = true;
        vm.selfs.employees = [];

        vm.evaluators.startDate = date;
        vm.evaluators.endDate = date;
        vm.evaluators.hasSendMail = true;
        vm.evaluators.includeSelfEvaluation = true;
        vm.evaluators.employees = [];

        vm.reviewers.startDate = date;
        vm.reviewers.endDate = date;
        vm.reviewers.hasSendMail = true;
        vm.reviewers.includeSelfEvaluation = true;
        vm.reviewers.employees = [];

        vm.managements.startDate = date;
        vm.managements.endDate = date;
        vm.managements.hasSendMail = true;
        vm.managements.includeSelfEvaluation = true;
        vm.managements.employees = [];

        vm.humanResources.startDate = date;
        vm.humanResources.endDate = date;
        vm.humanResources.hasSendMail = true;
        vm.humanResources.includeSelfEvaluation = true;
        vm.humanResources.employees = [];
    };

    /// Setting calendar.
    flowMngModel.calendar = function (vm) {
        vm.calendar = {
            selfs: {
                startDateOpened: false,
                endDateOpened: false
            },
            evaluators: {
                startDateOpened: false,
                endDateOpened: false
            },
            reviewers: {
                startDateOpened: false,
                endDateOpened: false
            },
            managements: {
                startDateOpened: false,
                endDateOpened: false
            },
            humanResources: {
                startDateOpened: false,
                endDateOpened: false
            }
        };
    };

    flowMngModel.resetOpenCalendar = function (vm) {
        vm.calendar.selfs.startDateOpened = false;
        vm.calendar.selfs.endDateOpened = false;

        vm.calendar.evaluators.startDateOpened = false;
        vm.calendar.evaluators.endDateOpened = false;

        vm.calendar.reviewers.startDateOpened = false;
        vm.calendar.reviewers.endDateOpened = false;

        vm.calendar.managements.startDateOpened = false;
        vm.calendar.managements.endDateOpened = false;

        vm.calendar.humanResources.startDateOpened = false;
        vm.calendar.humanResources.endDateOpened = false;
    };

    /// Ignore/Include Self-Evaluation.
    flowMngModel.openPopupConfirmIncludeSelfEvaluation = function (flow) {
        var name = "Self-Evaluation",
            action = flow.includeSelfEvaluation ? "ignore" : "include";

        var modalInstance = $modal.open({
            templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return name;
                },
                action: function () {
                    return action;
                }
            },
            windowClass: "modal-confirm-center"
        });

        modalInstance.result.then(function () {
            flow.includeSelfEvaluation = !flow.includeSelfEvaluation;
        })['finally'](function () {
            modalInstance = '';
        });
    };

    /// Get employee setting flow in department.
    flowMngModel.getEmployeeSettingFlowInDepartment = function (departmentId) {
        return flowMngFactory.getEmployeeSettingFlowInDepartment(departmentId)
        .success(function (employeeSettings) {
            flowMngModel.employeeSettings = employeeSettings;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    var successGetFlow = function (flows) {
        flowMngModel.resetCurrentFlow(flowMngModel);

        angular.forEach(flows, function (flow) {
            var processId = flow.ProcessId;
            switch (processId) {
                case 2:
                    flowMngModel.selfs.startDate = flow.StartDate;
                    flowMngModel.selfs.endDate = flow.EndDate;
                    flowMngModel.selfs.hasSendMail = flow.HasSendMail;
                    flowMngModel.selfs.includeSelfEvaluation = flow.IncludeSelfEvaluation;
                    if (flow.EvaluatorInfo.EmployeeId > 0) {
                        flowMngModel.selfs.employees.push(flow.EvaluatorInfo);
                    }
                    break;
                case 3:
                    flowMngModel.evaluators.startDate = flow.StartDate;
                    flowMngModel.evaluators.endDate = flow.EndDate;
                    flowMngModel.evaluators.hasSendMail = flow.HasSendMail;
                    flowMngModel.evaluators.includeSelfEvaluation = flow.IncludeSelfEvaluation;
                    if (flow.EvaluatorInfo.EmployeeId > 0) {
                        flowMngModel.evaluators.employees.push(flow.EvaluatorInfo);
                    }
                    break;
                case 4:
                    flowMngModel.reviewers.startDate = flow.StartDate;
                    flowMngModel.reviewers.endDate = flow.EndDate;
                    flowMngModel.reviewers.hasSendMail = flow.HasSendMail;
                    flowMngModel.reviewers.includeSelfEvaluation = flow.IncludeSelfEvaluation;
                    if (flow.EvaluatorInfo.EmployeeId > 0) {
                        flowMngModel.reviewers.employees.push(flow.EvaluatorInfo);
                    }
                    break;
                case 5:
                    flowMngModel.managements.startDate = flow.StartDate;
                    flowMngModel.managements.endDate = flow.EndDate;
                    flowMngModel.managements.hasSendMail = flow.HasSendMail;
                    flowMngModel.managements.includeSelfEvaluation = flow.IncludeSelfEvaluation;
                    if (flow.EvaluatorInfo.EmployeeId > 0) {
                        flowMngModel.managements.employees.push(flow.EvaluatorInfo);
                    }
                    break;
                case 6:
                    flowMngModel.humanResources.startDate = flow.StartDate;
                    flowMngModel.humanResources.endDate = flow.EndDate;
                    flowMngModel.humanResources.hasSendMail = flow.HasSendMail;
                    flowMngModel.humanResources.includeSelfEvaluation = flow.IncludeSelfEvaluation;
                    if (flow.EvaluatorInfo.EmployeeId > 0) {
                        flowMngModel.humanResources.employees.push(flow.EvaluatorInfo);
                    }
                    break;
            }
        });
    };

    /// Get flow.
    flowMngModel.getFlow = function (department, flowType) {
        return flowMngFactory.getFlow(department.DepartmentId, flowType)
        .success(function (flows) {
            successGetFlow(flows);
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Get flow employee.
    flowMngModel.getFlowEmployee = function (employeeId) {
        return flowMngFactory.getFlowEmployee(employeeId)
        .success(function (flows) {
            successGetFlow(flows);
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Remove item from list.
    flowMngModel.removeItemFromList = function (item, list) {
        var index = list.indexOf(item);
        if (index !== -1) {
            list.splice(index, 1);
        }
    };

    /// Validate add employee.
    flowMngModel.addEmployee = function (listIdentity, vm, employee, position) {
        var list, allowAdd = true;

        switch (listIdentity) {
            case "evaluatorList":
                list = vm.evaluators.employees;
                break;
            case "reviewerList":
                list = vm.reviewers.employees;
                break;
            case "managementList":
                list = vm.managements.employees;
                break;
            case "humanResourceList":
                list = vm.humanResources.employees;
                break;
        }

        $.each(list, function (i) {
            if (list[i].EmployeeId === employee.EmployeeId) {
                allowAdd = false;
            }
        });

        if (allowAdd) {
            if (position != null) {
                list.splice(position, 0, employee);
            }
            else {
                list.push(employee);
            }
        }
        else {
            notificationFactory.error('', 'This employee has already exists!');
        }
    };

    /// Set flow.
    flowMngModel.createFlow = function (vm) {
        if (!vm.flowType) {
            notificationFactory.error('', 'Please select a department!');
        }
        else {
            var model = {
                departmentId: vm.department.DepartmentId,
                flowType: vm.flowType,
                selfs: vm.selfs,
                evaluators: vm.evaluators,
                reviewers: vm.reviewers,
                managements: vm.managements,
                humanResources: vm.humanResources
            };

            return flowMngFactory.createFlow(model)
            .success(function (response) {
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
        }
    };

    /// Set employee flow.
    flowMngModel.createEmployeeFlow = function (vm) {
        if (!vm.currentSelectedEmployee) {
            notificationFactory.error('', 'Please select an employee!');
        } else {
            var model = {
                selfs: vm.selfs,
                evaluators: vm.evaluators,
                reviewers: vm.reviewers,
                managements: vm.managements,
                humanResources: vm.humanResources
            };

            return flowMngFactory.createEmployeeFlow(model)
            .success(function (response) {
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
        }
    };

    /// Delete flow.
    flowMngModel.openPopupDeleteFlow = function (vm) {
        if (!vm.department || !vm.department.DepartmentId) {
            notificationFactory.error('', 'Please select a department!');
        }
        else {
            var modalInstance = $modal.open({
                templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: "ModalConfirmController as modalConfirmCtrl",
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return '';
                    },
                    action: function () {
                        return 'delete';
                    }
                },
                windowClass: "modal-confirm-center"
            });

            modalInstance.result.then(function () {
                flowMngFactory.deleteFlow(vm.department.DepartmentId, vm.flowType).success(function (response) {
                    if (response.IsSuccessful) {
                        notificationFactory.success("DeletedSuccessfully");

                        vm.department = null;
                        flowMngModel.resetCurrentFlow(vm);
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
        }
    };

    return flowMngModel;
} ]);