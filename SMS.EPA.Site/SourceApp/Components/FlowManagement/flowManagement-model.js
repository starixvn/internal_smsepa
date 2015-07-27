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
        employees: []
    };
    flowMngModel.evaluators = {
        startDate: date,
        endDate: date,
        hasSendMail: true,
        employees: []
    };
    flowMngModel.reviewers = {
        startDate: date,
        endDate: date,
        hasSendMail: true,
        employees: []
    };
    flowMngModel.managements = {
        startDate: date,
        endDate: date,
        hasSendMail: true,
        employees: []
    };
    flowMngModel.humanResources = {
        startDate: date,
        endDate: date,
        hasSendMail: true,
        employees: []
    };

    flowMngModel.defaultSettingFlowEmployees = [];
    flowMngModel.personalSettingFlowEmployees = [];

    flowMngModel.resetCurrentFlow = function (vm) {
        vm.selfs.startDate = date;
        vm.selfs.endDate = date;
        vm.selfs.hasSendMail = true;
        vm.selfs.employees = [];

        vm.evaluators.startDate = date;
        vm.evaluators.endDate = date;
        vm.evaluators.hasSendMail = true;
        vm.evaluators.employees = [];

        vm.reviewers.startDate = date;
        vm.reviewers.endDate = date;
        vm.reviewers.hasSendMail = true;
        vm.reviewers.employees = [];

        vm.managements.startDate = date;
        vm.managements.endDate = date;
        vm.managements.hasSendMail = true;
        vm.managements.employees = [];

        vm.humanResources.startDate = date;
        vm.humanResources.endDate = date;
        vm.humanResources.hasSendMail = true;
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

    /// Get employee setting flow in department.
    flowMngModel.getEmployeeSettingFlowInDepartment = function (departmentId) {
        return flowMngFactory.getEmployeeSettingFlowInDepartment(departmentId)
        .success(function (data) {
            flowMngModel.defaultSettingFlowEmployees = data.DefaultFlowSettingEmployees;
            flowMngModel.personalSettingFlowEmployees = data.PersonalFlowSettingEmployees;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    var successGetFlow = function (flows) {
        angular.forEach(flows, function (flow) {
            var processId = flow.ProcessId;
            switch (processId) {
                case 1:
                    var object;
                    flowMngModel.selfs.startDate = flow.StartDate;
                    flowMngModel.selfs.endDate = flow.EndDate;
                    flowMngModel.selfs.hasSendMail = flow.HasSendMail;
                    if (flow.EvaluatorInfo.EmployeeId === -1) {
                        object = { NickName: "Self", EmployeeId: -1 };
                    }
                    else if (flow.EvaluatorInfo.EmployeeId > 0) {
                        object = flow.EvaluatorInfo;
                    }
                    flowMngModel.selfs.employees.push(object);
                    break;
                case 2:
                    flowMngModel.evaluators.startDate = flow.StartDate;
                    flowMngModel.evaluators.endDate = flow.EndDate;
                    flowMngModel.evaluators.hasSendMail = flow.HasSendMail;
                    if (flow.EvaluatorInfo.EmployeeId > 0) {
                        flowMngModel.evaluators.employees.push(flow.EvaluatorInfo);
                    }
                    break;
                case 3:
                    flowMngModel.reviewers.startDate = flow.StartDate;
                    flowMngModel.reviewers.endDate = flow.EndDate;
                    flowMngModel.reviewers.hasSendMail = flow.HasSendMail;
                    if (flow.EvaluatorInfo.EmployeeId > 0) {
                        flowMngModel.reviewers.employees.push(flow.EvaluatorInfo);
                    }
                    break;
                case 4:
                    flowMngModel.managements.startDate = flow.StartDate;
                    flowMngModel.managements.endDate = flow.EndDate;
                    flowMngModel.managements.hasSendMail = flow.HasSendMail;
                    if (flow.EvaluatorInfo.EmployeeId > 0) {
                        flowMngModel.managements.employees.push(flow.EvaluatorInfo);
                    }
                    break;
                case 5:
                    flowMngModel.humanResources.startDate = flow.StartDate;
                    flowMngModel.humanResources.endDate = flow.EndDate;
                    flowMngModel.humanResources.hasSendMail = flow.HasSendMail;
                    if (flow.EvaluatorInfo.EmployeeId > 0) {
                        flowMngModel.humanResources.employees.push(flow.EvaluatorInfo);
                    }
                    break;
            }
        });
    };

    /// Get department flow.
    flowMngModel.getDepartmentFlow = function (department) {
        return flowMngFactory.getDepartmentFlow(department.DepartmentId)
        .success(function (flows) {
            flowMngModel.resetCurrentFlow(flowMngModel);
            if (flows.length > 0) {
                successGetFlow(flows);
            }
            else {
                flowMngModel.selfs.employees = [{ NickName: "Self", EmployeeId: -1 }];
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Get flow employee.
    flowMngModel.getPersonalFlow = function (employeeId) {
        return flowMngFactory.getPersonalFlow(employeeId)
        .success(function (flows) {
            flowMngModel.resetCurrentFlow(flowMngModel);
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

    /// Set department flow.
    flowMngModel.createDepartmentFlow = function (vm) {
        if (!vm.department || !vm.department.DepartmentId) {
            notificationFactory.error('', 'Please select a department!');
        }
        else {
            var model = {
                departmentId: vm.department.DepartmentId,
                selfs: vm.selfs,
                evaluators: vm.evaluators,
                reviewers: vm.reviewers,
                managements: vm.managements,
                humanResources: vm.humanResources
            };

            return flowMngFactory.createDepartmentFlow(model)
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

    /// Set personal flow.
    flowMngModel.createPersonalFlow = function (vm) {
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

            return flowMngFactory.createPersonalFlow(model)
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

    /// Delete department flow.
    flowMngModel.openPopupDeleteDepartmentFlow = function (vm) {
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
                flowMngFactory.deleteDepartmentFlow(vm.department.DepartmentId, vm.flowType).success(function (response) {
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

    /// Delete personal flow.
    flowMngModel.openPopupDeletePersonalFlow = function (vm) {
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
            flowMngFactory.deletePersonalFlow(vm.currentSelectedEmployee).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("DeletedSuccessfully");

                    vm.currentSelectedEmployee = null;
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
    };

    return flowMngModel;
} ]);