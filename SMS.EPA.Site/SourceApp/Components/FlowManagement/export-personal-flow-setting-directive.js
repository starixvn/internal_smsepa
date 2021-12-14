(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('flowMngModule').directive('exportPersonalFlowSetting', exportPersonalFlowSetting);
    exportPersonalFlowSetting.$inject = ['baseUrl'];

    function exportPersonalFlowSetting(baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportPersonalFlowSetting', function () {
                    var data = $.param(scope.resultFlowSettingCtrl.exportPersonalFlowCriteria),
                        url = baseUrl + 'exportReport/exportPersonalFlowSetting',
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