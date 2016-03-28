'use strict';
/*global periodModule*/

periodModule.factory('monthlyPeriodViewStateService', ['$rootScope', function ($rootScope) {

    var service = {

        model: {
            fromDate: new Date(new Date().getFullYear(), 0, 1),
            toDate: new Date()
        },

        SaveState: function () {
            sessionStorage.monthlyPeriodViewStateService = angular.toJson(service.model);
        },

        RestoreState: function () {
            service.model = angular.fromJson(sessionStorage.monthlyPeriodViewStateService);
        }
    }

    $rootScope.$on("savestate", service.SaveState);
    $rootScope.$on("restorestate", service.RestoreState);

    return service;
}]);