'use strict';
/*global employeeModule*/

employeeModule.factory('EmployeeModel',
['$modal', 'employeeFactory', 'errorFactory',  'notificationFactory',
function ($modal, employeeFactory, errorFactory, notificationFactory) {
    var employeeModel = {};

    employeeModel.employees = [];
    employeeModel.groups = [];

    employeeModel.getAllEmployee = function (query, groupId) {
        return employeeFactory.getAllEmployee(query, groupId).success(function (employees) {
            employeeModel.employees = employees;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeModel.getAllGroup = function () {
        return employeeFactory.getAllGroup().success(function (groups) {
            employeeModel.groups = groups;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeModel.openPopupAddNewEmployee = function (vm) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/Employee/Templates/create.html',
            controller: "CreateEmployeeController as employeeCtrl",
            backdrop: 'static',
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    employeeModel.createEmployee = function (employee, modalInstance) {
        return employeeFactory.insertSpecialEmployee(employee).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("CreateSuccessfully");
                modalInstance.close();
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return employeeModel;
} ]);