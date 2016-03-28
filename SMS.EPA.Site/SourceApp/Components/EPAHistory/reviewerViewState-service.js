'use strict';
/*global epaHistoryModule*/

epaHistoryModule.factory('epaHistoryViewStateService', ['$rootScope', '$routeParams', function ($rootScope, $routeParams) {

    var service = {

        model: {
            periodId: $routeParams.periodId ? $routeParams.periodId : null,
            employee: null,
            employeeId: $routeParams.empId ? $routeParams.empId : null
        },

        SaveState: function () {
            sessionStorage.epaHistoryViewStateService = angular.toJson(service.model);
        },

        RestoreState: function () {
            service.model = angular.fromJson(sessionStorage.epaHistoryViewStateService);
        }
    }

    $rootScope.$on("savestate", service.SaveState);
    $rootScope.$on("restorestate", service.RestoreState);

    return service;
}]);