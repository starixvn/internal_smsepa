(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('disciplineModule').directive('exportDisciplineSummaryByPeriodReport', exportDisciplineSummaryByPeriodReport);
    exportDisciplineSummaryByPeriodReport.$inject = ['baseUrl'];

    function exportDisciplineSummaryByPeriodReport(baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportDisciplineSummaryByPeriodReport', function () {
                    var data = $.param(scope.disciplineSummaryByPeriodCtrl.exportCriteria),
                        url = baseUrl + 'ExportReport/ExportDisciplineSummaryByPeriodReport',
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