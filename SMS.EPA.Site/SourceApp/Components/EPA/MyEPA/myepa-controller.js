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
        vm.viewEPADetailUrl = '/MyEPA/Index?epaId=' + epa.EPAId + '&periodId=' + epa.PeriodId + '&employeeId=' + epa.EmployeeId;
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
['$window', '$routeParams', 'EmployeeEPAModel',
function ($window, $routeParams, EmployeeEPAModel) {
    var vm = this;

    vm.epaId = parseInt($routeParams.epaId);
    vm.periodId = parseInt($routeParams.periodId);
    vm.employeeId = parseInt($routeParams.employeeId);

    vm.maxStar = 5;

    vm.epaInfo = EmployeeEPAModel.epaInfo;

    vm.hoveringOver = function (value, kpi) {
        kpi.Percent = (100 * value) / vm.maxStar;
    };

    vm.hoveringLeave = function (kpiGroup, kpi) {
        kpi.Percent = (100 * kpi.Rate) / vm.maxStar;

        if (kpiGroup.IsSpreadKPI) {
            kpi.Score = (kpiGroup.Weight / kpiGroup.KPIs.length) * (kpi.Percent / 100);
        }
        else {
            kpi.Score = kpi.Weight * (kpi.Percent / 100);
        }

        if (kpi.Rate > 0) {
            EmployeeEPAModel.calculateScore(vm, vm.epaInfo);
        }
    };

    vm.inputScore = function (kpi) {
        kpi.Rate = 0;
        kpi.Percent = 0;
        EmployeeEPAModel.calculateScore(vm, vm.epaInfo);
    };

    vm.save = function (epaInfo) {
        if (EmployeeEPAModel.validateEPA(epaInfo)) {
            EmployeeEPAModel.updateEmployeeEPA(epaInfo);
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };

    vm.initialize = function () {
        EmployeeEPAModel.getEPAInformation(vm.epaId, vm.periodId, vm.employeeId).then(function () {
            vm.epaInfo = EmployeeEPAModel.epaInfo;
        });
    };

    vm.initialize();
}]);