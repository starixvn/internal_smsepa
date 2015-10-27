'use strict';
/*global errorModule*/

errorModule.controller('ErrorController', ['$window', function ($window) {
    $window.location.reload();
}]);