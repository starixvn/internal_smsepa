'use strict';
/*global employeeModule*/

employeeModule.factory('EmployeeModel',
['$modal', 'employeeFactory', 'errorFactory', 'notificationFactory',
function ($modal, employeeFactory, errorFactory, notificationFactory) {
    var employeeModel = {};

    employeeModel.employees = [];
    employeeModel.groups = [];
    employeeModel.jobTitles = [];

    employeeModel.getAllEmployee = function (query, groupId, isSpecialUser) {
        return employeeFactory.getAllEmployee(query, groupId, isSpecialUser).success(function (employees) {
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

    employeeModel.openPopupCreateNewEmployee = function (vm) {
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

    employeeModel.getAllJobTitle = function () {
        return employeeFactory.getAllJobTitle().success(function (jobTitles) {
            employeeModel.jobTitles = jobTitles;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeModel.validateEmployee = function (employee) {
        if (!employee) {
            return false;
        }
        else {
            if (!employee.Code) {
                notificationFactory.error("CodeRequried");
                return false;
            }
            else if (!employee.NickName) {
                notificationFactory.error("NickNameRequried");
                return false;
            }
            else if (!employee.FullName) {
                notificationFactory.error("FullNameRequried");
                return false;
            }
            else if (!employee.Username) {
                notificationFactory.error("UsernameRequried");
                return false;
            }
            else if (!employee.Password) {
                notificationFactory.error("PasswordRequried");
                return false;
            }
        }

        return true;
    };

    employeeModel.openPopupUpdateEmployee = function (vm, employee) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/Employee/Templates/edit.html',
            controller: "UpdateEmployeeController as employeeCtrl",
            resolve: {
                employee: function () {
                    return employee;
                }
            },
            backdrop: 'static',
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    employeeModel.updateEmployee = function (updatedEmployee, modalInstance) {
        return employeeFactory.updateSpecialEmployee(updatedEmployee).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdateSuccessfully");
                modalInstance.close(updatedEmployee);
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    var findEmployeeGroupById = function (groupId) {
        var groupResult = {};

        angular.forEach(employeeModel.groups, function (group) {
            if (group.GroupId === groupId) {
                groupResult = group;
            }
        });

        return groupResult;
    };

    var findEmployeeJobTitleById = function (jobTitleId) {
        var jobTitleResult = {};

        angular.forEach(employeeModel.jobTitles, function (jobTitle) {
            if (jobTitle.JobTitleId === jobTitleId) {
                jobTitleResult = jobTitle;
            }
        });

        return jobTitleResult;
    };

    employeeModel.updateChangedEmployee = function (employee, updatedEmployee) {
        employee.Code = updatedEmployee.Code;
        employee.NickName = updatedEmployee.NickName;
        employee.FullName = updatedEmployee.FullName;
        employee.Username = updatedEmployee.Username;
        employee.Password = updatedEmployee.Password;
        employee.Enabled = updatedEmployee.Enabled;
        employee.Group = findEmployeeGroupById(updatedEmployee.GroupId);
        employee.JobTitle = findEmployeeJobTitleById(updatedEmployee.JobTitleId);
    };

    employeeModel.openPopupChangePassword = function (vm) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/Employee/Templates/changePassword.html',
            controller: "ChangePasswordEmployeeController as changePasswordEmployeeCtrl",
            backdrop: 'static',
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    employeeModel.validatePassword = function (password) {
        if (!password) {
            return false;
        }
        else {
            if (!password.oldPassword) {
                notificationFactory.error("OldPasswordRequired");
                return false;
            }
            else if (!password.newPassword) {
                notificationFactory.error("NewPasswordRequired");
                return false;
            }
            else if (!password.confirmPassword) {
                notificationFactory.error("ConfirmPasswordRequired");
                return false;
            }
            else if (password.newPassword !== password.confirmPassword) {
                notificationFactory.error("WrongConfirmPassword");
                return false;
            }
        }

        return true;
    };

    employeeModel.changePassword = function (password, modalInstance) {
        return employeeFactory.changePassword(password).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdateSuccessfully");
                modalInstance.close();
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeModel.openPopupAssignRole = function (employee) {
        var modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/Employee/Templates/assignRole.html',
            controller: "AssignRoleToUserController as assignRoleCtrl",
            backdrop: 'static',
            resolve: {
                employee: function () {
                    return angular.copy(employee);
                }
            },
            windowClass: "modal-confirm-center"
        });

        modalInstance.result.then(function (editedRole) {
            employeeModel.assignRoleToEmployee(employee.EmployeeId, editedRole.roleIds).then(function () {
                employee.Roles = editedRole.roles;
            });
        })['finally'](function () {
            modalInstance = '';
        });
    };

    employeeModel.assignRoleToEmployee = function (employeeId, roleIds) {
        var employeeRoles = {
            employeeId: employeeId,
            roleIds: roleIds
        };

        return employeeFactory.assignRoleToEmployee(employeeRoles).success(function (response) {
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

    employeeModel.createRoles = function (roleIds, systemRoles) {
        var roles = [],
            roleIdList = roleIds.split(",");

        angular.forEach(roleIdList, function (roleId) {
            angular.forEach(systemRoles, function (role) {
                if (parseInt(roleId) === role.RoleId) {
                    roles.push(angular.copy(role));
                }
            });
        });

        return roles;
    };

    return employeeModel;
} ]);