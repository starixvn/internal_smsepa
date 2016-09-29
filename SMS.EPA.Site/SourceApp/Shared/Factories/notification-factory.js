(function (angular) {
    'use strict';
    /* global toastr */
    /* jshint latedef:nofunc */

    angular.module('appModule').factory('notificationFactory', notificationFactory);
    notificationFactory.$inject = ['$translate'];

    function notificationFactory($translate) {
        var normalOptions = {
            timeout: 4000
        },
        errorOptions = {
            timeout: 5000,
            escapeHtml: true
        };

        return {
            success: function (keyMessage, message) {
                if (message === '' || message === null || message === undefined) {
                    $translate(keyMessage).then(function (responseMessage) {
                        toastr.success(responseMessage, normalOptions);
                    });
                } else {
                    toastr.success(message, errorOptions);
                }
            },
            error: function (keyMessage, message) {
                if (message === '' || message === null || message === undefined) {
                    $translate(keyMessage).then(function (responseMessage) {
                        toastr.error(responseMessage, errorOptions);
                    });
                } else {
                    toastr.error(message, errorOptions);
                }
            },
            warning: function (keyMessage, message) {
                if (message === '' || message === null || message === undefined) {
                    $translate(keyMessage).then(function (responseMessage) {
                        toastr.warning(responseMessage, errorOptions);
                    });
                } else {
                    toastr.warning(message, errorOptions);
                }
            },
            info: function (keyMessage, message) {
                if (message === '' || message === null || message === undefined) {
                    $translate(keyMessage).then(function (responseMessage) {
                        toastr.info(responseMessage, normalOptions);
                    });
                } else {
                    toastr.info(message, normalOptions);
                }
            }
        };
    }
})(angular);