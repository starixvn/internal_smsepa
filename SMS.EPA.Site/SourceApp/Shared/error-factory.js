/*global appModule*/
'use strict';

appModule.factory("errorFactory",
['$location', '$window', 'notificationFactory', 'baseUrl', 
function ($location, $window, notificationFactory, baseUrl) {
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
} ]);