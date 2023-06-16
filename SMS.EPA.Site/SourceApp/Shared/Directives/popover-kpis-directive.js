(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').directive('popoverKpis', directive);

    function directive() {
        return {
            restrict: 'A',
            link: function (scope, el, attrs) {
                let KPIs = JSON.parse(attrs.kpisData);
                let content = '<h5 class="tooltip-kpi tooltip-kpi-padding">KPI in template:</h5><div class="tooltip-kpi-padding">'
                KPIs.forEach(kpi => {
                    content += `<div>- ${kpi.KPIName} (${kpi.TemplateWeight.toFixed(2)})</div>`
                });
                content += '</div>'
                $(el).popover({
                    trigger: 'hover',
                    html: true,
                    content: function () {
                        return content;
                    },
                    template: '<div class="popover popover-avatar" role="tooltip">' +
                        '<div class="arrow"></div>' +
                        '<div class="popover-content"></div>' +
                        '</div>',
                    placement: 'right'
                });
            }
        };
    }
})(angular);