(function (angular) {
    'use strict';

    angular.module('disciplineModule').directive('exportDisciplineDetailReport', exportDisciplineDetailReport);
    exportDisciplineDetailReport.$inject = ['baseUrl'];

    function exportDisciplineDetailReport(baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportDisciplineDetailReport', function () {
                    var data = $.param(scope.detailAttendanceCtrl.exportCriteria),
                        url = baseUrl + 'ExportReport/ExportDisciplineDetailReport',
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