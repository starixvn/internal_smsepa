(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('flowMngModule').factory('personalFlowFactory', personalFlowFactory);
    personalFlowFactory.$inject = ['$http', 'baseUrl'];

    function personalFlowFactory($http, baseUrl) {
        return {
            getPersonalFlow: function (flowTemplateId) {
                return $http.get(baseUrl + 'api/PersonalFlow/GetByFlowTemplateId', { params: { flowTemplateId } });
            },
            getPersonalFlowById: function (employeeId, flowTemplateId) {
                return $http.get(baseUrl + 'api/PersonalFlow/GetById', { params: { employeeId, flowTemplateId } });
            },
            createPersonalFlow: function (model) {
                return $http.post(baseUrl + 'api/PersonalFlow/CreatePersonalFlow', model);
            },
            deletePersonalFlow: function (employeeId, flowTemplateId) {
                return $http['delete'](baseUrl + 'api/PersonalFlow/DeletePersonalFlow', { params: { employeeId, flowTemplateId } });
            },
            getEmployeeFlowSetting: function (flowTemplateId, teamIds, isMonthly) {
                return $http.get(baseUrl + 'api/PersonalFlow/GetEmployeeFlowSetting', { params: { flowTemplateId, teamIds, isMonthly } });
            }
        };
    }
})(angular);