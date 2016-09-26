(function (angular) {
    'use strict';

    angular.module('attendanceModule').directive('exportAttendanceDetailReport', exportAttendanceDetailReport);
    exportAttendanceDetailReport.$inject = ['baseUrl'];

    function exportAttendanceDetailReport(baseUrl) {
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
    }
})(angular);