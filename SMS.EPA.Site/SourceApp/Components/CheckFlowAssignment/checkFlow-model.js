'use strict';
/*global checkFlowAssignmentModule*/

checkFlowAssignmentModule.factory('CheckFlowModel',
['checkFlowFactory', 'errorFactory', 'notificationFactory',
function (checkFlowFactory, errorFactory, notificationFactory) {
    var checkFlowModel = {};
    checkFlowModel.employeeSettings = [];

    checkFlowModel.evaluatorType = {
        Evaluator: 1,
        Reviewer: 2,
        Management: 3
    };

    checkFlowModel.getEmployeeFlowTargetSetting = function (groupId, employeeId, targetTypeId) {
        return checkFlowFactory.getEmployeeFlowTargetSetting(groupId, employeeId, targetTypeId).success(function (employeeSettings) {
            checkFlowModel.employeeSettings = employeeSettings;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    checkFlowModel.updateEmployeeFlowTargetSetting = function (employeeAssignments) {
        return checkFlowModel.updateEmployeeFlowTargetSetting(employeeAssignments).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdateSuccessfully");
            }
            else {
                notificationFactory.error("", response.Message);
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    checkFlowModel.resetCalendar = function (vm) {
        vm.calendar.evaluator.startDateOpened = false;
        vm.calendar.evaluator.endDateOpened = false;

        vm.calendar.reviewer.startDateOpened = false;
        vm.calendar.reviewer.endDateOpened = false;

        vm.calendar.management.startDateOpened = false;
        vm.calendar.management.endDateOpened = false;
    };

    checkFlowModel.openPopupStartDate = function (vm, event, evaluatorType) {
        event.preventDefault();
        event.stopPropagation();
        checkFlowModel.resetCalendar(vm);

        switch (evaluatorType) {
            case checkFlowModel.evaluatorType.Evaluator:
                vm.calendar.evaluator.startDateOpened = true;
                break;
            case checkFlowModel.evaluatorType.Reviewer:
                vm.calendar.reviewer.startDateOpened = true;
                break;
            case checkFlowModel.evaluatorType.Management:
                vm.calendar.management.startDateOpened = true;
                break;
        }
    };

    checkFlowModel.openPopupEndDate = function (vm, event, evaluatorType) {
        event.preventDefault();
        event.stopPropagation();
        checkFlowModel.resetCalendar(vm);

        switch (evaluatorType) {
            case checkFlowModel.evaluatorType.Evaluator:
                vm.calendar.evaluator.endDateOpened = true;
                break;
            case checkFlowModel.evaluatorType.Reviewer:
                vm.calendar.reviewer.endDateOpened = true;
                break;
            case checkFlowModel.evaluatorType.Management:
                vm.calendar.management.endDateOpened = true;
                break;
        }
    };

    return checkFlowModel;
}]);