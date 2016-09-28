(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').factory('errorFactory', errorFactory);
    errorFactory.$inject = ['$location', '$window', 'baseUrl'];

    function errorFactory($location, $window, baseUrl) {
        return {
            errorHandle: function (status) {
                if (status === 405) {
                    $location.path(baseUrl + 'Home/AccessDenied');
                }
                else if (status === 401) {
                    $window.location.href = baseUrl + "Auth/Login";
                }
                else {
                    $location.path(baseUrl + 'Home/InternalError');
                }
            }
        };
    }
})(angular);