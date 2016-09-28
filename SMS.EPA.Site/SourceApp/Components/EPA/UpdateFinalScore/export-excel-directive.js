(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').directive('exportEpaAfterUpdateFinalScore', exportEpaAfterUpdateFinalScore);
    exportEpaAfterUpdateFinalScore.$inject = ['baseUrl'];

    function exportEpaAfterUpdateFinalScore(baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportEpaAfterUpdateFinalScore', function () {
                    var data = $.param(scope.updateFinalScoreCtrl.exportCriteria),
                        url = baseUrl + 'ExportReport/ExportEPAAfterUpdateFinalScore',
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