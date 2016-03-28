'use strict';
/*global epaModule*/

epaModule.factory('managementViewStateService', ['$rootScope', function ($rootScope) {

    var service = {

        model: {
            groupId: '',
            isLocal: '',
            employeeIds: ''
        },

        SaveState: function () {
            sessionStorage.managementViewStateService = angular.toJson(service.model);
        },

        RestoreState: function () {
            service.model = angular.fromJson(sessionStorage.managementViewStateService);
        }
    }

    $rootScope.$on("savestate", service.SaveState);
    $rootScope.$on("restorestate", service.RestoreState);

    return service;
}]);