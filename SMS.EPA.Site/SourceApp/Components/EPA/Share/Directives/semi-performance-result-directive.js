(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').directive('epaSemiPerformanceResult', performanceResult);
    performanceResult.$inject = ["$filter", "EvaluationModel"];

    function performanceResult($filter, EvaluationModel) {
        return {
            restrict: 'E',
            scope: {
                epaInfo: '='
            },
            controller: function () {
                var vm = this;

                vm.process = vm.epaInfo ? angular.copy(vm.epaInfo.Process) : null;

                vm.range = function (n) {
                    return new Array(n);
                };

                vm.SelfScore = EvaluationModel.getScoreData($filter,
                    vm.process.Self.StatusId,
                    vm.epaInfo.MonthlyEPA.AverageSelfScoreConverted,
                    vm.epaInfo.ScoreRatingType.SelfEvaluated);

                vm.EvaluatorScore = EvaluationModel.getScoreData($filter,
                    vm.process.Evaluators[0].StatusId,
                    vm.epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted,
                    vm.epaInfo.ScoreRatingType.Evaluator);

                vm.ReviewersScore = EvaluationModel.getMultiScoreDatas($filter,
                    vm.process.Reviewers,
                    vm.epaInfo.ScoreRatingType.Reviewer);

                vm.ManagementsScore = EvaluationModel.getMultiScoreDatas($filter,
                    vm.process.Managements,
                    vm.epaInfo.ScoreRatingType.Management);

                if (vm.epaInfo.FinalScore === "N/A") {
                    vm.epaInfo.FinalScore = "";
                }
            },
            controllerAs: 'performanceResultCtrl',
            bindToController: true,
            templateUrl: '/SourceApp/Components/EPA/Share/Directives/Templates/semiPerformanceResult.tmpl.html'
        };
    }
})(angular);