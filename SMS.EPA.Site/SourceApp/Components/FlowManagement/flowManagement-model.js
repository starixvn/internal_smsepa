'use strict';
/*global moment, flowMngModule*/

flowMngModule.factory('FlowMngModel',
['$modal', 'flowMngFactory', 'notificationFactory', 'errorFactory',
function ($modal, flowMngFactory, notificationFactory, errorFactory) {
    var flowMngModel = {};
    var date = moment(new Date()).format("DD MMM, YYYY");

    flowMngModel.selfs = {
        duration: 0,
        hasSendMail: true,
        employees: []
    };
    flowMngModel.evaluators = {
        duration: 0,
        hasSendMail: true,
        employees: []
    };
    flowMngModel.reviewers = {
        duration: 0,
        hasSendMail: true,
        employees: []
    };
    flowMngModel.managements = {
        duration: 0,
        hasSendMail: true,
        employees: []
    };
    flowMngModel.humanResources = {
        duration: 0,
        hasSendMail: true,
        employees: []
    };

    flowMngModel.flowTemplates = [];
    flowMngModel.flowTemplate = {};
    flowMngModel.defaultSettingFlowEmployees = [];
    flowMngModel.personalSettingFlowEmployees = [];

    flowMngModel.resetCurrentFlow = function (vm) {
        vm.selfs.duration = 0;
        vm.selfs.hasSendMail = true;
        vm.selfs.employees = [];

        vm.evaluators.duration = 0;
        vm.evaluators.hasSendMail = true;
        vm.evaluators.employees = [];

        vm.reviewers.duration = 0;
        vm.reviewers.hasSendMail = true;
        vm.reviewers.employees = [];

        vm.managements.duration = 0;
        vm.managements.hasSendMail = true;
        vm.managements.employees = [];

        vm.humanResources.duration = 0;
        vm.humanResources.hasSendMail = true;
        vm.humanResources.employees = [];
    };

    /// Get flow template.
    flowMngModel.getFlowTemplate = function (targetTypeId, isActive) {
        return flowMngFactory.getFlowTemplate(targetTypeId, isActive)
        .success(function (flowTemplates) {
            flowMngModel.flowTemplates = flowTemplates;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Get flow template by Id.
    flowMngModel.getFlowTemplateById = function (flowTemplateId) {
        return flowMngFactory.getFlowTemplateById(flowTemplateId)
        .success(function (flowTemplate) {
            flowMngModel.flowTemplate = flowTemplate;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    flowMngModel.validateFlowTemplate = function (flowTemplate) {
        if (!flowTemplate) {
            return false;
        }
        else {
            if (!flowTemplate.Name) {
                notificationFactory.error("NameRequire");
                return false;
            }
            else if (!flowTemplate.TargetTypeId) {
                notificationFactory.error("FormRequire");
                return false;
            }
        }

        return true;
    };

    flowMngModel.createFlowTemplate = function (flowTemplate) {
        return flowMngFactory.createFlowTemplate(flowTemplate).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("CreateSuccessfully");
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function () {
            errorFactory.errorHandle(status);
        });
    };

    flowMngModel.updateFlowTemplate = function (flowTemplate) {
        return flowMngFactory.updateFlowTemplate(flowTemplate).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdateSuccessfully");
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function () {
            errorFactory.errorHandle(status);
        });
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
                    flowMngModel.humanResources.duration = flow.Duration;
                    flowMngModel.humanResources.hasSendMail = flow.HasSendMail;
                    if (flow.EvaluatorInfo.EmployeeId > 0) {
                        flowMngModel.humanResources.employees.push(flow.EvaluatorInfo);
                    }
                    break;
                case 2:
                    var object;
                    flowMngModel.selfs.duration = flow.Duration;
                    flowMngModel.selfs.hasSendMail = flow.HasSendMail;
                    if (flow.EvaluatorInfo.EmployeeId === -1) {
                        object = { NickName: "Self", EmployeeId: -1 };
                    }
                    else if (flow.EvaluatorInfo.EmployeeId > 0) {
                        object = flow.EvaluatorInfo;
                    }
                    flowMngModel.selfs.employees.push(object);
                    break;
                case 3:
                    flowMngModel.evaluators.duration = flow.Duration;
                    flowMngModel.evaluators.hasSendMail = flow.HasSendMail;
                    if (flow.EvaluatorInfo.EmployeeId > 0) {
                        flowMngModel.evaluators.employees.push(flow.EvaluatorInfo);
                    }
                    break;
                case 4:
                    flowMngModel.reviewers.duration = flow.Duration;
                    flowMngModel.reviewers.hasSendMail = flow.HasSendMail;
                    if (flow.EvaluatorInfo.EmployeeId > 0) {
                        flowMngModel.reviewers.employees.push(flow.EvaluatorInfo);
                    }
                    break;
                case 5:
                    flowMngModel.managements.duration = flow.Duration;
                    flowMngModel.managements.hasSendMail = flow.HasSendMail;
                    if (flow.EvaluatorInfo.EmployeeId > 0) {
                        flowMngModel.managements.employees.push(flow.EvaluatorInfo);
                    }
                    break;
            }
        });
    };

    /// Get department flow.
    flowMngModel.getDepartmentFlow = function (department, flowTemplateId) {
        return flowMngFactory.getDepartmentFlow(department.DepartmentId, flowTemplateId)
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

    flowMngModel.validateDepartmentFlow = function (vm) {
        if (!vm.department || !vm.department.DepartmentId) {
            notificationFactory.error("DepartmentNotSelectedWarning");
            return false;
        }
        else if (vm.humanResources.employees.length === 0) {
            notificationFactory.error('', 'Please setting flow for Human Resource.');
            return false;
        }
        else if(vm.evaluators.employees.length === 0 &&
            vm.reviewers.employees.length === 0 &&
            vm.managements.employees.length === 0) {
            notificationFactory.error('', 'Please setting flow for Evaluator or Reviewer or Management or Human Resource.');
            return false;
        }
        
        return true;
    };

    /// Set department flow.
    flowMngModel.createDepartmentFlow = function (vm) {
        var model = {
            departmentId: vm.department.DepartmentId,
            flowTemplateId: vm.FlowTemplateId,
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
    };

    flowMngModel.validatePersonalFlow = function (vm) {
        if (!vm.employeeId) {
            notificationFactory.error("EmployeeNotSelectedWarning");
            return false;
        }
        else if (vm.evaluators.employees.length === 0 &&
            vm.reviewers.employees.length === 0 &&
            vm.managements.employees.length === 0 &&
            vm.humanResources.employees.length === 0) {
            notificationFactory.error('', 'Please setting flow for Evaluator or Reviewer or Management or Human Resource.');
            return false;
        }

        return true;
    };

    /// Set personal flow.
    flowMngModel.createPersonalFlow = function (vm) {
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
        var index = -1;

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
            flowMngFactory.deletePersonalFlow(vm.employeeId).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("DeletedSuccessfully");

                    index = vm.personalSettingFlowEmployees.map(function (e) {
                        return e.EmployeeId;
                    }).indexOf(vm.employeeId);

                    if (index !== -1) {
                        vm.defaultSettingFlowEmployees.push(vm.personalSettingFlowEmployees[index]);
                        vm.personalSettingFlowEmployees.splice(index, 1);
                    }

                    vm.employeeId = null;
                    vm.showDeleteButton = false;
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