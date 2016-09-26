(function (angular) {
    'use strict';

    angular.module('attendanceModule').directive('exportAttendanceSummaryReport', exportAttendanceSummaryReport);
    exportAttendanceSummaryReport.$inject = ['baseUrl'];

    function exportAttendanceSummaryReport(baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportAttendanceSummaryReport', function () {
                    var data = $.param(scope.attendanceCtrl.exportCriteria),
                        url = baseUrl + 'ExportReport/ExportAttendanceSummaryReport',
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