(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').directive('epaSemiPerformanceResult', performanceResult);
    performanceResult.$inject = ["$filter", "utilitiesFactory"];

    function performanceResult($filter, utilitiesFactory) {
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

                vm.SelfScore = getScoreData($filter,
                    utilitiesFactory,
                    vm.process.Self.StatusId,
                    vm.epaInfo.MonthlyEPA.AverageSelfScoreConverted,
                    vm.epaInfo.ScoreRatingType.SelfEvaluated);

                vm.EvaluatorScore = getScoreData($filter,
                    utilitiesFactory,
                    vm.process.Evaluators[0].StatusId,
                    vm.epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted,
                    vm.epaInfo.ScoreRatingType.Evaluator);

                vm.ReviewersScore = getMultiScoreDatas($filter,
                    utilitiesFactory,
                    vm.process.Reviewers,
                    vm.epaInfo.ScoreRatingType.Reviewer);

                vm.ManagementsScore = getMultiScoreDatas($filter,
                    utilitiesFactory,
                    vm.process.Managements,
                    vm.epaInfo.ScoreRatingType.Management);
            },
            controllerAs: 'performanceResultCtrl',
            bindToController: true,
            templateUrl: '/SourceApp/Components/EPA/Share/Directives/Templates/semiPerformanceResult.tmpl.html'
        };
    }

    function getMultiScoreDatas($filter, utilitiesFactory, actors, maxScore) {
        var data = [];
        var index;
        for (index = 0; index < actors.length; index++) {
            var actorScore = getScoreData($filter,
                utilitiesFactory,
                actors[index].StatusId,
                actors[index].TotalScore,
                maxScore);
            data.push(actorScore);
        }

        return data;
    }

    function getScoreData($filter, utilitiesFactory, statudId, score, maxScore) {
        return {
            IsShowed: isScoreShowed(statudId, utilitiesFactory),
            Class: getScoreClass(statudId, utilitiesFactory),
            Score: getScoreValue($filter, score, maxScore)
        }
    }

    function isScoreShowed(statusId, utilitiesFactory)
    {
        if (statusId !== utilitiesFactory.processStatus.Open) {
            return true;
        } else {
            return false;
        }
    }

    function getScoreClass(statusId, utilitiesFactory) {
        if (statusId === utilitiesFactory.processStatus.InProgress) {
            return "red";
        }
        return "";
    }

    function getScoreValue($filter, score, maxScore) {
        if (score === "" || score === null) {
            return "N/A";
        }

        return $filter("customScore")(score) + " / " + $filter("customScore")(maxScore);
    }
})(angular);