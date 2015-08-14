'use strict';
/*global epaModule*/

epaModule.controller('EmployeeEPAController',
['EmployeeEPAModel', 'CommonModel',
function (EmployeeEPAModel, CommonModel) {
    var vm = this;

    vm.myEPAs = EmployeeEPAModel.listEPA;
    vm.statusList = CommonModel.statusList;

    vm.filter = {
        status: ''
    };

    var getEPAOfCurrentMember = function (status) {
        EmployeeEPAModel.getEPAOfCurrentMember(status).then(function () {
            vm.listEPA = EmployeeEPAModel.listEPA;
        });
    };

    vm.query = function (filter) {
        getEPAOfCurrentMember(filter.status);
    };

    vm.viewEPADetail = function (epa) {
        vm.viewEPADetailUrl = '/MyEPA/Index?employeeId=' + epa.EmployeeId;
    };

    vm.initialize = function () {
        CommonModel.getAllStatus().then(function () {
            vm.statusList = CommonModel.statusList;
        });

        getEPAOfCurrentMember(vm.filter.status);
    };

    vm.initialize();
}]);

epaModule.controller('EmployeeEPAInfoController',
['$routeParams', 'EmployeeEPAModel',
function ($routeParams, EmployeeEPAModel) {
    var vm = this;

    vm.max = 5;
    vm.employeeId = parseInt($routeParams.employeeId);

    vm.epaInfo = EmployeeEPAModel.epaInfo;

    vm.initialize = function () {
        EmployeeEPAModel.getEPAInformation(vm.employeeId).then(function () {
            vm.epaInfo = EmployeeEPAModel.epaInfo;
        });
    };

    vm.initialize();
}]);