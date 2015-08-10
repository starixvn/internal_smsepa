'use strict';
/*global epaModule*/

epaModule.controller('MyEPAController',
['EmployeeEPAModel', 'CommonModel',
function (EmployeeEPAModel, CommonModel) {
    var vm = this;

    vm.myEPAs = EmployeeEPAModel.myEPAs;
    vm.statusList = CommonModel.statusList;

    vm.filter = {
        status: ''
    };

    var getEPAOfCurrentMember = function (status) {
        EmployeeEPAModel.getEPAOfCurrentMember(status).then(function () {
            vm.myEPAs = EmployeeEPAModel.myEPAs;
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