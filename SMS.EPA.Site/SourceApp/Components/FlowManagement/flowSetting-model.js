(function (flowMngModule) {
    'use strict';

    flowMngModule.factory('FlowSettingModel', flowSettingModel);

    flowSettingModel.$inject = ['$modal', 'notificationFactory', 'baseUrl'];

    function flowSettingModel($modal, notificationFactory, baseUrl) {
        var flowModel = {};

        flowModel.selfs = {
            duration: 0,
            hasSendMail: true,
            remindDay: 0,
            employees: []
        };
        flowModel.evaluators = {
            duration: 0,
            hasSendMail: true,
            remindDay: 0,
            employees: []
        };
        flowModel.reviewers = {
            duration: 0,
            hasSendMail: true,
            remindDay: 0,
            employees: []
        };
        flowModel.managements = {
            duration: 0,
            hasSendMail: true,
            remindDay: 0,
            employees: []
        };
        flowModel.humanResources = {
            duration: 0,
            hasSendMail: true,
            remindDay: 0,
            employees: []
        };

        flowModel.resetCurrentFlow = function (vm) {
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

        flowModel.removeItemFromList = function (item, list) {
            var index = list.indexOf(item);
            if (index !== -1) {
                list.splice(index, 1);
            }
        };

        flowModel.addEmployee = function (listIdentity, vm, employee, position) {
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
                } else {
                    list.push(employee);
                }
            } else {
                notificationFactory.error('', 'This employee has already existed!');
            }
        };

        flowModel.successGetFlow = function (flows, model) {
            angular.forEach(flows, function (flow) {
                flow.EvaluatorInfo.Avatar = baseUrl + flow.EvaluatorInfo.Avatar;

                switch (flow.ProcessId) {
                    case 1:
                        model.humanResources.duration = flow.Duration;
                        model.humanResources.hasSendMail = flow.HasSendMail;
                        model.humanResources.remindDay = flow.RemindDay;
                        model.humanResources.employees.push(flow.EvaluatorInfo);
                        break;
                    case 2:
                        var object;
                        model.selfs.duration = flow.Duration;
                        model.selfs.hasSendMail = flow.HasSendMail;
                        model.selfs.remindDay = flow.RemindDay;
                        if (flow.EvaluatorInfo.EmployeeId === -1) {
                            object = { NickName: "Self", EmployeeId: -1 };
                        } else {
                            object = flow.EvaluatorInfo;
                        }
                        model.selfs.employees.push(object);
                        break;
                    case 3:
                        model.evaluators.duration = flow.Duration;
                        model.evaluators.hasSendMail = flow.HasSendMail;
                        model.evaluators.remindDay = flow.RemindDay;
                        model.evaluators.employees.push(flow.EvaluatorInfo);
                        break;
                    case 4:
                        model.reviewers.duration = flow.Duration;
                        model.reviewers.hasSendMail = flow.HasSendMail;
                        model.reviewers.remindDay = flow.RemindDay;
                        model.reviewers.employees.push(flow.EvaluatorInfo);
                        break;
                    case 5:
                        model.managements.duration = flow.Duration;
                        model.managements.hasSendMail = flow.HasSendMail;
                        model.managements.remindDay = flow.RemindDay;
                        model.managements.employees.push(flow.EvaluatorInfo);
                        break;
                }
            });
        }

        return flowModel;
    }
})(angular.module('flowMngModule'));