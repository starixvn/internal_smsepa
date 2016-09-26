(function (angular) {
    'use strict';

    angular.module('reportModule').directive('exportEpaSummaryReport', exportEpaSummaryReport);
    exportEpaSummaryReport.$inject = ['baseUrl'];

    function exportEpaSummaryReport(baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportEpaSummaryReport', function () {
                    var data = $.param(scope.epaSummaryReportCtrl.exportCriteria),
                        url = baseUrl + 'ExportReport/ExportEPASummaryReport',
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