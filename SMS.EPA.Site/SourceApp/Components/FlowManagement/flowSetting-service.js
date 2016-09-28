(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('flowMngModule').service('cacheService', cacheService);
    cacheService.$inject = [];

    function cacheService() {
        var property;

        return {
            getProperty: function () {
                return property;
            },
            setProperty: function (value) {
                property = value;
            }
        };
    }
})(angular);