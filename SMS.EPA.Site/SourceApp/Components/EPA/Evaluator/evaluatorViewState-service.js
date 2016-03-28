'use strict';
/*global epaModule*/

epaModule.factory('evaluatorViewStateService', ['$rootScope', function ($rootScope) {

    var service = {

        model: {
            groupId: '',
            isLocal: '',
            isMonthly: false,
            employeeIds: ''
        },

        SaveState: function () {
            sessionStorage.evaluatorViewStateService = angular.toJson(service.model);
        },

        RestoreState: function () {
            service.model = angular.fromJson(sessionStorage.evaluatorViewStateService);
        }
    }

    $rootScope.$on("savestate", service.SaveState);
    $rootScope.$on("restorestate", service.RestoreState);

    return service;
}]);