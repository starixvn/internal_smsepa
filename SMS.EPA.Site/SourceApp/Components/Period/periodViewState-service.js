'use strict';
/*global periodModule*/

periodModule.factory('periodViewStateService', ['$rootScope', function ($rootScope) {

    var service = {

        model: {
            targetTypeId: null,
            isActive: 'true'
        },

        SaveState: function () {
            sessionStorage.periodViewStateService = angular.toJson(service.model);
        },

        RestoreState: function () {
            service.model = angular.fromJson(sessionStorage.periodViewStateService);
        }
    }

    $rootScope.$on("savestate", service.SaveState);
    $rootScope.$on("restorestate", service.RestoreState);

    return service;
}]);