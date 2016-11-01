(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').filter('customScore', customScore);
    customScore.$inject = ['$filter'];

    function customScore($filter) {
        return function (input) {
            var _number;

            if (input === null) {
                _number = "";
            }
            else if (input === 0) {
                _number = 0;
            }
            else if (parseFloat(input) % 1 === 0) {
                _number = $filter('number')(parseFloat(input));
            }
            else if (parseFloat(input) % 1 !== 0) {
                _number = $filter('number')(parseFloat(input), 2);
            }

            return _number;
        };
    }
})(angular);