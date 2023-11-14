(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').directive('epaRecommendation', recommendation);
    recommendation.$inject = ['$window', 'utilitiesFactory'];

    function recommendation($window, utilitiesFactory) {
        return {
            restrict: 'E',
            scope: {
                epaInfo: '=',
                jobLevels: '='
            },
            controller: function () {
                let vm = this;

                vm.enableExtendProbation = false;

                if (vm.epaInfo) {
                    vm.evaluatorRecommendation = vm.epaInfo.Recommendations.filter(item => item.ProcessId == utilitiesFactory.processType.Evaluator)[0];
                    vm.recommendation = vm.epaInfo.Recommendations.filter(item => item.ProcessId == utilitiesFactory.processType.Reviewer)[0];

                    if ($window.jobLevelExtendProbations === 'All'
                        || $window.jobLevelExtendProbations.split(',').includes(vm.epaInfo.Employee.JobLevel)) {
                        vm.enableExtendProbation = true;
                    }
                }
            },
            controllerAs: 'recommendationCtrl',
            bindToController: true,
            templateUrl: '/SourceApp/Components/EPA/Share/Directives/Templates/recommendation.tmpl.html'
        };
    }
})(angular);