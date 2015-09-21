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

epaModule.directive('scrollIf', [function () {
    return {
        link: function ($scope, iElement) {
            $scope.$watch(function () {
                var head = $(iElement).find('thead'),
                    body = $(iElement).find('tbody');
                var child = body.find('tr'),
                    len = child.length;

                if (len) {
                    var maxHeight = 0;

                    child.each(function (i, e) {
                        maxHeight += $(e).height();
                    });

                    if (maxHeight > body.height()) {
                        head.addClass('header-fixed');
                    }
                    else {
                        head.removeClass('header-fixed');
                    }
                }
            });
        }
    };
}]);

epaModule.directive('manageColumnMenu',
[function () {
    var generateFormForMenu = function (showColumns) {
        var hideColumnsForm = $('<form class="form-horizontal "></form>');
        var definedColumns = generateColumnHeaders();

        $(definedColumns).each(function (idx, column) {
            if (idx > 0) {
                var checkbox = '<input type="checkbox" checked data-index="' + idx + '" id="' + idx + '">' +
                               '<label class="checkbox-label" for="' + idx + '">' + column.HeaderText + '</label>';

                var index = $(showColumns).index(idx);
                if (index === -1) {
                    checkbox = checkbox.replace("checked", "");
                }

                hideColumnsForm.append('<div class="form-group">' +
                                            '<span class="checkbox">' +
                                                checkbox +
                                            '</span>' +
                                    '<div>');
            }
        });

        return hideColumnsForm;
    };

    return {
        link: function (scope, iElement) {
            
        }
    };
}]);