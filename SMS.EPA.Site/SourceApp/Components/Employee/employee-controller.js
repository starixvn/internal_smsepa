'use strict';
/*global employeeModule*/

employeeModule.controller('EmployeeController',
['$window', 'EmployeeModel', 'utilitiesFactory', 'CommonModel',
function ($window, EmployeeModel, utilitiesFactory, CommonModel) {
    var vm = this;

    vm.employees = EmployeeModel.employees;
    vm.filter = {
        query: '',
        groupId: '',
        isSpecialUser: ''
    };

    vm.groupOptions = [];
    vm.currentPage = 1;
    vm.pageSize = 20;

    utilitiesFactory.numberEntryList(vm);

    EmployeeModel.getAllGroup().then(function () {
        vm.groupOptions = EmployeeModel.groups;
    });

    var getEmployee = function (query, groupId, isSpecialUser) {
        EmployeeModel.getAllEmployee(query, groupId, isSpecialUser).then(function () {
            vm.employees = EmployeeModel.employees;
            utilitiesFactory.searchOnPage(vm, vm.employees);
        });
    };

    getEmployee(vm.filter.query, vm.filter.groupId, vm.filter.isSpecialUser);

    vm.queryEmployee = function (filter) {
        getEmployee(filter.query, filter.groupId, filter.isSpecialUser);
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
            getEmployee(vm.filter.query, vm.filter.groupId, vm.filter.isSpecialUser);
        });
    };

    vm.updateEmployee = function (employee) {
        EmployeeModel.openPopupUpdateEmployee(vm, employee).then(function (updatedEmployee) {
            EmployeeModel.updateChangedEmployee(employee, updatedEmployee);
        })['finally'](function () {
            vm.modalInstance = '';
        });
    };

    vm.assignRole = function (employee) {
        EmployeeModel.openPopupAssignRole(employee);
    };
} ]);

employeeModule.controller('CreateEmployeeController',
['$modalInstance', 'EmployeeModel',
function ($modalInstance, EmployeeModel) {
    var vm = this;

    vm.groupOptions = EmployeeModel.groups;
    vm.jobTitleOptions = EmployeeModel.jobTitles;

    vm.employee = {
        Code: '',
        NickName: '',
        FullName: '',
        GroupId: '',
        JobTitleId: '',
        Username: '',
        Password: '',
        Enabled: true
    };

    EmployeeModel.getAllGroup().then(function () {
        vm.groupOptions = EmployeeModel.groups;
    });

    EmployeeModel.getAllJobTitle().then(function () {
        vm.jobTitleOptions = EmployeeModel.jobTitles;
    });

    vm.save = function (employee) {
        if (EmployeeModel.validateEmployee(employee)) {
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
        Code: employee.Code,
        NickName: employee.NickName,
        FullName: employee.FullName,
        GroupId: employee.Group ? employee.Group.GroupId : '',
        JobTitleId: employee.JobTitle ? employee.JobTitle.JobTitleId : '',
        Username: employee.Username,
        Password: employee.Password,
        Enabled: employee.Enabled
    };

    EmployeeModel.getAllGroup().then(function () {
        vm.groupOptions = EmployeeModel.groups;
    });

    EmployeeModel.getAllJobTitle().then(function () {
        vm.jobTitleOptions = EmployeeModel.jobTitles;
    });

    vm.save = function (updatedEmployee) {
        if (EmployeeModel.validateEmployee(updatedEmployee) && updatedEmployee.Password === vm.originalEmployee.Password) {
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
} ]);