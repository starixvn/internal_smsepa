'use strict';
/*global appModule*/

appModule.directive('datepickerPopup', function () {
    return {
        restrict: 'EAC',
        require: 'ngModel',
        link: function (scope, element, attr, controller) {
            //remove the default formatter from the input directive to prevent conflict
            controller.$formatters.shift();
        }
    };
});

appModule.directive('numberOnly', ['$filter', function ($filter) {
    return {
        require: 'ngModel',
        link: function (scope, element, attr, ctrl) {
            ctrl.$formatters.unshift(function () {
                var value = ctrl.$modelValue;

                if (value !== "") {
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
}]);