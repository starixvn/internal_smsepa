'use strict';
/*global appModule*/

appModule.directive('calendarImage', function () {
    return {
        restrict: 'AE',
        replace: 'true',
        template: '<img class="calendar-image" src="/Content/Site/images/calendar.png"></img>'
    };
});

appModule.directive('percentage', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            modelCtrl.$parsers.push(function (inputValue) {
                if (inputValue === undefined) {
                    return '';
                }

                var transformedInput = inputValue.replace(/[^0-9]/g, '');
                if (transformedInput < 0 || transformedInput > 100) {
                    modelCtrl.$setViewValue('');
                    modelCtrl.$render();
                }
                else {
                    if (transformedInput !== inputValue) {
                        modelCtrl.$setViewValue(transformedInput);
                        modelCtrl.$render();
                    }
                }

                return transformedInput;
            });
        }
    };
});

appModule.directive('numberDot', [function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attr, ctrl) {
            ctrl.$parsers.push(function (inputValue) {
                if (inputValue) {
                    var digits = inputValue.replace(/[^\d+\.?\d+$]/, '');

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