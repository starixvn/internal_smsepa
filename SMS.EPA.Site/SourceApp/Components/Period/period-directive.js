'use strict';
/*global periodModule*/

periodModule.directive('evaluatorChosen', function () {
    return {
        restrict: 'A',
        link: function (scope) {
            scope.$on('evaluatorChosen', function () {
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
});

periodModule.directive('exportEmployeeSettingReport', ['baseUrl', function (baseUrl) {
    return {
        link: function (scope) {
            scope.$on('exportEmployeeSettingReport', function () {
                var data = $.param(scope.employeeSettingPeriodCtrl.exportCriteria),
                    url = baseUrl + 'ExportReport/ExportEmployeeSettingReport',
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
}]);