(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').directive('epaNumberDot', numberDot);
    numberDot.$inject = [];

    function numberDot() {
        return {
            require: 'ngModel',
            link: function (scope, element, attr, ctrl) {
                ctrl.$parsers.push(function (inputValue) {
                    if (inputValue) {
                        var digits = inputValue.replace(/[^\d+\.?\d+$]/, '');

                        digits = digits.replace(/^[\s+0]+/, "");

                        if (digits !== inputValue) {
                            ctrl.$setViewValue(digits);
                            ctrl.$render();
                        }

                        return digits;
                    }

                    return '';
                });
            }
        };
    }
})(angular);