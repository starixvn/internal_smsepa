(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('reportModule').directive('exportCommentSummaryReport', exportCommentSummaryReport);
    exportCommentSummaryReport.$inject = ['baseUrl'];

    function exportCommentSummaryReport(baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportCommentSummaryReport', function () {
                    var data = $.param(scope.epaDetailReportCtrl.exportCommentCriteria),
                        url = baseUrl + 'ExportCommentReport/ExportExcel',
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