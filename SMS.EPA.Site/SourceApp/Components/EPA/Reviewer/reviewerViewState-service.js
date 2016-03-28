'use strict';
/*global epaModule*/

epaModule.factory('reviewerViewStateService', ['$rootScope', function ($rootScope) {

    var service = {

        model: {
            groupId: '',
            isLocal: '',
            isMonthly: false,
            employeeIds: ''
        },

        SaveState: function () {
            sessionStorage.reviewerViewStateService = angular.toJson(service.model);
        },

        RestoreState: function () {
            service.model = angular.fromJson(sessionStorage.reviewerViewStateService);
        }
    }

    $rootScope.$on("savestate", service.SaveState);
    $rootScope.$on("restorestate", service.RestoreState);

    return service;
}]);