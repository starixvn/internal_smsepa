'use strict';
/*global kpiGroupModule*/

kpiGroupModule.factory('kpiGroupViewStateService', ['$rootScope', function ($rootScope) {

    var service = {

        model: {
            isActive: 'true'
        },

        SaveState: function () {
            sessionStorage.kpiGroupViewStateService = angular.toJson(service.model);
        },

        RestoreState: function () {
            service.model = angular.fromJson(sessionStorage.kpiGroupViewStateService);
        }
    }

    $rootScope.$on("savestate", service.SaveState);
    $rootScope.$on("restorestate", service.RestoreState);

    return service;
}]);