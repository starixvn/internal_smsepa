(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').directive('epaRecommendation', recommendation);
    recommendation.$inject = [];

    function recommendation() {
        return {
            restrict: 'E',
            scope: {
                epaInfo: '=',
                jobGrades: '='
            },
            controller: function () {
            },
            controllerAs: 'recommendationCtrl',
            bindToController: true,
            templateUrl: '/SourceApp/Components/EPA/Share/Directives/Templates/recommendation.tmpl.html'
        };
    }
})(angular);