(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').directive('floatThead', floatThead);
    floatThead.$inject = [];

    function floatThead() {
        return {
            restrict: 'A',
            link: function (scope, elem) {
                window.onscroll = function () {
                    elem.floatThead();
                    elem.floatThead('reflow');
                }
            }
        };
    }
})(angular);