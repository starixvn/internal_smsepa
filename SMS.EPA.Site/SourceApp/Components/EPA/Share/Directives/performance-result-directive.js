(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').directive('epaPerformanceResult', performanceResult);
    performanceResult.$inject = ["$filter", "EvaluationModel"];

    function performanceResult($filter, EvaluationModel) {
        return {
            restrict: 'E',
            scope: {
                epaInfo: '='
            },
            controller: function () {
                var vm = this;

                vm.range = function (n) {
                    return new Array(n);
                };

                if (vm.epaInfo) {
                    vm.process = vm.epaInfo ? angular.copy(vm.epaInfo.Process) : null;

                    debugger;

                    vm.SelfScore = EvaluationModel.getScoreData($filter,
                        vm.process.Self.StatusId,
                        vm.process.Self.TotalScore,
                        vm.epaInfo.ScoreRatingType.SelfEvaluated);

                    vm.EvaluatorsScore = EvaluationModel.getMultiScoreDatas($filter,
                        vm.process.Evaluators,
                        vm.epaInfo.ScoreRatingType.Evaluator);

                    vm.ReviewersScore = EvaluationModel.getMultiScoreDatas($filter,
                        vm.process.Reviewers,
                        vm.epaInfo.ScoreRatingType.Reviewer);

                    vm.ManagementsScore = EvaluationModel.getMultiScoreDatas($filter,
                        vm.process.Managements,
                        vm.epaInfo.ScoreRatingType.Management);
                }
            },
            controllerAs: 'performanceResultCtrl',
            bindToController: true,
            templateUrl: '/SourceApp/Components/EPA/Share/Directives/Templates/performanceResult.tmpl.html'
        };
    }
})(angular);