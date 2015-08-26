'use strict';
/*global epaModule*/

epaModule.controller('ReviewerController',
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
        EmployeeEPAModel.getMember(4, groupId).then(function () {
            vm.members = EmployeeEPAModel.members;
        });
    };

    vm.updateStatusEPA = function (epa, status) {
        EmployeeEPAModel.openPopupUpdateStatusEPA(epa, epa.EPAId, epa.PeriodId, 4, epa.Employee.EmployeeId, status);
    };

    vm.viewEvaluationDetail = function (memberEPA, evaluator) {
        vm.viewEvaluationDetailUrl = '/Evaluator/Detail?epaId=' + memberEPA.EPAId + '&periodId=' + memberEPA.PeriodId
            + '&employeeId=' + memberEPA.Employee.EmployeeId + '&evaluatorId=' + evaluator.EvaluatorId + '&viewDetail';
    };

    vm.viewEPADetail = function (memberEPA) {
        vm.viewEPADetailUrl = '/Reviewer/Detail?epaId=' + memberEPA.EPAId + '&periodId=' + memberEPA.PeriodId
            + '&employeeId=' + memberEPA.Employee.EmployeeId;
    };

    var getEPAByReviewer = function (fromDate, toDate, targetTypeId, groupId, employeeIds, status) {
        EmployeeEPAModel.getEPAByReviewer(fromDate, toDate, targetTypeId, groupId, employeeIds, status).then(function () {
            vm.memberEPAs = EmployeeEPAModel.memberEPAs;
        });
    };

    vm.query = function (filter) {
        if (EmployeeEPAModel.validateFromToDate(filter.fromDate, filter.toDate)) {
            filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
            getEPAByReviewer(filter.fromDate, filter.toDate, filter.targetTypeId, filter.groupId, filter.employeeIds, filter.status);
        }
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

epaModule.controller('MemberEPAInfoOfReviewerController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function ($window, $translate, $routeParams, EmployeeEPAModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.epaId = parseInt($routeParams.epaId);
    vm.periodId = parseInt($routeParams.periodId);
    vm.employeeId = parseInt($routeParams.employeeId);

    vm.maxStar = 5;
    vm.dataSaved = false;
    vm.isReadonly = false;

    vm.status = {
        isopen: false
    };
    vm.epaStatus = EmployeeEPAModel.epaStatus;

    vm.toggleDropdown = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        vm.status.isopen = !vm.status.isopen;
    };

    vm.cancel = function () {
        $window.history.back();
    };
}]);