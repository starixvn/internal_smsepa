(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').directive('repeatEnd', repeatEnd);
    repeatEnd.$inject = [];

    function repeatEnd() {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                if (scope.$last) {
                    scope.$eval(attrs.repeatEnd);
                }
            }
        };
    }
})(angular);