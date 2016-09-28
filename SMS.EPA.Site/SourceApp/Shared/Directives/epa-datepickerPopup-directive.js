(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').directive('datepickerPopup', datepickerPopup);
    datepickerPopup.$inject = [];

    function datepickerPopup() {
        return {
            restrict: 'EAC',
            require: 'ngModel',
            link: function (scope, element, attr, controller) {
                //remove the default formatter from the input directive to prevent conflict
                controller.$formatters.shift();
            }
        };
    }
})(angular);