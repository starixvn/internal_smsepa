(function (angular) {
    'use strict';

    angular.module('reportModule').directive('exportEpaDetailReport', exportEpaDetailReport);
    exportEpaDetailReport.$inject = ['baseUrl'];

    function exportEpaDetailReport(baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportEpaDetailReport', function () {
                    var data = $.param(scope.epaDetailReportCtrl.exportCriteria),
                        url = baseUrl + 'ExportReport/ExportEPADetailReport',
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