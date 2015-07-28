'use strict';
/*global checkFlowAssignmentModule*/

checkFlowAssignmentModule.directive('evaluatorChosen', function () {
    return {
        restrict: 'A',
        link: function (scope) {
            scope.$on('evaluatorChosen', function () {
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

checkFlowAssignmentModule.directive('exportCheckAssignmentReport', ['baseUrl', function (baseUrl) {
    return {
        link: function (scope) {
            scope.$on('exportCheckAssignmentReport', function () {
                var data = $.param(scope.checkFlowCtrl.exportCriteria),
                    url = baseUrl + 'ExportReport/ExportCheckAssignmentReport',
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
