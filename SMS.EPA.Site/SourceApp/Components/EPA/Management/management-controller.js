'use strict';
/*global epaModule*/

epaModule.controller('ManagementController',
['EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function (EmployeeEPAModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.memberEPAs = EmployeeEPAModel.memberEPAs;
    vm.statusList = CommonModel.statusList;
    vm.members = EmployeeEPAModel.members;

    vm.status = {
        isopen: false
    };
    vm.epaStatus = EmployeeEPAModel.epaStatus;

    vm.filter = {
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
        EmployeeEPAModel.openPopupUpdateStatusEPA(epa.ProcessByManagements[0], epa.EPAId, 5, epa.Employee.EmployeeId, status);
    };

    vm.viewEPADetail = function (memberEPA, evaluator) {
        vm.viewEPADetailUrl = '/Management/Detail?epaId=' + memberEPA.EPAId;
    };

    var getEPAByManagement = function (groupId, employeeIds, status) {
        EmployeeEPAModel.getEPAByManagement(groupId, employeeIds, status).then(function () {
            vm.memberEPAs = EmployeeEPAModel.memberEPAs;
        });
    };

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByManagement(filter.groupId, filter.employeeIds, filter.status);
    };

    vm.initialize = function () {
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

epaModule.controller('MemberEPAInfoOfManagentController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function ($window, $translate, $routeParams, EmployeeEPAModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.epaId = parseInt($routeParams.epaId);

    vm.dataSaved = false;
    vm.isReadonly = false;

    var checkStatus = function (epaInfo) {
        if (epaInfo.ProcessByManagements[0].IsEndProcess) {
            vm.isReadonly = true;
        }
    };

    vm.save = function (epaInfo) {
        var totalcore = epaInfo.ProcessByManagements[0].TotalScore,
            comment = epaInfo.ProcessByManagements[0].Comment;
        if (EmployeeEPAModel.validateEvaluationOfReviewerOrManagement(totalcore, comment)) {
            var criteria = {
                epaId: epaInfo.EPAId,
                processId: 5,
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
        EmployeeEPAModel.getEPAInfoByManagement(vm.epaId).then(function () {
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