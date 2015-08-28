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
        targetTypeId: '',
        groupId: '',
        employeeIds: '',
        status: ''
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
        EmployeeEPAModel.openPopupUpdateStatusEPA(epa.ProcessByReviewers[0], epa.EPAId, 4, epa.Employee.EmployeeId, status);
    };

    vm.viewEPADetailOfEvaluator = function (memberEPA, evaluator) {
        vm.viewEPADetailOfEvaluatorUrl = '/Evaluator/Detail?epaId=' + memberEPA.EPAId
            + '&employeeId=' + memberEPA.Employee.EmployeeId
            + '&evaluatorId=' + evaluator.EvaluatorId;
    };

    vm.viewEPADetail = function (memberEPA, evaluator) {
        vm.viewEPADetailUrl = '/Reviewer/Detail?epaId=' + memberEPA.EPAId
            + '&employeeId=' + memberEPA.Employee.EmployeeId
            + '&evaluatorId=' + evaluator.EvaluatorId;
    };

    var getEPAByReviewer = function (targetTypeId, groupId, employeeIds, status) {
        EmployeeEPAModel.getEPAByReviewer(targetTypeId, groupId, employeeIds, status).then(function () {
            vm.memberEPAs = EmployeeEPAModel.memberEPAs;
        });
    };

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByReviewer(filter.targetTypeId, filter.groupId, filter.employeeIds, filter.status);
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
    vm.employeeId = parseInt($routeParams.employeeId);
    vm.evaluatorId = parseInt($routeParams.evaluatorId);

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

    var checkStatus = function (epaInfo) {
        if (epaInfo.ProcessByReviewers[0].IsEndProcess) {
            vm.isReadonly = true;
        }
    };

    vm.updateStatusEPA = function (epa, status) {
        EmployeeEPAModel.openPopupUpdateStatusEPA(epa.ProcessByReviewers[0], epa.EPAId, 4, epa.Employee.EmployeeId, status);
    };

    vm.save = function (epaInfo) {
        var totalcore = epaInfo.ProcessByReviewers[0].TotalScore,
            comment = epaInfo.ProcessByReviewers[0].Comment;
        if (EmployeeEPAModel.validateEvaluationOfReviewerOrManagement(totalcore, comment)) {
            var criteria = {
                epaId: epaInfo.EPAId,
                processId: 4,
                employeeId: epaInfo.Employee.EmployeeId,
                score: totalcore,
                comment: comment
            };

            EmployeeEPAModel.updateScoreByReviewerOrManagement(vm.dataSaved, criteria);
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };

    vm.initialize = function () {
        EmployeeEPAModel.getEPAInfoByReviewer(vm.epaId, vm.employeeId, vm.evaluatorId).then(function () {
            var data = EmployeeEPAModel.epaInfo;

            vm.epaInfo = data;
            checkStatus(vm.epaInfo);

            angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
                if (!EmployeeEPAModel.checkKPIOptional(kpiGroup) && kpiGroup.IsSpreadKPI) {
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