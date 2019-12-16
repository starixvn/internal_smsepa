(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('reportModule').directive('exportRecommendationSummaryReport', exportRecommendationSummaryReport);
    exportRecommendationSummaryReport.$inject = ['baseUrl'];

    function exportRecommendationSummaryReport(baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportRecommendationSummaryReport', function () {
                    var data = $.param(scope.epaDetailReportCtrl.exportRecommendationCriteria),
                        url = baseUrl + 'ExportRecommendationReport/ExportExcel',
                        downloadUrl = url + "?" + data,
                        iframe = $('body iframe');

                    if (iframe.length === 0) {
                        iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
                        iframe.appendTo('body');
                    }

                    iframe.attr('src', downloadUrl);
                });
            }
        };
    }
})(angular);