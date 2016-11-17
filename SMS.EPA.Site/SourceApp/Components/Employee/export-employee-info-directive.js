(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('employeeModule').directive('exportEmployeeInfoReport', exportEmployeeInfoReport);
    exportEmployeeInfoReport.$inject = ['baseUrl'];

    function exportEmployeeInfoReport(baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportEmployeeInfoReport', function () {
                    var data = $.param(scope.employeeCtrl.exportCriteria),
                        url = baseUrl + 'ExportReport/ExportEmployeeInfoReport',
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