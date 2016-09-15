(function (angular) {
    'use strict';

    angular.module('kpiModule').directive('updateTrigger', updateTrigger);

    function updateTrigger() {
        return {
            link: function (scope, iElement, attrs) {
                scope.$watch(attrs.ngModel, function () {
                    iElement.trigger('chosen:updated');
                });
            }
        };
    }
})(angular);