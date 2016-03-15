'use strict';
/*global employeeModule*/

employeeModule.controller('EmployeeController',
['$window', 'EmployeeModel', 'utilitiesFactory', 'CommonModel', 'enumerationsFactory',
function ($window, EmployeeModel, utilitiesFactory, CommonModel, enumerationsFactory) {
    var vm = this;

    vm.employees = EmployeeModel.employees;
    vm.defaultPassword = EmployeeModel.defaultPassword;
    vm.permission = EmployeeModel.permission;
    vm.policyEnum = enumerationsFactory.policy;

    vm.filter = {
        query: '',
        groupId: '',
        isLocal: '',
        isSpecialUser: '',
        isDisabledUser: '',
        hasMonthlyEPA: ''
    };

    vm.groupOptions = [];
    vm.currentPage = 1;
    vm.pageSize = 20;

    utilitiesFactory.numberEntryList(vm);

    EmployeeModel.getAllGroup().then(function () {
        vm.groupOptions = EmployeeModel.groups;
    });

    EmployeeModel.getDefaultPassword().then(function () {
        vm.defaultPassword = EmployeeModel.defaultPassword;
    });

    EmployeeModel.getUserPermissionOnEmployee().then(function () {
        vm.permission = EmployeeModel.permission;
    });

    var getEmployee = function (query, groupId, isLocal, isSpecialUser, isDisabledUser, hasMonthlyEPA) {
        if (!hasMonthlyEPA) {
            hasMonthlyEPA = '';
        }

        EmployeeModel.getAllEmployee(query, groupId, isLocal, isSpecialUser, isDisabledUser, hasMonthlyEPA).then(function () {
            vm.employees = EmployeeModel.employees;
            utilitiesFactory.searchOnPage(vm, vm.employees);
        });
    };

    getEmployee(vm.filter.query, vm.filter.groupId, vm.filter.isLocal, vm.filter.isSpecialUser, vm.filter.isDisabledUser, vm.filter.hasMonthlyEPA);

    vm.queryEmployee = function (filter) {
        vm.searchKey = '';
        vm.currentPage = 1;
        getEmployee(filter.query, filter.groupId, filter.isLocal, filter.isSpecialUser, filter.isDisabledUser, filter.hasMonthlyEPA);
    };

    /*
    ** Get index of row in table.
    */
    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };

    vm.createEmployee = function () {
        EmployeeModel.openPopupCreateNewEmployee(vm).then()['finally'](function () {
            vm.modalInstance = '';
            getEmployee(vm.filter.query, vm.filter.groupId, vm.filter.isLocal, vm.filter.isSpecialUser);
        });
    };

    vm.updateEmployee = function (employee) {
        EmployeeModel.openPopupUpdateEmployee(vm, employee).then(function (updatedEmployee) {
            EmployeeModel.updateChangedEmployee(employee, updatedEmployee);
        })['finally'](function () {
            vm.modalInstance = '';
            getEmployee(vm.filter.query, vm.filter.groupId, vm.filter.isLocal, vm.filter.isSpecialUser, vm.filter.isDisabledUser, vm.filter.hasMonthlyEPA);
        });
    };

    vm.assignRole = function (employee) {
        EmployeeModel.openPopupAssignRole(employee);
    };

    vm.resetPassword = function (employee) {
        EmployeeModel.openPopupResetPassword(employee, vm.defaultPassword);
    };

    vm.updateHasMonthlyEPA = function (employee) {
        EmployeeModel.openPopupUpdateHasMonthlyEPA(vm, employee);
    };
} ]);

employeeModule.controller('CreateEmployeeController',
['$modalInstance', 'EmployeeModel',
function ($modalInstance, EmployeeModel) {
    var vm = this;

    vm.groupOptions = EmployeeModel.groups;
    vm.jobTitleOptions = EmployeeModel.jobTitles;

    vm.employee = {
        NickName: '',
        FullName: '',
        GroupId: '',
        JobTitleId: '',
        Username: '',
        Password: '',
        Enabled: true,
        IsLocal: 'true',
        Email: ''
    };

    EmployeeModel.getAllGroup().then(function () {
        vm.groupOptions = EmployeeModel.groups;
    });

    EmployeeModel.getAllJobTitle().then(function () {
        vm.jobTitleOptions = EmployeeModel.jobTitles;
    });

    vm.save = function (employee) {
        if (EmployeeModel.validateEmployee(employee, 'create')) {
            EmployeeModel.createEmployee(employee, $modalInstance);
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
} ]);

employeeModule.controller('UpdateEmployeeController',
['$modalInstance', 'EmployeeModel', 'employee',
function ($modalInstance, EmployeeModel, employee) {
    var vm = this;

    vm.groupOptions = EmployeeModel.groups;
    vm.jobTitleOptions = EmployeeModel.jobTitles;
    vm.originalEmployee = employee;

    vm.employee = {
        EmployeeId: employee.EmployeeId,
        NickName: employee.NickName,
        FullName: employee.FullName,
        GroupId: employee.Group ? employee.Group.GroupId : '',
        JobTitleId: employee.JobTitle ? employee.JobTitle.JobTitleId : '',
        Username: employee.Username,
        Enabled: employee.Enabled,
        IsLocal: employee.IsLocal ? 'true' : 'false',
        Email: employee.Email
    };

    EmployeeModel.getAllGroup().then(function () {
        vm.groupOptions = EmployeeModel.groups;
    });

    EmployeeModel.getAllJobTitle().then(function () {
        vm.jobTitleOptions = EmployeeModel.jobTitles;
    });

    vm.save = function (updatedEmployee) {
        if (EmployeeModel.validateEmployee(updatedEmployee, 'update')) {
            EmployeeModel.updateEmployee(updatedEmployee, $modalInstance);
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
} ]);

employeeModule.controller('EmployeeProfileController',
['$window', 'EmployeeModel', function ($window, EmployeeModel) {
    var vm = this;

    vm.logout = function () {
        $window.location.href = "Auth/Logout";
    };

    vm.changePassword = function () {
        EmployeeModel.openPopupChangePassword(vm).then()['finally'](function () {
            vm.modalInstance = '';
        });
    };
} ]);

employeeModule.controller('ChangePasswordEmployeeController',
['$modalInstance', 'EmployeeModel',
function ($modalInstance, EmployeeModel) {
    var vm = this;

    vm.password = {
        oldPassword: '',
        newPassword: '',
        confirmPassword: ''
    };

    vm.save = function (password) {
        if (EmployeeModel.validatePassword(password)) {
            EmployeeModel.changePassword(password, $modalInstance);
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
} ]);

employeeModule.controller('AssignRoleToUserController',
['$modalInstance', 'EmployeeModel', 'employee', 'CommonModel',
function ($modalInstance, EmployeeModel, employee, CommonModel) {
    var vm = this;

    vm.employee = employee;
    vm.systemRoles = [];

    CommonModel.getSystemRoles(true).then(function () {
        vm.systemRoles = CommonModel.systemRoles;
    });

    vm.checkRolePermission = function (roleId) {
        var isChecked = false;
        angular.forEach(vm.employee.Roles, function (role) {
            if (role.RoleId === roleId) {
                isChecked = true;
            }
        });

        return isChecked;
    };

    vm.save = function (roleIds) {
        var roles = [];

        if (roleIds) {
            roles = EmployeeModel.createRoles(roleIds, vm.systemRoles);
        }

        var editedRole = {
            roles: roles,
            roleIds: roleIds
        };
        $modalInstance.close(editedRole);
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
}]);

employeeModule.controller("ResetPasswordController",
 ['$modalInstance', 'employee', 'defaultPassword',
 function ($modalInstance, employee, defaultPassword) {
     var vm = this;

     vm.employee = employee;
     vm.defaultPassword = defaultPassword;

     vm.save = function () {
         $modalInstance.close();
     };

     vm.cancel = function () {
         $modalInstance.dismiss("cancel");
     };
 }]);

employeeModule.controller("ModalConfirmSetUnsetMonthlyEvaluationController",
['$modalInstance', 'name', 'action',
function ($modalInstance, name, action) {
     var vm = this;

     vm.name = name;
     vm.action = action;

     vm.ok = function () {
         $modalInstance.close();
     };

     vm.cancel = function () {
         $modalInstance.dismiss("cancel");
     };
 }]);