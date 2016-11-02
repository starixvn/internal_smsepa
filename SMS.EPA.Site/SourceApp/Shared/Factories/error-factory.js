(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').factory('errorFactory', errorFactory);
    errorFactory.$inject = ['$location', '$window', 'notificationFactory', 'baseUrl'];

    function errorFactory($location, $window, notificationFactory, baseUrl) {
        return {
            errorHandle: function (status, internalPageRedirect) {
                if (status === 401) {
                    $window.location.href = baseUrl + "Auth/Login";
                } else if (status === 404) {
                    $location.path(baseUrl + 'Home/PageNotFound');
                } else if (status === 405) {
                    $location.path(baseUrl + 'Home/AccessDenied');
                } else {
                    notificationFactory.error("ErrorMessage");

                    if (internalPageRedirect) {
                        $location.path(baseUrl + 'Home/InternalError');
                    }
                }
            }
        };
    }
})(angular);