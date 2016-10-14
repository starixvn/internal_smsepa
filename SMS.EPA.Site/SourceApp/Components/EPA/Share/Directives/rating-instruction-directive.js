(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').directive('epaRatingInstruction', ratingInstruction);
    ratingInstruction.$inject = [];

    function ratingInstruction() {
        return {
            restrict: 'E',
            templateUrl: '/SourceApp/Components/EPA/Share/Directives/Templates/ratingInstruction.tmpl.html'
        };
    }
})(angular);