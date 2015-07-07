/*global appModule*/
'use strict';

appModule.factory("errorFactory",
['$location', 'notificationFactory',
function ($location, notificationFactory) {
    return {
        errorHandle: function (status) {
            if (status === 405) {
                $location.path('Home/AccessDenied');
            }
            else {
                notificationFactory.error("ErrorMessage");
            }
        }
    };
} ]);