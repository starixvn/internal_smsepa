(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('flowMngModule').directive('exportDepartmentFlowSetting', exportDepartmentFlowSetting);
    exportDepartmentFlowSetting.$inject = ['baseUrl'];

    function exportDepartmentFlowSetting(baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportDepartmentFlowSetting', function () {
                    var data = $.param(scope.resultFlowSettingCtrl.exportDepartmentFlowCriteria),
                        url = baseUrl + 'exportReport/exportDepartmentFlowSetting',
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