'use strict';
/*global moment, flowMngModule*/

flowMngModule.factory('FlowMngModel',
['$modal', 'flowMngFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory',
function ($modal, flowMngFactory, utilitiesFactory, notificationFactory, errorFactory) {
    var flowMngModel = {};

    flowMngModel.remindDay = [0, 1, 2, 3, 4, 5, 6, 7];

    flowMngModel.selfs = {
        duration: 0,
        hasSendMail: true,
        remindDay: 0,
        employees: []
    };
    flowMngModel.evaluators = {
        duration: 0,
        hasSendMail: true,
        remindDay: 0,
        employees: []
    };
    flowMngModel.reviewers = {
        duration: 0,
        hasSendMail: true,
        remindDay: 0,
        employees: []
    };
    flowMngModel.managements = {
        duration: 0,
        hasSendMail: true,
        remindDay: 0,
        employees: []
    };
    flowMngModel.humanResources = {
        duration: 0,
        hasSendMail: true,
        remindDay: 0,
        employees: []
    };

    flowMngModel.flowTemplates = [];
    flowMngModel.flowTemplate = {};
    flowMngModel.defaultFlowSettings = [];
    flowMngModel.personalFlowSettings = [];

    // #region Common

    flowMngModel.resetCurrentFlow = function (vm) {
        vm.selfs.duration = 0;
        vm.selfs.hasSendMail = true;
        vm.selfs.remindDay = 0;
        vm.selfs.employees = [];

        vm.evaluators.duration = 0;
        vm.evaluators.hasSendMail = true;
        vm.evaluators.remindDay = 0;
        vm.evaluators.employees = [];

        vm.reviewers.duration = 0;
        vm.reviewers.hasSendMail = true;
        vm.reviewers.remindDay = 0;
        vm.reviewers.employees = [];

        vm.managements.duration = 0;
        vm.managements.hasSendMail = true;
        vm.managements.remindDay = 0;
        vm.managements.employees = [];

        vm.humanResources.duration = 0;
        vm.humanResources.hasSendMail = true;
        vm.humanResources.remindDay = 0;
        vm.humanResources.employees = [];
    };

    flowMngModel.removeItemFromList = function (item, list) {
        var index = list.indexOf(item);
        if (index !== -1) {
            list.splice(index, 1);
        }
    };

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

    var successGetFlow = function (flows) {
        angular.forEach(flows, function (flow) {
            var processId = flow.ProcessId;
            switch (processId) {
                case 1:
                    flowMngModel.humanResources.duration = flow.Duration;
                    flowMngModel.humanResources.hasSendMail = flow.HasSendMail;
                    flowMngModel.humanResources.remindDay = flow.RemindDay;
                    flowMngModel.humanResources.employees.push(flow.EvaluatorInfo);
                    break;
                case 2:
                    var object;
                    flowMngModel.selfs.duration = flow.Duration;
                    flowMngModel.selfs.hasSendMail = flow.HasSendMail;
                    flowMngModel.selfs.remindDay = flow.RemindDay;
                    if (flow.EvaluatorInfo.EmployeeId === -1) {
                        object = { NickName: "Self", EmployeeId: -1 };
                    }
                    else {
                        object = flow.EvaluatorInfo;
                    }
                    flowMngModel.selfs.employees.push(object);
                    break;
                case 3:
                    flowMngModel.evaluators.duration = flow.Duration;
                    flowMngModel.evaluators.hasSendMail = flow.HasSendMail;
                    flowMngModel.evaluators.remindDay = flow.RemindDay;
                    flowMngModel.evaluators.employees.push(flow.EvaluatorInfo);
                    break;
                case 4:
                    flowMngModel.reviewers.duration = flow.Duration;
                    flowMngModel.reviewers.hasSendMail = flow.HasSendMail;
                    flowMngModel.reviewers.remindDay = flow.RemindDay;
                    flowMngModel.reviewers.employees.push(flow.EvaluatorInfo);
                    break;
                case 5:
                    flowMngModel.managements.duration = flow.Duration;
                    flowMngModel.managements.hasSendMail = flow.HasSendMail;
                    flowMngModel.managements.remindDay = flow.RemindDay;
                    flowMngModel.managements.employees.push(flow.EvaluatorInfo);
                    break;
            }
        });
    };

    // #endregion Common

    // #region Flow Template

    flowMngModel.getFlowTemplate = function (targetTypeId, isActive) {
        return flowMngFactory.getFlowTemplate(targetTypeId, isActive)
        .success(function (flowTemplates) {
            flowMngModel.flowTemplates = flowTemplates;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

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
                notificationFactory.error("NameRequired");
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
                notificationFactory.success("CreatedSuccessfully");
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
                notificationFactory.success("UpdatedSuccessfully");
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function () {
            errorFactory.errorHandle(status);
        });
    };

    flowMngModel.openPopupUpdateStatusFlowTemplate = function (flowTemplate) {
        flowTemplate.IsActive = !flowTemplate.IsActive;

        var action = flowTemplate.IsActive ? "activate" : "deactivate",
            newFlowTemplate = {
                flowTemplateId: flowTemplate.FlowTemplateId,
                IsActive: flowTemplate.IsActive
            };

        var modalInstance = $modal.open({
            templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return flowTemplate.Name;
                },
                action: function () {
                    return action;
                }
            },
            windowClass: "modal-confirm-center"
        });

        var keyMessage = '';

        modalInstance.result.then(function () {
            flowMngFactory.updateStatusFlowTemplate(newFlowTemplate)
            .success(function (response) {
                if (response.IsSuccessful) {
                    keyMessage = action === "activate" ? "ActivationSuccess" : "DeactivationSuccess";
                    notificationFactory.success(keyMessage);
                }
                else {
                    flowTemplate.IsActive = !newFlowTemplate.IsActive;
                    notificationFactory.error("", response.Message);
                }
            })
            .error(function (response, status) {
                if (status === 405) {
                    errorFactory.errorHandle(status);
                }
                else {
                    keyMessage = action === "activate" ? "ActivationFail" : "DeactivationFail";
                    flowTemplate.IsActive = !newFlowTemplate.IsActive;
                    notificationFactory.error(keyMessage);
                }
            });
        }, function () {
            flowTemplate.IsActive = !newFlowTemplate.IsActive;
        })['finally'](function () {
            modalInstance = '';
        });
    };

    flowMngModel.openPopupDeleteFlowTemplate = function (vm, flowTemplate) {
        var modalInstance = $modal.open({
            templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return flowTemplate.Name;
                },
                action: function () {
                    return 'delete';
                }
            },
            windowClass: "modal-confirm-center"
        });

        modalInstance.result.then(function () {
            flowMngFactory.deleteFlowTemplate(flowTemplate.FlowTemplateId).success(function (response) {
                if (response.IsSuccessful) {
                    var index = vm.flowTemplates.indexOf(flowTemplate);
                    vm.flowTemplates.splice(index, 1);
                    utilitiesFactory.searchOnPage(vm, vm.flowTemplates);

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

    // #endregion Flow Template

    // #region Department Flow Seting

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

    flowMngModel.validateDepartmentFlow = function (vm) {
        if (!vm.department || !vm.department.DepartmentId) {
            notificationFactory.error("DepartmentNotSelectedWarning");
            return false;
        }
        else if (vm.humanResources.employees.length === 0) {
            notificationFactory.error('', 'Please setting flow for Human Resource.');
            return false;
        }
        else if (vm.evaluators.employees.length === 0 &&
            vm.reviewers.employees.length === 0 &&
            vm.managements.employees.length === 0) {
            notificationFactory.error('', 'Please setting flow for Evaluator or Reviewer or Management.');
            return false;
        }

        return true;
    };

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
                flowMngFactory.deleteDepartmentFlow(vm.department.DepartmentId, vm.FlowTemplateId)
                .success(function (response) {
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

    // #endregion Department Flow Seting

    // #region Personal Flow Seting

    flowMngModel.getPersonalFlow = function (employeeId, flowTemplateId) {
        return flowMngFactory.getPersonalFlow(employeeId, flowTemplateId)
        .success(function (flows) {
            flowMngModel.resetCurrentFlow(flowMngModel);
            successGetFlow(flows);
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    flowMngModel.validatePersonalFlow = function (vm) {
        if (!vm.employeeId) {
            notificationFactory.error("EmployeeNotSelectedWarning");
            return false;
        }
        else if (vm.humanResources.employees.length === 0) {
            notificationFactory.error('', 'Please setting flow for Human Resource.');
            return false;
        }
        else if (vm.evaluators.employees.length === 0 &&
            vm.reviewers.employees.length === 0 &&
            vm.managements.employees.length === 0) {
            notificationFactory.error('', 'Please setting flow for Evaluator or Reviewer or Management.');
            return false;
        }

        return true;
    };

    flowMngModel.createPersonalFlow = function (vm) {
        var model = {
            flowTemplateId: vm.FlowTemplateId,
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
            flowMngFactory.deletePersonalFlow(vm.employeeId, vm.FlowTemplateId)
            .success(function (response) {
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

    flowMngModel.getEmployeeFlowSetting = function (flowTemplateId, departmentId, groupId) {
        return flowMngFactory.getEmployeeFlowSetting(flowTemplateId, departmentId, groupId)
        .success(function (data) {
            flowMngModel.defaultFlowSettings = data.DefaultFlowSettings;
            flowMngModel.personalFlowSettings = data.PersonalFlowSettings;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    // #endregion Personal Flow Seting

    return flowMngModel;
} ]);