'use strict';
/*global monthlyPeriodModule*/

monthlyPeriodModule.directive('evaluatorChosen', function () {
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
            });
        }
    };
});