(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').factory('lockScreenService', lockScreenService);
    lockScreenService.$inject = [];

    function lockScreenService() {
        var service = {
            start: function () {
                $('.loading-process').fadeIn();
            },
            stop: function () {
                $('.loading-process').fadeOut(300);
            }
        };

        return service;
    }
})(angular);