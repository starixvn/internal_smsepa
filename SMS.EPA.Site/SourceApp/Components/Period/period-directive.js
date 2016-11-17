(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('periodModule').directive('updatedChosenPeriod', updatedChosenPeriod);
    updatedChosenPeriod.$inject = [];

    angular.module('periodModule').directive('exportEmployeeFlowSettingReport', exportEmployeeFlowSettingReport);
    exportEmployeeFlowSettingReport.$inject = ['baseUrl'];

    function updatedChosenPeriod() {
        return {
            restrict: 'A',
            link: function (scope) {
                scope.$on('updatedChosen', function () {
                    var humanResourceElement = $('[name="humanResourceSelect"]');
                    $(humanResourceElement).trigger("chosen:updated");

                    var evaluatorElement = $('[name="evaluatorSelect"]');
                    $(evaluatorElement).trigger("chosen:updated");

                    var reviewerElement = $('[name="reviewerSelect"]');
                    $(reviewerElement).trigger("chosen:updated");

                    var managementElement = $('[name="managementSelect"]');
                    $(managementElement).trigger("chosen:updated");
                });
            }
        };
    }

    function exportEmployeeFlowSettingReport(baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportEmployeeFlowSettingReport', function () {
                    var data = $.param(scope.empSettingCtrl.exportCriteria),
                        url = baseUrl + 'ExportReport/ExportEmployeeFlowSettingReport',
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