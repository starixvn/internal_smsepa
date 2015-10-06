'use strict';
/*global attendanceModule*/

attendanceModule.directive('exportAttendanceSummaryReport', ['baseUrl', function (baseUrl) {
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
}]);

attendanceModule.directive('exportAttendanceDetailReport', ['baseUrl', function (baseUrl) {
    return {
        link: function (scope) {
            scope.$on('exportAttendanceDetailReport', function () {
                var data = $.param(scope.detailAttendanceCtrl.exportCriteria),
                    url = baseUrl + 'ExportReport/ExportAttendanceDetailReport',
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
}]);