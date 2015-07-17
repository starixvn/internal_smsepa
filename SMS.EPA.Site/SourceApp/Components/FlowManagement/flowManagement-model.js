'use strict';
/*global flowMngModule*/

flowMngModule.factory('FlowMngModel',
['$modal', 'flowMngFactory', 'notificationFactory', 'errorFactory',
function ($modal, flowMngFactory, notificationFactory, errorFactory) {
    var flowMngModel = {};

    flowMngModel.selfs = [{ EmployeeId: -1, NickName: "Self" }];
    flowMngModel.evaluators = [];
    flowMngModel.reviewers = [];
    flowMngModel.managements = [];
    flowMngModel.humanResources = [];

    /// Get flow.
    flowMngModel.getFlow = function (departmentId, flowType) {
        return flowMngFactory.getFlow(departmentId, flowType)
        .success(function (flows) {
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
                        flowMngModel.selfs.push(object);
                        break;
                    case 3:
                        flowMngModel.evaluators.push(flow.EvaluatorInfo);
                        break;
                    case 4:
                        flowMngModel.reviewers.push(flow.EvaluatorInfo);
                        break;
                    case 5:
                        flowMngModel.managements.push(flow.EvaluatorInfo);
                        break;
                    case 6:
                        flowMngModel.humanResources.push(flow.EvaluatorInfo);
                        break;
                }
            });
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return flowMngModel;
} ]);