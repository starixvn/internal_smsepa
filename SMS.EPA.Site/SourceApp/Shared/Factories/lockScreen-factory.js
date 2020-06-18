(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').factory('lockScreenService', lockScreenService);
    lockScreenService.$inject = [];

    function lockScreenService() {
        var service = {
            start: function () {
                $('#loadingProcess').fadeIn();
            },
            stop: function () {
                $('#loadingProcess').fadeOut(300);
            }
        };

        return service;
    }
})(angular);