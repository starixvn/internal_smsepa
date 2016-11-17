(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').directive('epaPerformanceGrade', performanceGrade);
    performanceGrade.$inject = [];

    function performanceGrade() {
        return {
            restrict: 'E',
            scope: {
                standardGrades: '='
            },
            controller: function () {
            },
            controllerAs: 'performanceGradeCtrl',
            bindToController: true,
            templateUrl: '/SourceApp/Components/EPA/Share/Directives/Templates/performanceGrade.tmpl.html'
        };
    }
})(angular);