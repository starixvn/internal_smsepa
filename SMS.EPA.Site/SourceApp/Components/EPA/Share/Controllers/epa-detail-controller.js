(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('epaModule').controller('EPADetailController', epaDetailController);
    epaDetailController.$inject = ['$location', '$routeParams', 'EvaluationModel', 'CommonModel', 'utilitiesFactory', 'historyFactory',
        'lockScreenService', 'baseUrl'];

    function epaDetailController($location, $routeParams, EvaluationModel, CommonModel, utilitiesFactory, historyFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.epaId = parseInt($routeParams.id);
        vm.allowedExportPdf = $routeParams.exportPdf;
        vm.jobGrades = [];
        vm.standardGrades = [];
        vm.isViewDetail = true;

        vm.back = function () {
            if ($routeParams.detailRpt) {
                $location.url(baseUrl + 'Report/EPADetailReport?periodId=' + $routeParams.periodId
                    + '&departmentId=' + $routeParams.departmentId
                    + '&phaseIds=' + $routeParams.phaseIds
                    + '&isLocal=' + $routeParams.isLocal
                    + '&employeeId=' + $routeParams.employeeId);
            } else {
                historyFactory.back();
            }
        };

        vm.exportPdf = function () {
            var url = baseUrl + 'ExportPdf/ExportPdf/' + vm.epaId;
            var iframe = $('body iframe');

            if (iframe.length === 0) {
                iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
                iframe.appendTo('body');
            }

            iframe.attr('src', url);
        };

        function getEPAInfoById() {
            EvaluationModel.getEPAInfoById(vm.epaId).then(function() {
                vm.epaInfo = EvaluationModel.epaInfo;
                vm.epaInfo.Employee.Avatar = baseUrl + vm.epaInfo.Employee.Avatar;

                if (vm.epaInfo.Process.PerformanceScore !== null) {

                    debugger;

                    vm.epaInfo.FinalScore =
                        (vm.epaInfo.Process.PerformanceScore + vm.epaInfo.DisciplineScore).toString();

                    vm.epaInfo.Grade = EvaluationModel.convertScoreToGrade(
                        vm.epaInfo.Process.PerformanceScore,
                        vm.epaInfo.DisciplineScore,
                        vm.epaInfo.HasDisciplineScore,
                        vm.epaInfo.MaxFinalScore - vm.epaInfo.MaxDisciplineScore,
                        vm.standardGrades);
                }

                if (vm.epaInfo.Recommendation &&
                    vm.epaInfo.Recommendation.JobGrade &&
                    vm.epaInfo.Recommendation.JobGrade.JobGradeId) {
                    var index = vm.jobGrades.map(function(e) { return e.JobGradeId; })
                        .indexOf(vm.epaInfo.Recommendation.JobGrade.JobGradeId);

                    if (index === -1) {
                        vm.jobGrades.push(vm.epaInfo.Recommendation.JobGrade);
                    }
                }

                lockScreenService.stop();
            });
        }

        function initialize() {
            lockScreenService.start();

            CommonModel.getJobGrade(true).then(function () {
                vm.jobGrades = CommonModel.jobGrades;
            });

            CommonModel.getStandardGrade().then(function () {
                vm.standardGrades = CommonModel.standardGrades;

                getEPAInfoById();
            });
        }

        initialize();
    }
})(angular);