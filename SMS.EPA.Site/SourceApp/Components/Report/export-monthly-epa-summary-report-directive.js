(function (angular) {
    'use strict';

    angular.module('reportModule').directive('exportMonthlyEpaSummaryReport', exportMonthlyEpaSummaryReport);
    exportMonthlyEpaSummaryReport.$inject = ['baseUrl'];

    function exportMonthlyEpaSummaryReport(baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportMonthlyEpaSummaryReport', function () {
                    var data = $.param(scope.monthlySummaryReportCtrl.exportCriteria),
                        url = baseUrl + 'ExportReport/ExportMonthlyEPASummaryReport',
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