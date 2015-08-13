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
                notificationFactory.error("NameRequired");
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

    deptModel.createDepartment = function (vm, department) {
        var criteria = {
            Code: department.Code,
            Name: department.Name,
            ManagerId: department.Manager.EmployeeId,
            ParentId: department.ParentId,
            IsActive: department.IsActive
        };

        return departmentFactory.createDepartment(criteria)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("CreatedSuccessfully");

                vm.initialize();
            }
            else {
                notificationFactory.error("", response.Message);
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    deptModel.updateDepartment = function (vm, department) {
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
                notificationFactory.success("UpdatedSuccessfully");

                vm.initialize();
            }
            else {
                notificationFactory.error("", response.Message);
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    deptModel.openPopupDeleteDepartment = function (vm, department) {
        var modalInstance = $modal.open({
            templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return department.Name;
                },
                action: function () {
                    return 'delete';
                }
            },
            windowClass: "modal-confirm-center"
        });

        modalInstance.result.then(function () {
            departmentFactory.deleteDepartment(department.DepartmentId).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("DeletedSuccessfully");

                    vm.initialize();
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

    return deptModel;
}]);