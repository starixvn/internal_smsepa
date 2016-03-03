'use strict';
/*global epaModule*/

epaModule.controller('SemiMonthlyController',
['$window', '$timeout', '$translate', '$routeParams', 'EmployeeEPAModel', 'SelfEvalutorModel', 'CommonModel', 'utilitiesFactory', 'baseUrl',
function ($window, $timeout, $translate, $routeParams, EmployeeEPAModel, SelfEvalutorModel, CommonModel, utilitiesFactory, baseUrl) {
    var vm = this;
    vm.epaId = parseInt($routeParams.id);

    vm.standardGrades = CommonModel.standardGrades;

    vm.getClassProgress = function (statusEndProcess) {
        return EmployeeEPAModel.getClassProgress(statusEndProcess);
    };

    vm.cancel = function () {
        $window.history.back();
    };

    var getEPAInfoByEvaluator = function () {
        SelfEvalutorModel.getEPAInfoByEvaluator(vm.epaId).then(function () {
            vm.epaInfo = SelfEvalutorModel.epaInfo;
            checkStatus(vm.epaInfo);

            vm.epaInfo.Employee.Avatar = baseUrl + vm.epaInfo.Employee.Avatar;
            vm.epaInfo.ProcessBySelf.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.ProcessBySelf.TotalScore);
            vm.epaInfo.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.CurrentProcess.TotalScore);
            vm.maxPerformanceScoreOfSelf = vm.epaInfo.ProcessBySelf.MaxTotalScore;
            vm.maxPerformanceScore = vm.epaInfo.CurrentProcess.MaxTotalScore;

            if (vm.epaInfo.HasDisciplineScore) {
                if (vm.epaInfo.DisciplineScore < 0) {
                    vm.epaInfo.DisciplineScore = 0;
                }

                vm.maxPerformanceScoreOfSelf += vm.epaInfo.MaxDisciplineScore;
                vm.maxPerformanceScore += vm.epaInfo.MaxDisciplineScore;
            }

            vm.finalScoreOfSelf = vm.epaInfo.ProcessBySelf.TotalScore + vm.epaInfo.DisciplineScore;
            vm.finalScoreOfSelf = EmployeeEPAModel.checkScore(vm.finalScoreOfSelf, 0, vm.maxPerformanceScoreOfSelf);
            vm.finalScore = vm.epaInfo.CurrentProcess.TotalScore + vm.epaInfo.DisciplineScore;
            vm.finalScore = EmployeeEPAModel.checkScore(vm.finalScore, 0, vm.maxPerformanceScore);

            if (vm.epaInfo.LastFinalScore.length > 0) {
                vm.epaInfo.LastFinalScore = utilitiesFactory.parseFloat(vm.epaInfo.LastFinalScore);
            } else {
                vm.epaInfo.LastFinalScore = null;
            }

            vm.gradeOfSelf = EmployeeEPAModel.convertScoreToGrade(vm.finalScoreOfSelf, vm.standardGrades);
            vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
            vm.evaluatedDate = vm.epaInfo.CurrentProcess.EvaluateDate ? vm.epaInfo.CurrentProcess.EvaluateDate : null;

            angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
                EmployeeEPAModel.scoreSpread(kpiGroup);
            });

            SelfEvalutorModel.getUserPermissionOnEvaluator().then(function () {
                vm.permission = SelfEvalutorModel.permission;

                if (vm.permission !== 2) {
                    vm.isReadonly = true;
                    vm.hoveringLeave = null;
                }
            });
        });
    };

    vm.initialize = function () {
        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;

            getEPAInfoByEvaluator();
        });
    };

    vm.initialize();
}]);