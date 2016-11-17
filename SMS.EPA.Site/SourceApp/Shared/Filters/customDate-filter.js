(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').filter('customDate', customDateFilter);
    customDateFilter.$inject = ['$filter'];

    function customDateFilter($filter) {
        return function (input) {
            if (input === null) {
                return '';
            }

            var _date = $filter('date')(new Date(input), 'dd MMM, yyyy');
            return _date;
        };
    }
})(angular);