(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('disciplineModule').directive('exportDisciplineSummaryReport', exportDisciplineSummaryReport);
    exportDisciplineSummaryReport.$inject = ['baseUrl'];

    function exportDisciplineSummaryReport(baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportDisciplineSummaryReport', function () {
                    var data = $.param(scope.disciplineCtrl.exportCriteria),
                        url = baseUrl + 'ExportReport/ExportDisciplineSummaryReport',
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