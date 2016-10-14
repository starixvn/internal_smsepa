(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').directive('epaPerformanceResult', performanceResult);
    performanceResult.$inject = [];

    function performanceResult() {
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
            },
            controllerAs: 'performanceResultCtrl',
            bindToController: true,
            templateUrl: '/SourceApp/Components/EPA/Share/Directives/Templates/performanceResult.tmpl.html'
        };
    }
})(angular);