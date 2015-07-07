'use strict';
/*global employeeModule*/

employeeModule.controller('EmployeeController',
['EmployeeModel', 'utilitiesFactory', 'CommonModel',
function (EmployeeModel, utilitiesFactory, CommonModel) {
    var vm = this;

    vm.employees = EmployeeModel.employees;
    vm.filter = {
        query: '',
        groupId: ''
    };

    vm.groupOptions = [];
    vm.currentPage = 1;
    vm.pageSize = 20;

    utilitiesFactory.numberEntryList(vm);

    EmployeeModel.getAllGroup().then(function () {
        vm.groupOptions = EmployeeModel.groups;
    });

    var getEmployee = function (query, groupId) {
        EmployeeModel.getAllEmployee(query, groupId).then(function () {
            vm.employees = EmployeeModel.employees;
            utilitiesFactory.searchOnPage(vm, vm.employees);
        });
    };

    getEmployee(vm.filter.query, vm.filter.groupId);

    vm.queryEmployee = function (filter) {
        getEmployee(filter.query, filter.groupId);
    };

    /*
    ** Get index of row in table.
    */
    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };

    vm.addEmployee = function () {
        EmployeeModel.openPopupAddNewEmployee(vm).then()['finally'](function () {
            vm.modalInstance = '';
            getEmployee(vm.filter.query, vm.filter.groupId);
        });
    };
} ]);

employeeModule.controller('CreateEmployeeController',
['$modalInstance', 'EmployeeModel',
function ($modalInstance, EmployeeModel) {
    var vm = this;

    vm.employee = {
    };

    vm.save = function (employee) {
        EmployeeModel.createEmployee(employee, $modalInstance);
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
} ]);