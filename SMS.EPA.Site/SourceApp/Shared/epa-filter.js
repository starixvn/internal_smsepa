'use strict';
/*global appModule*/

appModule.filter('customDate', ['$filter', function ($filter) {
    return function (input) {
        if (input == null) { return ""; }
        var _date = $filter('date')(new Date(input), 'dd MMM, yyyy');
        return _date;
    };
}]);

appModule.filter('customNumber', ['$filter', function ($filter) {
    return function (input) {
        if (input == null || input === 0) { return ""; }
        var _number = $filter('number')(input, 2);
        return _number;
    };
}]);

appModule.filter('range', function () {
    return function (input, max) {
        max = parseInt(max);

        for (var i = 1; i < max; i++)
            input.push(i);

        return input;
    };
});