(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('monthlyPeriodModule').directive('updatedChosenMonthlyPeriod', updatedChosenMonthlyPeriod);
    updatedChosenMonthlyPeriod.$inject = [];

    function updatedChosenMonthlyPeriod() {
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
                });
            }
        };
    }
})(angular);