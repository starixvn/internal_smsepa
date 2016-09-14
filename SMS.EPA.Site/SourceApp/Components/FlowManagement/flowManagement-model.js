'use strict';
/*global flowMngModule*/

flowMngModule.factory('FlowMngModel',
['$modal', 'flowMngFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory', 'baseUrl',
function ($modal, flowMngFactory, utilitiesFactory, notificationFactory, errorFactory, baseUrl) {
    var flowMngModel = {};

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
    flowMngModel.departmentSettings = [];
    flowMngModel.flowSettings = [];

    flowMngModel.permission = 0;

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
        employee.Avatar = baseUrl + employee.Avatar;

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
            notificationFactory.error('', 'This employee has already existed!');
        }
    };

    var successGetFlow = function (flows) {
        angular.forEach(flows, function (flow) {
            flow.EvaluatorInfo.Avatar = baseUrl + flow.EvaluatorInfo.Avatar;

            switch (flow.ProcessId) {
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

    // #region Department Flow Seting

    flowMngModel.getDepartmentFlow = function (flowTemplateId) {
        return flowMngFactory.getDepartmentFlow(flowTemplateId)
        .success(function (flows) {
            flowMngModel.flowSettings = flows;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    flowMngModel.getDepartmentFlowById = function (department, flowTemplateId) {
        return flowMngFactory.getDepartmentFlowById(department.DepartmentId, flowTemplateId)
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
        else if (vm.humanResources.employees.length === 0
            || vm.evaluators.employees.length === 0
            || vm.reviewers.employees.length === 0) {
            notificationFactory.error('SettingFlowWarning');
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
                        return "delete this department's setting";
                    }
                },
                windowClass: "modal-confirm-center"
            });

            return modalInstance.result.then(function () {
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

    flowMngModel.getPersonalFlow = function (flowTemplateId) {
        return flowMngFactory.getPersonalFlow(flowTemplateId)
        .success(function (flows) {
            flowMngModel.flowSettings = flows;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    flowMngModel.getPersonalFlowById = function (employeeId, flowTemplateId) {
        return flowMngFactory.getPersonalFlowById(employeeId, flowTemplateId)
        .success(function (flows) {
            flowMngModel.resetCurrentFlow(flowMngModel);
            successGetFlow(flows);
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    flowMngModel.validatePersonalFlow = function (vm) {
        if (!vm.employeeSelected) {
            notificationFactory.error("EmployeeNotSelectedWarning");
            return false;
        }
        else if (vm.flowTemplate.TargetType.TargetTypeId === vm.targetTypeEnum.Monthly && !vm.employeeSelected.HasMonthlyEPA) {
            notificationFactory.error("EmployeeNotSetMonthlyEvaluation");
            return false;
        }
        else if (vm.humanResources.employees.length === 0
            || vm.evaluators.employees.length === 0
            || vm.reviewers.employees.length === 0) {
            notificationFactory.error('SettingFlowWarning');
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
                    return "delete this personal's setting";
                }
            },
            windowClass: "modal-confirm-center"
        });

        modalInstance.result.then(function () {
            flowMngFactory.deletePersonalFlow(vm.employeeId, vm.FlowTemplateId)
            .success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("DeletedSuccessfully");

                    index = vm.personalFlowSettings.map(function (e) {
                        return e.EmployeeId;
                    }).indexOf(vm.employeeId);

                    if (index !== -1) {
                        vm.defaultFlowSettings.push(vm.personalFlowSettings[index]);
                        vm.personalFlowSettings.splice(index, 1);
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

    flowMngModel.getUserPermissionOnFlowManagement = function () {
        return flowMngFactory.getUserPermissionOnFlowManagement()
        .success(function (permission) {
            flowMngModel.permission = parseInt(permission);
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    flowMngModel.getDepartmentHasSetting = function (flowTemplateId) {
        return flowMngFactory.getDepartmentHasSetting(flowTemplateId).success(function (departments) {
            flowMngModel.departmentSettings = departments;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return flowMngModel;
}]);