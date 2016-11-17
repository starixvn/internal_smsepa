(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').filter('customNumber', customNumberFilter);
    customNumberFilter.$inject = ['$filter'];

    function customNumberFilter($filter) {
        return function (input) {
            if (input == null || input === 0) {
                return '';
            }

            var _number = $filter('number')(input, 2);
            return _number;
        };
    }
})(angular);