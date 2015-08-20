'use strict';
/*global epaModule*/

epaModule.controller('EvaluatorController',
['EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function (EmployeeEPAModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.memberEPAs = EmployeeEPAModel.memberEPAs;
    vm.targetTypes = CommonModel.targetTypes;
    vm.statusList = CommonModel.statusList;
    vm.members = EmployeeEPAModel.members;

    vm.status = {
        isopen: false
    };
    vm.epaStatus = EmployeeEPAModel.epaStatus;

    vm.filter = {
        fromDate: new Date(),
        toDate: new Date(),
        targetTypeId: '',
        groupId: '',
        employeeIds: '',
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

    vm.toggleDropdown = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        vm.status.isopen = !vm.status.isopen;
    };

    vm.getMember = function (groupId) {
        EmployeeEPAModel.getMember(3, groupId).then(function () {
            vm.members = EmployeeEPAModel.members;
        });
    };

    vm.updateStatusEPA = function (epa, status) {
        EmployeeEPAModel.openPopupUpdateStatusEPA(epa, epa.EPAId, epa.PeriodId, 3, epa.Employee.EmployeeId, status);
    };

    var getEPAByEvaluator = function (fromDate, toDate, targetTypeId, groupId, employeeIds, status) {
        EmployeeEPAModel.getEPAByEvaluator(fromDate, toDate, targetTypeId, groupId, employeeIds, status).then(function () {
            vm.memberEPAs = EmployeeEPAModel.memberEPAs;
        });
    };

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');

        getEPAByEvaluator(filter.fromDate, filter.toDate, filter.targetTypeId, filter.groupId, filter.employeeIds, filter.status);
    };

    vm.viewEPAInfoOfMember = function (memberEPA) {
        EmployeeEPAModel.viewEPAInfoOfMember(memberEPA.EPAId, memberEPA.PeriodId, memberEPA.Employee.EmployeeId);
    };

    vm.viewEPADetail = function (memberEPA) {
        vm.viewEPADetailUrl = '/Evaluator/Detail?epaId=' + memberEPA.EPAId + '&periodId=' + memberEPA.PeriodId + '&employeeId=' + memberEPA.Employee.EmployeeId;
    };

    vm.initialize = function () {
        CommonModel.getTargetType().then(function () {
            vm.targetTypes = CommonModel.targetTypes;
        });

        CommonModel.getAllGroup().then(function () {
            vm.groupOptions = CommonModel.groups;
        });

        CommonModel.getAllStatus().then(function () {
            vm.statusList = CommonModel.statusList;
        });

        vm.getMember(vm.filter.groupId);

        vm.query(vm.filter);
    };

    vm.initialize();
}]);

epaModule.controller('MemberEPAInfoController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function ($window, $translate, $routeParams, EmployeeEPAModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.epaId = parseInt($routeParams.epaId);
    vm.periodId = parseInt($routeParams.periodId);
    vm.employeeId = parseInt($routeParams.employeeId);

    vm.maxStar = 5;
    vm.dataSaved = false;
    vm.isReadonly = false;

    vm.cancel = function () {
        $window.history.back();
    };

    vm.initialize = function () {
        EmployeeEPAModel.getEPAInfoByEvaluator(vm.epaId, vm.periodId, vm.employeeId).then(function () {
            var data = EmployeeEPAModel.epaInfo;

            vm.employee = data.EmployeeEPAInfo.Employee;
            vm.evaluators = data.EmployeeEPAInfo.Evaluators;
            vm.memberEPA = data.EmployeeEPAInfo;
            vm.epaInfo = data;
        });
    };

    vm.initialize();
}]);

kpiModule.controller('ViewEPAInfoOfMemberController',
['$modalInstance', 'EmployeeEPAModel', 'epaId', 'periodId', 'employeeId',
function ($modalInstance, EmployeeEPAModel, epaId, periodId, employeeId) {
    var vm = this;

    vm.isReadonly = true;
    vm.maxStar = 5;

    EmployeeEPAModel.getEPAInfoByEmployee(epaId, periodId, employeeId).then(function () {
        var data = EmployeeEPAModel.epaInfo;

        vm.employee = data.Employee;
        vm.evaluators = data.Evaluators;
        vm.epaInfo = data;

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

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
}]);