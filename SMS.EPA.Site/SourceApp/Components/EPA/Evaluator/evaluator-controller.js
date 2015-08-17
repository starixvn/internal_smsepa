'use strict';
/*global epaModule*/

epaModule.controller('EvaluatorController',
['EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function (EmployeeEPAModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.targetTypes = CommonModel.targetTypes;
    vm.employees = CommonModel.employees;

    vm.filter = {
        fromDate: new Date(),
        toDate: new Date(),
        targetTypeId: '',
        groupId: '',
        employeeId: '',
        statusId: ''
    };

    utilitiesFactory.getDateFormat(vm);

    vm.calendar = {
        startDateOpened: false,
        endDateOpened: false
    };

    vm.openPopupStartDate = function ($event) {
        utilitiesFactory.openPopupStartDate($event, vm);
    };

    vm.openPopupEndDate = function ($event) {
        utilitiesFactory.openPopupEndDate($event, vm);
    };

    vm.getEmployeeByGroup = function (groupId) {
        CommonModel.getAllEmployee('', groupId, null, false).then(function () {
            vm.employees = CommonModel.employees;
        });
    };

    vm.query = function (filter) {
    };

    vm.initialize = function () {
        CommonModel.getTargetType().then(function () {
            vm.targetTypes = CommonModel.targetTypes;
        });

        CommonModel.getAllGroup().then(function () {
            vm.groupOptions = CommonModel.groups;
        });

        vm.getEmployeeByGroup(vm.filter.groupId);
    };

    vm.initialize();
}]);