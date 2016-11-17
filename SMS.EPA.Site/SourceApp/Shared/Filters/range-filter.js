(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').filter('range', rangeFilter);
    rangeFilter.$inject = [];

    function rangeFilter() {
        return function (input, max) {
            max = parseInt(max);

            for (var i = 1; i < max; i++) {
                input.push(i);
            }

            return input;
        };
    }
})(angular);