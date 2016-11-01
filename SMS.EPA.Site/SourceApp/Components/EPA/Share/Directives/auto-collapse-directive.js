(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').directive('autoCollapse', autoCollapse);
    autoCollapse.$inject = ['$timeout'];

    function autoCollapse($timeout) {
        return {
            restrict: 'A',
            scope: {
                collapsed: "="
            },
            link: function (scope, el, attrs) {
                if (attrs.collapsed === 'true') {
                    $timeout(function () {
                        $(el).collapse('hide');
                        scope.collapsed = true;
                        attrs.$set('collapsed', false);
                    }, 5000);
                }
            }
        };
    }
})(angular);