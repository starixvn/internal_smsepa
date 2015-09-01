'use strict';
/*global epaModule*/

epaModule.controller('ReviewerController',
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
        vm.viewEPADetailUrl = '/Reviewer/Detail?epaId=' + memberEPA.EPAId;
    };

    var getEPAByReviewer = function (groupId, employeeIds) {
        EmployeeEPAModel.getEPAByReviewer(groupId, employeeIds).then(function () {
            vm.memberEPAs = EmployeeEPAModel.memberEPAs;
        });
    };

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByReviewer(filter.groupId, filter.employeeIds);
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

epaModule.controller('MemberEPAInfoOfReviewerController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel',
function ($window, $translate, $routeParams, EmployeeEPAModel) {
    var vm = this;

    vm.epaId = parseInt($routeParams.epaId);

    vm.dataSaved = false;
    vm.isReadonly = false;
    vm.evaluatedDate = null;
    vm.finalScore = null;

    var checkStatus = function (epaInfo) {
        if (epaInfo.ProcessByReviewers[0].IsEndProcess) {
            vm.isReadonly = true;
        }
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

            if (data.AverageScoreByEvaluators && parseFloat(data.AverageScoreByEvaluators) > 0) {
                data.ProcessByReviewers[0].TotalScore = data.AverageScoreByEvaluators;
            }

            if (data.Department.DepartmentId == 2) {
                vm.finalScore = data.ProcessByReviewers[0].TotalScore ? (parseFloat(data.ProcessByReviewers[0].TotalScore) + data.AttendanceScore) : data.AttendanceScore;
            }
            else {
                vm.finalScore = data.ProcessByReviewers[0].TotalScore;
            }

            vm.evaluatedDate = data.ProcessByReviewers[0].EvaluateDate ? data.ProcessByReviewers[0].EvaluateDate : null;
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