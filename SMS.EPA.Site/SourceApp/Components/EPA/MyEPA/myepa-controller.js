'use strict';
/*global epaModule*/

epaModule.controller('EmployeeEPAController',
['EmployeeEPAModel', 'CommonModel',
function (EmployeeEPAModel, CommonModel) {
    var vm = this;

    vm.myEPAs = EmployeeEPAModel.myEPAs;
    vm.statusList = CommonModel.statusList;
    vm.targetTypes = CommonModel.targetTypes;

    vm.status = {
        isopen: false
    };
    vm.epaStatus = EmployeeEPAModel.epaStatus;

    vm.filter = {
        targetTypeId: '',
        status: ''
    };

    vm.toggleDropdown = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        vm.status.isopen = !vm.status.isopen;
    };

    var getEPAOfCurrentMember = function (targetTypeId, status) {
        EmployeeEPAModel.getEPAOfCurrentMember(targetTypeId, status).then(function () {
            vm.myEPAs = EmployeeEPAModel.myEPAs;
        });
    };

    vm.query = function (filter) {
        getEPAOfCurrentMember(filter.targetTypeId, filter.status);
    };

    vm.updateStatusEPA = function (epa, status) {
        EmployeeEPAModel.openPopupUpdateStatusEPA(epa, epa.EPAId, epa.PeriodId, 2, epa.Employee.EmployeeId, status);
    };

    vm.viewEPADetail = function (epa) {
        vm.viewEPADetailUrl = '/SelfEvaluation/Detail?epaId=' + epa.EPAId + '&periodId=' + epa.PeriodId + '&employeeId=' + epa.Employee.EmployeeId;
    };

    vm.initialize = function () {
        CommonModel.getTargetType().then(function () {
            vm.targetTypes = CommonModel.targetTypes;
        });

        CommonModel.getAllStatus().then(function () {
            vm.statusList = CommonModel.statusList;
        });

        vm.query(vm.filter);
    };

    vm.initialize();
}]);

epaModule.controller('EmployeeEPAInfoController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel',
function ($window, $translate, $routeParams, EmployeeEPAModel) {
    var vm = this;

    vm.epaId = parseInt($routeParams.epaId);
    vm.periodId = parseInt($routeParams.periodId);
    vm.employeeId = parseInt($routeParams.employeeId);

    vm.maxStar = 5;
    vm.dataSaved = false;
    vm.isReadonly = false;

    vm.epaInfo = EmployeeEPAModel.epaInfo;

    vm.status = {
        isopen: false
    };
    vm.epaStatus = EmployeeEPAModel.epaStatus;

    vm.toggleDropdown = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        vm.status.isopen = !vm.status.isopen;
    };

    vm.checkStatus = function (isEndProcess) {
        if (isEndProcess) {
            vm.isReadonly = true;
            vm.hoveringOver = null;
            vm.hoveringLeave = null;
        }
        else {
            vm.hoveringOver = function (value, kpi) {
                kpi.Percent = (100 * value) / vm.maxStar;
            };

            vm.hoveringLeave = function (kpiGroup, kpi) {
                kpi.Percent = (100 * kpi.Rate) / vm.maxStar;

                if (kpiGroup.IsSpreadKPI && kpi.Percent > 0) {
                    kpi.Score = (kpiGroup.Weight / kpiGroup.KPIs.length) * (kpi.Percent / 100);
                }
                else if (kpi.Percent > 0) {
                    kpi.Score = kpi.Weight * (kpi.Percent / 100);
                }

                if (kpi.Rate > 0) {
                    EmployeeEPAModel.calculateScore(vm, vm.epaInfo);
                }
            };
        }
    };

    vm.cancelRating = function (kpi) {
        kpi.Rate = 0;
        kpi.Percent = 0;
        kpi.Score = null;

        EmployeeEPAModel.calculateScore(vm, vm.epaInfo);
    };

    vm.inputScore = function (kpi) {
        kpi.Rate = 0;
        kpi.Percent = 0;

        if (!EmployeeEPAModel.validateWeightInput(kpi.Score, kpi.Weight, vm.weightInputValidMessage)) {
            kpi.Score = null;
        }
        else {
            EmployeeEPAModel.calculateScore(vm, vm.epaInfo);
        }
    };

    vm.updateStatusEPA = function (status) {
        EmployeeEPAModel.openPopupUpdateStatusEPA(vm.epaInfo, vm.epaId, vm.periodId, 2, vm.employeeId, status);
    };

    vm.save = function (epaInfo) {
        if (EmployeeEPAModel.validateEPA(epaInfo)) {
            EmployeeEPAModel.updateEmployeeEPA(vm.dataSaved, epaInfo);
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };

    vm.initialize = function () {
        $translate('WeightInputValid').then(function (message) {
            vm.weightInputValidMessage = message;
        });

        EmployeeEPAModel.getEPAInfoByEmployee(vm.epaId, vm.periodId, vm.employeeId).then(function () {
            var data = EmployeeEPAModel.epaInfo;

            vm.employee = data.Employee;
            vm.evaluators = data.Evaluators;
            vm.epaInfo = data;
            vm.checkStatus(vm.epaInfo.IsEndProcess);

            angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
                if (kpiGroup.IsSpreadKPI) {
                    var weight = kpiGroup.Weight / kpiGroup.KPIs.length;
                    angular.forEach(kpiGroup.KPIs, function (kpi) {
                        if ((weight % 1) === 0) {
                            kpi.Weight = weight;
                        }
                        else {
                            kpi.Weight = weight.toFixed(2);
                        }
                    });
                }
            });
        });
    };

    vm.initialize();
}]);