(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').directive('popoverCommentKpis', directive);

    function directive() {
        return {
            restrict: 'A',
            link: function (scope, el, attrs) {
                let content = '<div><strong>Comment:</strong> ' + attrs.comment + '</div>'
                $(el).popover({
                    trigger: 'click hover',
                    html: true,
                    content: function () {
                        return content;
                    },
                    template: '<div class="popover popover-assessment-kpi" role="tooltip">' +
                        '<div class="arrow"></div>' +
                        '<div class="popover-content"></div>' +
                        '</div>',
                    placement: 'left'
                });
            }
        };
    }
})(angular);