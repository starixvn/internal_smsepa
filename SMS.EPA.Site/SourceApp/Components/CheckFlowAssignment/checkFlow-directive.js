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
