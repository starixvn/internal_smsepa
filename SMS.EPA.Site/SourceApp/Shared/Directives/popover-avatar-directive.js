(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').directive('popoverAvatar', popoverAvatar);
    popoverAvatar.$inject = ['baseUrl'];

    function popoverAvatar(baseUrl) {
        return {
            restrict: 'A',
            link: function (scope, el, attrs) {
                $(el).popover({
                    trigger: 'hover',
                    html: true,
                    content: function () {
                        return '<img class="avatar" src="' + baseUrl + attrs.avatarSrc + '" />';
                    },
                    template: '<div class="popover popover-avatar" role="tooltip">' +
                                    '<div class="arrow"></div>' +
                                    '<h3 class="popover-title"></h3>' +
                                    '<div class="popover-content"></div>' +
                                '</div>',
                    placement: 'top'
                });
            }
        };
    }
})(angular);