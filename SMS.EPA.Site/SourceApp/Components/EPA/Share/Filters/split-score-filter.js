(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').filter('splitScore', splitScore);
    splitScore.$inject = ['$filter', 'utilitiesFactory'];

    function splitScore($filter, utilitiesFactory) {
        return function (input) {
            var _number;

            if (input === null) {
                _number = "";
            } else if (input === 0) {
                _number = 0;
            } else {
                _number = $filter('number')(utilitiesFactory.splitNumber(parseFloat(input), 2));
            }

            return _number;
        };
    }
})(angular);