(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').directive('epaPerformanceGrade', performanceGrade);
    performanceGrade.$inject = ['$filter'];

    function performanceGrade($filter) {
        return {
            restrict: 'E',
            scope: {
                standardGrades: '='
            },
            controller: function () {
                var vm = this;

                if (vm.standardGrades && vm.standardGrades.length > 0) {
                    vm.disciplineGrades = $filter("filter")(vm.standardGrades, { Type: 1 });
                    vm.performanceGrades = $filter("filter")(vm.standardGrades, { Type: 2 });
                    vm.finalGrades = $filter("filter")(vm.standardGrades, { Type: 3 });
                }
            },
            controllerAs: 'performanceGradeCtrl',
            bindToController: true,
            templateUrl: '/SourceApp/Components/EPA/Share/Directives/Templates/performanceGrade.tmpl.html'
        };
    }
})(angular);