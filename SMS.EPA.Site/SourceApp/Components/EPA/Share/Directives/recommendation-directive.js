(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').directive('epaRecommendation', recommendation);
    recommendation.$inject = ['utilitiesFactory'];

    function recommendation(utilitiesFactory) {
        return {
            restrict: 'E',
            scope: {
                epaInfo: '=',
                jobGrades: '='
            },
            controller: function () {
                var vm = this;
                if (vm.epaInfo) {
                    vm.evaluatorRecommendation = vm.epaInfo.Recommendations.filter(item => item.ProcessId == utilitiesFactory.processType.Evaluator)[0];
                    vm.recommendation = vm.epaInfo.Recommendations.filter(item => item.ProcessId == utilitiesFactory.processType.Reviewer)[0];
                }
            },
            controllerAs: 'recommendationCtrl',
            bindToController: true,
            templateUrl: '/SourceApp/Components/EPA/Share/Directives/Templates/recommendation.tmpl.html'
        };
    }
})(angular);