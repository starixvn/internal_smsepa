(function (angular) {
    'use strict';

    angular.module('reportModule').directive('exportMonthlyEpaDetailReport', exportMonthlyEpaDetailReport);
    exportMonthlyEpaDetailReport.$inject = ['baseUrl'];

    function exportMonthlyEpaDetailReport(baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportMonthlyEpaDetailReport', function () {
                    var data = $.param(scope.monthlyDetailReportCtrl.exportCriteria),
                        url = baseUrl + 'ExportReport/ExportMonthlyEPADetailReport',
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