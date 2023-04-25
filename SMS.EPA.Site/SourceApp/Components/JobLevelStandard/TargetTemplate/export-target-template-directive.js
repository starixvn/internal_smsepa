(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('targetTemplateModule').directive('exportTargetTemplateManagement', exportTargetTemplateManagement);
    exportTargetTemplateManagement.$inject = ['baseUrl'];

    function exportTargetTemplateManagement(baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportTargetTemplateManagement', function () {
                    var data = $.param(scope.targetTemplateCtrl.exportCriteria),
                        url = baseUrl + 'ExportTargetTemplate/ExportTargetTemplateKPIs',
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