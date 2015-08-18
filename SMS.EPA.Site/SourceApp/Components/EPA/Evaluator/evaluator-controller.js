'use strict';
/*global epaModule*/

epaModule.controller('EvaluatorController',
['EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function (EmployeeEPAModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.listEPA = [];
    vm.targetTypes = CommonModel.targetTypes;
    vm.members = EmployeeEPAModel.members;

    vm.filter = {
        fromDate: new Date(),
        toDate: new Date(),
        targetTypeId: '',
        groupId: '',
        employeeId: '',
        status: ''
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

    vm.getMember = function (groupId) {
        EmployeeEPAModel.getMember(3, groupId).then(function () {
            vm.members = EmployeeEPAModel.members;
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

        vm.getMember(vm.filter.groupId);
    };

    vm.initialize();
}]);