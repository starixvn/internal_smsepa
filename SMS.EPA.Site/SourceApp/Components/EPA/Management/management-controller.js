'use strict';
/*global epaModule*/

epaModule.controller('ManagementController',
['EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function (EmployeeEPAModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.memberEPAs = EmployeeEPAModel.memberEPAs;
    vm.members = EmployeeEPAModel.members;

    vm.filter = {
        groupId: '',
        employeeIds: ''
    };

    vm.getMember = function (groupId) {
        EmployeeEPAModel.getMember(4, groupId).then(function () {
            vm.members = EmployeeEPAModel.members;
        });
    };

    vm.viewEPADetail = function (memberEPA) {
        vm.viewEPADetailUrl = '/Management/Detail?epaId=' + memberEPA.EPAId;
    };

    var getEPAByManagement = function (groupId, employeeIds) {
        EmployeeEPAModel.getEPAByManagement(groupId, employeeIds).then(function () {
            vm.memberEPAs = EmployeeEPAModel.memberEPAs;
        });
    };

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByManagement(filter.groupId, filter.employeeIds);
    };

    vm.initialize = function () {
        CommonModel.getAllGroup().then(function () {
            vm.groupOptions = CommonModel.groups;
        });

        vm.getMember(vm.filter.groupId);

        vm.query(vm.filter);
    };

    vm.initialize();
}]);

epaModule.controller('MemberEPAInfoOfManagentController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'utilitiesFactory',
function ($window, $translate, $routeParams, EmployeeEPAModel, utilitiesFactory) {
    var vm = this;

    vm.epaId = parseInt($routeParams.epaId);

    vm.dataSaved = false;
    vm.isReadonly = false;
    vm.evaluatedDate = null;
    vm.finalScore = null;

    var checkStatus = function (epaInfo) {
        if (epaInfo.ProcessByManagements[0].IsEndProcess) {
            vm.isReadonly = true;
        }
    };

    // Event read more / less text.
    utilitiesFactory.maxLengthDescription(vm);

    vm.readMore = function (item) {
        utilitiesFactory.readMore(item);
    };

    vm.readLess = function (item) {
        utilitiesFactory.readLess(item);
    };

    vm.save = function (epaInfo) {
        var totalcore = epaInfo.ProcessByManagements[0].TotalScore,
            comment = epaInfo.ProcessByManagements[0].Comment;
        if (EmployeeEPAModel.validateEvaluationOfReviewerOrManagement(totalcore)) {
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

            if (!data.ProcessByManagements[0].EvaluateDate && data.AverageScoreByEvaluators && parseFloat(data.AverageScoreByEvaluators) > 0) {
                data.ProcessByManagements[0].TotalScore = data.AverageScoreByReviewers;
            }

            if (data.Department.DepartmentId == 2) {
                vm.finalScore = data.ProcessByManagements[0].TotalScore ? (parseFloat(data.ProcessByManagements[0].TotalScore) + data.AttendanceScore) : data.AttendanceScore;
            }
            else {
                vm.finalScore = data.ProcessByManagements[0].TotalScore;
            }

            vm.evaluatedDate = data.ProcessByManagements[0].EvaluateDate ? data.ProcessByManagements[0].EvaluateDate : null;
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