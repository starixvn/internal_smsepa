'use strict';
/*global departmentModule*/

departmentModule.factory('DepartmentModel',
['$modal', 'departmentFactory', 'notificationFactory', 'errorFactory',
function ($modal, departmentFactory, notificationFactory, errorFactory) {
    var deptModel = {};

    deptModel.validateDepartment = function (department) {
        if (!department) {
            return false;
        }
        else {
            if (!department.Code) {
                notificationFactory.error("CodeRequried");
                return false;
            }
            else if (!department.Name) {
                notificationFactory.error("NameRequire");
                return false;
            }
            else if(!department.ParentId){
                notificationFactory.error("ParentNodeRequired");
                return false;
            }
            else if (!department.Manager || !department.Manager.EmployeeId) {
                notificationFactory.error("ManagerRequired");
                return false;
            }
        }

        return true;
    };

    deptModel.updateDepartment = function (department) {
        var criteria = {
            DepartmentId: department.DepartmentId,
            Code: department.Code,
            Name: department.Name,
            ManagerId: department.Manager.EmployeeId,
            ParentId: department.ParentId,
            IsActive: department.IsActive
        };

        return departmentFactory.updateDepartment(criteria)
        .success(function (response) {
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

    return deptModel;
}]);