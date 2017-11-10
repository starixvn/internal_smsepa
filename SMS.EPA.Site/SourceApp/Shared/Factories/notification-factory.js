(function (angular) {
    'use strict';
    /* global toastr */
    /* jshint latedef:nofunc */

    angular.module('appModule').factory('notificationFactory', notificationFactory);
    notificationFactory.$inject = ['$translate'];

    function notificationFactory($translate) {
        var normalOptions = {
            timeOut: 4000
        },
        errorOptions = {
            timeOut: 7000,
            extendedTimeOut: 500,
            escapeHtml: false
        };

        return {
            success: function (keyMessage, message) {
                if (message === '' || message === null || message === undefined) {
                    $translate(keyMessage).then(function (responseMessage) {
                        toastr.success(responseMessage, null, normalOptions);
                    });
                } else {
                    toastr.success(message, null, errorOptions);
                }
            },
            error: function (keyMessage, message) {
                if (message === '' || message === null || message === undefined) {
                    $translate(keyMessage).then(function (responseMessage) {
                        toastr.error(responseMessage, null, errorOptions);
                    });
                } else {
                    toastr.error(message, null, errorOptions);
                }
            },
            warning: function (keyMessage, message) {
                if (message === '' || message === null || message === undefined) {
                    $translate(keyMessage).then(function (responseMessage) {
                        toastr.warning(responseMessage, null, errorOptions);
                    });
                } else {
                    toastr.warning(message, null, errorOptions);
                }
            },
            info: function (keyMessage, message) {
                if (message === '' || message === null || message === undefined) {
                    $translate(keyMessage).then(function (responseMessage) {
                        toastr.info(responseMessage, null, normalOptions);
                    });
                } else {
                    toastr.info(message, null, normalOptions);
                }
            }
        };
    }
})(angular);