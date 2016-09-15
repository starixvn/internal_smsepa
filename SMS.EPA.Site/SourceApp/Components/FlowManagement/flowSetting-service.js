'use strict';
/*global flowMngModule*/

flowMngModule.service('cacheService', function () {
    var property;

    return {
        getProperty: function () {
            return property;
        },
        setProperty: function (value) {
            property = value;
        }
    };
});