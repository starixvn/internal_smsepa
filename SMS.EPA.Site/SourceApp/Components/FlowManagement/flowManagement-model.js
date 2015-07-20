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
        ignoreSelfEvaluation: false,
        employees: [{ EmployeeId: -1, NickName: "Self"}]
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

    /// Get flow.
    flowMngModel.getFlow = function (departmentId, flowType) {
        return flowMngFactory.getFlow(departmentId, flowType)
        .success(function (flows) {
            flowMngModel.selfs.employees = [];
            flowMngModel.evaluators.employees = [];
            flowMngModel.reviewers.employees = [];
            flowMngModel.managements.employees = [];
            flowMngModel.humanResources.employees = [];

            angular.forEach(flows, function (flow) {
                var processId = flow.ProcessId;
                switch (processId) {
                    case 2:
                        var object;
                        if (flow.EvaluatorInfo.EmployeeId === -1) {
                            object = { EmployeeId: -1, NickName: "Self" };
                        }
                        else {
                            object = flow.EvaluatorInfo;
                        }
                        flowMngModel.selfs.startDate = flow.StartDate;
                        flowMngModel.selfs.endDate = flow.EndDate;
                        flowMngModel.selfs.hasSendMail = flow.HasSendMail;
                        flowMngModel.selfs.employees.push(object);
                        break;
                    case 3:
                        flowMngModel.evaluators.startDate = flow.StartDate;
                        flowMngModel.evaluators.endDate = flow.EndDate;
                        flowMngModel.evaluators.hasSendMail = flow.HasSendMail;
                        flowMngModel.evaluators.employees.push(flow.EvaluatorInfo);
                        break;
                    case 4:
                        flowMngModel.reviewers.startDate = flow.StartDate;
                        flowMngModel.reviewers.endDate = flow.EndDate;
                        flowMngModel.reviewers.hasSendMail = flow.HasSendMail;
                        flowMngModel.reviewers.employees.push(flow.EvaluatorInfo);
                        break;
                    case 5:
                        flowMngModel.managements.startDate = flow.StartDate;
                        flowMngModel.managements.endDate = flow.EndDate;
                        flowMngModel.managements.hasSendMail = flow.HasSendMail;
                        flowMngModel.managements.employees.push(flow.EvaluatorInfo);
                        break;
                    case 6:
                        flowMngModel.humanResources.startDate = flow.StartDate;
                        flowMngModel.humanResources.endDate = flow.EndDate;
                        flowMngModel.humanResources.hasSendMail = flow.HasSendMail;
                        flowMngModel.humanResources.employees.push(flow.EvaluatorInfo);
                        break;
                }
            });
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

    flowMngModel.createFlow = function (vm) {
        if (!vm.flowType) {
            notificationFactory.error('', 'Please select a department!');
        }
        else {
            var model = {
                departmentId: vm.departmentId,
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

    return flowMngModel;
} ]);