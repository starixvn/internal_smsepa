(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').directive('popoverAssessmentKpis', directive);
    directive.$inject = ['utilitiesFactory'];

    function directive(utilitiesFactory) {
        return {
            restrict: 'A',
            link: function (scope, el, attrs) {
                let kpis = JSON.parse(attrs.kpis);
                let content = '<div class="popover-assessment-title"><strong>KPIs:</strong></div><div class="kpis">'
                kpis.forEach(kpi => {
                    content += `<div>- ${kpi.KPIName} (${utilitiesFactory.splitNumber(kpi.TemplateWeight, 2)})</div>`
                });
                content += '</div>'
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
                    placement: 'right'
                });
            }
        };
    }
})(angular);