(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').directive('epaNumberOnly', numberOnly);
    numberOnly.$inject = ['$filter'];

    function numberOnly($filter) {
        return {
            require: 'ngModel',
            link: function (scope, element, attr, ctrl) {
                ctrl.$formatters.unshift(function () {
                    var value = ctrl.$modelValue;

                    if (value !== '') {
                        value = $filter(attr.numberOnly)(value);
                    }

                    return value;
                });

                ctrl.$parsers.push(function (inputValue) {
                    if (inputValue) {
                        var digits = inputValue.replace(/[^0-9]/g, '');

                        if (digits !== '') {
                            digits = $filter(attr.numberOnly)(digits);
                        }

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