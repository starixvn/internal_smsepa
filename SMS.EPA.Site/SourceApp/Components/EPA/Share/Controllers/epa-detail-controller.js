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
        vm.jobLevels = [];
        vm.standardGrades = [];
        vm.isViewDetail = true;

        vm.back = function () {
            if ($routeParams.detailRpt) {
                $location.path(baseUrl + 'Report/EPADetailReport').search({
                        periodId: $routeParams.periodId,
                        departmentId: $routeParams.departmentId,
                        phaseIds: $routeParams.phaseIds,
                        isLocal: $routeParams.isLocal,
                        employeeId: $routeParams.employeeId
                    });
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

                    var result = EvaluationModel.calculateScoreAndGradeToDisplay(vm.epaInfo);

                    vm.epaInfo.FinalScore = result.FinalScore.toString();
                    vm.epaInfo.Grade = result.Grade;
                }

                lockScreenService.stop();
            });
        }

        function initialize() {
            lockScreenService.start();

            CommonModel.getJobLevel(null).then(function () {
                vm.jobLevels = CommonModel.jobLevels;
            });

            CommonModel.getStandardGrade().then(function () {
                vm.standardGrades = CommonModel.standardGrades;

                getEPAInfoById();
            });
        }

        initialize();
    }
})(angular);