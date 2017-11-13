(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('employeeModule').factory('EmployeeModel', employeeModel);
    employeeModel.$inject = ['$uibModal', '$window', '$timeout', 'employeeFactory', 'errorFactory', 'notificationFactory'];

    function employeeModel($uibModal, $window, $timeout, employeeFactory, errorFactory, notificationFactory) {
        var model = {},
            emailPattern = /^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;

        model.employees = [];
        model.defaultPassword = null;
        model.permission = 0;

        model.getAllEmployee = function (query, departmentId, isLocal, isSpecialUser, isDisabledUser, hasMonthlyEPA) {
            return employeeFactory.getAllEmployee(query, departmentId, isLocal, isSpecialUser, isDisabledUser, hasMonthlyEPA)
            .success(function (employees) {
                model.employees = employees;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.openPopupCreateNewEmployee = function (vm) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/Employee/Templates/create.html',
                controller: "CreateEmployeeController as employeeCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center"
            });

            return vm.modalInstance.result;
        };

        model.createEmployee = function (employee, modalInstance) {
            return employeeFactory.createSpecialEmployee(employee).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("CreatedSuccessfully");
                    modalInstance.close();
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.validateEmployee = function (employee, mode) {
            if (!employee) {
                return false;
            }
            else {
                if (!employee.NickName) {
                    notificationFactory.error("NickNameRequried");
                    return false;
                } else if (!employee.FullName) {
                    notificationFactory.error("FullNameRequried");
                    return false;
                } else if (!employee.Username) {
                    notificationFactory.error("UsernameRequried");
                    return false;
                } else if (employee.Username.indexOf(' ') !== -1) {
                    notificationFactory.error("UsernameNotSpaceWarning");
                    return false;
                } else if (mode === 'create' && !employee.Password) {
                    notificationFactory.error("PasswordRequried");
                    return false;
                } else if (mode === 'create' && (employee.Password.length < 6 || employee.Password.length > 32)) {
                    notificationFactory.error("PasswordLengthWarning");
                    return false;
                } else if (!employee.GroupId) {
                    notificationFactory.error("SectionRequired");
                    return false;
                } else if (employee.Email && !emailPattern.test(employee.Email)) {
                    notificationFactory.error("EmailWrongPatternWarning");
                    return false;
                }
            }

            return true;
        };

        model.openPopupUpdateEmployee = function (vm, employee) {
            vm.modalInstance = $uibModal.open({
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

        model.updateEmployee = function (updatedEmployee, modalInstance) {
            return employeeFactory.updateSpecialEmployee(updatedEmployee).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                    modalInstance.close(updatedEmployee);
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        function findEmployeeGroupById(groupId) {
            var groupResult = {};

            angular.forEach(model.groups, function (group) {
                if (group.GroupId === groupId) {
                    groupResult = group;
                }
            });

            return groupResult;
        }

        function findEmployeeJobTitleById(jobTitleId) {
            var jobTitleResult = {};

            angular.forEach(model.jobTitles, function (jobTitle) {
                if (jobTitle.JobTitleId === jobTitleId) {
                    jobTitleResult = jobTitle;
                }
            });

            return jobTitleResult;
        }

        model.updateChangedEmployee = function (employee, updatedEmployee) {
            employee.Code = updatedEmployee.Code;
            employee.NickName = updatedEmployee.NickName;
            employee.FullName = updatedEmployee.FullName;
            employee.Username = updatedEmployee.Username;
            employee.Enabled = updatedEmployee.Enabled;
            employee.Group = findEmployeeGroupById(updatedEmployee.GroupId);
            employee.JobTitle = findEmployeeJobTitleById(updatedEmployee.JobTitleId);
        };

        model.openPopupChangePassword = function (vm) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/Employee/Templates/changePassword.html',
                controller: "ChangePasswordEmployeeController as changePasswordEmployeeCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center"
            });

            return vm.modalInstance.result;
        };

        model.validatePassword = function (password) {
            if (!password) {
                return false;
            }
            else {
                if (!password.oldPassword) {
                    notificationFactory.error("OldPasswordRequired");
                    return false;
                } else if (!password.newPassword) {
                    notificationFactory.error("NewPasswordRequired");
                    return false;
                } else if (!password.confirmPassword) {
                    notificationFactory.error("ConfirmPasswordRequired");
                    return false;
                } else if (password.newPassword !== password.confirmPassword) {
                    notificationFactory.error("WrongConfirmPassword");
                    return false;
                }
            }

            return true;
        };

        model.changePassword = function (password, modalInstance) {
            return employeeFactory.changePassword(password).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                    modalInstance.close();

                    $timeout(function () {
                        $window.location.href = "Auth/Logout";
                    }, 1000);
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.openPopupAssignRole = function (employee) {
            var modalInstance = $uibModal.open({
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
                model.assignRoleToEmployee(employee.EmployeeId, editedRole.roleIds).then(function () {
                    employee.Roles = editedRole.roles;
                });
            })['finally'](function () {
                modalInstance = '';
            });
        };

        model.assignRoleToEmployee = function (employeeId, roleIds) {
            var employeeRoles = {
                employeeId: employeeId,
                roleIds: roleIds
            };

            return employeeFactory.assignRoleToEmployee(employeeRoles).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.createRoles = function (roleIds, systemRoles) {
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

        model.getDefaultPassword = function () {
            return employeeFactory.getDefaultPassword().success(function (password) {
                model.defaultPassword = password;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.openPopupResetPassword = function (employee, defaultPassword) {
            var modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/Employee/Templates/resetPassword.html',
                controller: "ResetPasswordController as resetPasswordCtrl",
                backdrop: 'static',
                resolve: {
                    employee: function () {
                        return angular.copy(employee);
                    },
                    defaultPassword: function () {
                        return defaultPassword;
                    }
                },
                windowClass: "modal-confirm-center"
            });

            modalInstance.result.then(function () {
                employeeFactory.resetPassword(employee.EmployeeId).success(function (response) {
                    if (response.IsSuccessful) {
                        notificationFactory.success("UpdatedSuccessfully");
                    } else {
                        notificationFactory.error('', response.Message);
                    }
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
            })['finally'](function () {
                modalInstance = '';
            });
        };

        model.openPopupUpdateHasMonthlyEPA = function (vm, employee) {
            employee.HasMonthlyEPA = !employee.HasMonthlyEPA;

            var action = employee.HasMonthlyEPA ? "set" : "unset",
            newEmployee = {
                EmployeeId: employee.EmployeeId,
                HasMonthlyEPA: employee.HasMonthlyEPA
            };

            var modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/Employee/Templates/modalConfirmSetUnsetMonthlyEvaluation.html',
                controller: "ModalConfirmSetUnsetMonthlyEvaluationController as modalConfirmCtrl",
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return employee.NickName;
                    },
                    action: function () {
                        return action;
                    }
                },
                windowClass: "modal-confirm-center"
            });

            modalInstance.result.then(function () {
                employeeFactory.updateHasMonthlyEPA(newEmployee).success(function (response) {
                    if (response.IsSuccessful) {
                        notificationFactory.success("UpdatedSuccessfully");
                    } else {
                        employee.HasMonthlyEPA = !newEmployee.HasMonthlyEPA;
                        notificationFactory.error('', response.Message);
                    }
                }).error(function (response, status) {
                    if (status === 405) {
                        errorFactory.errorHandle(status);
                    } else {
                        employee.HasMonthlyEPA = !newEmployee.HasMonthlyEPA;
                        notificationFactory.error("UpdatedUnsuccessfully");
                    }
                });
            }, function () {
                employee.HasMonthlyEPA = !newEmployee.HasMonthlyEPA;
            })['finally'](function () {
                modalInstance = '';
            });
        };

        model.getUserPermissionOnEmployee = function () {
            return employeeFactory.getUserPermissionOnEmployee().success(function (permission) {
                model.permission = parseInt(permission);
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        return model;
    }
})(angular);