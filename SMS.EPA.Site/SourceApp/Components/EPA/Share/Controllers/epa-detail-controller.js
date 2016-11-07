(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('epaModule').controller('EPADetailController', epaDetailController);
    epaDetailController.$inject = ['$routeParams', 'EvaluationModel', 'CommonModel', 'utilitiesFactory', 'historyFactory',
        'lockScreenService', 'baseUrl'];

    function epaDetailController($routeParams, EvaluationModel, CommonModel, utilitiesFactory, historyFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.epaId = parseInt($routeParams.id);
        vm.allowedExportPdf = $routeParams.exportPdf;
        vm.jobGrades = [];
        vm.standardGrades = [];
        vm.isViewDetail = true;

        vm.back = function () {
            historyFactory.back();
        };

        vm.exportPdf = function () {
            var url = baseUrl + 'ExportPDF/ExportPdf?epaId=' + vm.epaId;
            var iframe = $('body iframe');

            if (iframe.length === 0) {
                iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
                iframe.appendTo('body');
            }

            iframe.attr('src', url);
        };

        function getEPAInfoById() {
            EvaluationModel.getEPAInfoById(vm.epaId).then(function () {
                vm.epaInfo = EvaluationModel.epaInfo;
                vm.epaInfo.Employee.Avatar = baseUrl + vm.epaInfo.Employee.Avatar;

                // self
                vm.epaInfo.Process.Self.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.Self.TotalScore);

                // evaluator
                angular.forEach(vm.epaInfo.Process.Evaluators, function (evaluator) {
                    evaluator.TotalScore = utilitiesFactory.parseFloat(evaluator.TotalScore);
                });

                // reviewer
                angular.forEach(vm.epaInfo.Process.Reviewers, function (reviewer) {
                    reviewer.TotalScore = utilitiesFactory.parseFloat(reviewer.TotalScore);
                });

                // management
                angular.forEach(vm.epaInfo.Process.Managements, function (management) {
                    management.TotalScore = utilitiesFactory.parseFloat(management.TotalScore);
                });

                if (vm.epaInfo.Period.IsEndPeriod && vm.epaInfo.FinalScore.length > 0) {
                    vm.epaInfo.FinalScore = utilitiesFactory.parseFloat(vm.epaInfo.FinalScore);
                    vm.epaInfo.Grade = EvaluationModel.convertScoreToGrade(vm.epaInfo.FinalScore, vm.standardGrades);
                } else {
                    vm.epaInfo.FinalScore = null;
                }

                if (vm.epaInfo.Recommendation && vm.epaInfo.Recommendation.JobGrade && vm.epaInfo.Recommendation.JobGrade.JobGradeId) {
                    var index = vm.jobGrades.map(function (e) { return e.JobGradeId; }).indexOf(vm.epaInfo.Recommendation.JobGrade.JobGradeId);

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