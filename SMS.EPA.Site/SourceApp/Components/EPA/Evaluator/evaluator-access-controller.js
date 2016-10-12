'use strict';
/*global epaModule*/

epaModule.controller('EvaluatorController',
['$routeParams', 'EmployeeEPAModel', 'EvaluatorEPAModel', 'SelfEvalutorModel', 'CommonModel',
    'utilitiesFactory', 'historyFactory', 'lockScreenService', 'baseUrl',
function ($routeParams, EmployeeEPAModel, EvaluatorEPAModel, SelfEvalutorModel, CommonModel,
    utilitiesFactory, historyFactory, lockScreenService, baseUrl) {
    var vm = this;

    vm.epaInfos = [];
    vm.members = [];
    vm.standardGrades = CommonModel.standardGrades;
    vm.targetTypeEnum = utilitiesFactory.targetType;
    vm.sortBySelfScore = { ascending: false, descending: false };
    vm.filter = {};
    vm.monthlyFilterSelected = false;

    function initFilter() {
        var filter = historyFactory.getState();
        return filter ? filter : {
            groupId: '',
            isLocal: '',
            statusProcess: $routeParams.statusProcess ? $routeParams.statusProcess : '',
            isMonthly: $routeParams.isMonthly ? $routeParams.isMonthly === 'true' : false,
            employeeIds: ''
        };
    }

    function sortBySelfScore(epaInfo1, epaInfo2) {
        var tempa = epaInfo1.Process.Self.TotalScore ? parseFloat(epaInfo1.Process.Self.TotalScore) : 0,
            tempb = epaInfo2.Process.Self.TotalScore ? parseFloat(epaInfo2.Process.Self.TotalScore) : 0;

        if (epaInfo1.IsSemiHasMonthly) {
            tempa = epaInfo1.MonthlyEPA.AverageSelfScoreConverted;
        }

        if (epaInfo2.IsSemiHasMonthly) {
            tempb = epaInfo2.MonthlyEPA.AverageSelfScoreConverted;
        }

        return tempa - tempb;
    }

    vm.changeSorting = function () {
        if (!vm.sortBySelfScore.ascending) {
            vm.sortBySelfScore.ascending = true;
            vm.sortBySelfScore.descending = false;
            vm.epaInfos.sort(sortBySelfScore);
        } else {
            vm.sortBySelfScore.ascending = false;
            vm.sortBySelfScore.descending = true;
            vm.epaInfos.reverse();
        }
    };

    vm.getMember = function () {
        EvaluatorEPAModel.getMemberByEvaluator(vm.filter.groupId, vm.filter.isLocal, vm.filter.isMonthly).then(function () {
            vm.members = EvaluatorEPAModel.members;
            utilitiesFactory.setSelectedItem(vm.members, "EmployeeId", vm.filter.employeeIds);
        });
    };

    vm.selectedCls = function () {
        if (vm.sortBySelfScore.ascending) {
            return 'sorting-asc';
        } else if (vm.sortBySelfScore.descending) {
            return 'sorting-desc';
        }
    };

    function getEPAByEvaluator(groupId, isLocal, isMonthly, employeeIds, statusProcess) {
        lockScreenService.start();
        vm.sortBySelfScore = { ascending: false, descending: false };

        EvaluatorEPAModel.getEPAByEvaluator(groupId, isLocal, isMonthly, employeeIds, statusProcess).then(function () {
            vm.epaInfos = EvaluatorEPAModel.epaInfos;

            angular.forEach(vm.epaInfos, function (epaInfo) {
                if (epaInfo.IsSemiHasMonthly) {
                    epaInfo.Process.Self.TotalScore = epaInfo.MonthlyEPA.AverageSelfScoreConverted;
                    epaInfo.FinalScore = utilitiesFactory.parseFloat(epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted) + epaInfo.DisciplineScore;
                } else {
                    epaInfo.Process.Self.TotalScore = utilitiesFactory.parseFloat(epaInfo.Process.Self.TotalScore);
                    epaInfo.FinalScore = utilitiesFactory.parseFloat(epaInfo.Process.CurrentProcess.TotalScore) + epaInfo.DisciplineScore;
                }
                epaInfo.FinalScore = EmployeeEPAModel.checkScore(epaInfo.FinalScore, 0, epaInfo.Process.CurrentProcess.MaxPerformanceScore);

                if (epaInfo.LastFinalScore.length > 0) {
                    epaInfo.LastFinalScore = utilitiesFactory.parseFloat(epaInfo.LastFinalScore);
                } else {
                    epaInfo.LastFinalScore = null;
                }

                epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(epaInfo.FinalScore, vm.standardGrades);
            });

            if (isMonthly) {
                vm.monthlyFilterSelected = true;
            } else {
                vm.monthlyFilterSelected = false;
            }

            lockScreenService.stop();
        });

        historyFactory.saveState(vm.filter);
    }

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByEvaluator(filter.groupId, filter.isLocal, filter.isMonthly, filter.employeeIds, filter.statusProcess);
    };

    vm.viewEPADetail = function (epaInfo) {
        vm.viewEPADetailUrl = baseUrl + 'Evaluator/Detail/' + epaInfo.EPAId;
    };

    vm.viewEPAResult = function (epaInfo) {
        vm.viewEPAResultUrl = baseUrl + 'EPA/Detail/' + epaInfo.EPAId;
    };

    vm.viewOtherViolationDetail = function (epaInfo) {
        CommonModel.openPopupViewOtherViolationDetail(epaInfo.EPAId);
    };

    vm.viewEPAHistory = function (employeeId) {
        vm.viewEPAHistoryUrl = baseUrl + 'EPAHistory/Index?empId=' + employeeId;
    };

    function initialize() {
        vm.filter = initFilter();
        EvaluatorEPAModel.getGroupByEvaluator().then(function () {
            vm.groupOptions = EvaluatorEPAModel.groups;
        });

        vm.getMember();

        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;
            getEPAByEvaluator(vm.filter.groupId, vm.filter.isLocal, vm.filter.isMonthly, vm.filter.employeeIds, vm.filter.statusProcess);
        });
    }

    initialize();
}]);