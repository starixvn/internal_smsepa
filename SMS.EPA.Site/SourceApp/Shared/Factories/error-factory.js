(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').factory('errorFactory', errorFactory);
    errorFactory.$inject = ['$location', '$window', 'notificationFactory', 'baseUrl'];

    function errorFactory($location, $window, notificationFactory, baseUrl) {
        return {
            errorHandle: function (status, internalPageRedirect) {
                switch (status) {
                    case 401:
                        $window.location.href = baseUrl + 'Auth/Login';
                        break;

                    case 404:
                        $location.path(baseUrl + 'Home/PageNotFound');
                        break;

                    case 403:
                    case 405:
                        $location.path(baseUrl + 'Home/AccessDenied');
                        break;

                    default:
                        notificationFactory.error('ErrorMessage');

                        if (internalPageRedirect) {
                            $location.path(baseUrl + 'Home/InternalError');
                        }
                        break;
                }
            }
        };
    }
})(angular);