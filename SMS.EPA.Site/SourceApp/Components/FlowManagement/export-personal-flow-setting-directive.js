(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('periodModule').directive('exportPersonalFlowSettingReport', exportPersonalFlowSettingReport);
    exportPersonalFlowSettingReport.$inject = ['baseUrl'];

    function exportPersonalFlowSettingReport(baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportPersonalFlowSettingReport', function () {
                    var data = $.param(scope.resultFlowSettingCtrl.exportCriteria),
                        url = baseUrl + 'ExportReport/ExportPersonalFlowSettingReport',
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