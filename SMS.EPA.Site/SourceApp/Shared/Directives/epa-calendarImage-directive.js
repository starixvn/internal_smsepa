(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').directive('epaCalendarImage', calendarImage);
    calendarImage.$inject = [];

    function calendarImage() {
        return {
            restrict: 'AE',
            replace: 'true',
            template: '<img class="calendar-image" src="/Content/Site/images/calendar.png"></img>'
        };
    }
})(angular);