'use strict';
/*global epaModule*/

epaModule.directive('ngDecimal', [function () {
    return {
        require: 'ngModel',
        scope: {
            fraction: "@"
        },
        link: function (scope, element, attr, ctrl) {
            var fraction = parseInt(scope.fraction) || 2;

            ctrl.$parsers.push(function (inputValue) {
                if (inputValue) {
                    var digits = inputValue.replace(/[^\d+\.?\d+$]/, '');

                    if (digits && ((parseFloat(digits) * Math.pow(10, fraction)) % 1 !== 0)) {
                        digits = parseFloat(digits).toFixed(fraction).toString();
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

epaModule.filter('customScore', ['$filter', function ($filter) {
    return function (input) {
        var _number;

        if (input === null) {
            _number = "";
        }
        else if (input === 0) {
            _number = 0;
        }
        else if (parseFloat(input) % 1 === 0) {
            _number = $filter('number')(parseFloat(input));
        }
        else if (parseFloat(input) % 1 !== 0) {
            _number = $filter('number')(parseFloat(input), 2);
        }

        return _number;
    };
}]);

epaModule.directive('popoverAvatar', ['baseUrl', function (baseUrl) {
    return {
        restrict: 'A',
        link: function (scope, el, attrs) {
            $(el).popover({
                trigger: 'hover',
                html: true,
                content: function () {
                    return '<img class="avatar" src="' + baseUrl + attrs.avatarSrc + '" />';
                },
                template: '<div class="popover popover-avatar" role="tooltip">' +
                                '<div class="arrow"></div>' +
                                '<h3 class="popover-title"></h3>' +
                                '<div class="popover-content"></div>' +
                            '</div>',
                placement: 'top'
            });
        }
    };
}]);

epaModule.directive('autoCollapse', ['$timeout', function ($timeout) {
    return {
        restrict: 'A',
        scope: {
            collapse: "="
        },
        link: function (scope, el, attrs) {
            var autoCollapse = attrs.autoCollapse;

            if (autoCollapse === 'true') {
                $timeout(function () {
                    $(el).collapse('hide');
                    scope.collapse = true;
                    attrs.$set('autoCollapse', false);
                }, 5000);
            }
        }
    };
}]);