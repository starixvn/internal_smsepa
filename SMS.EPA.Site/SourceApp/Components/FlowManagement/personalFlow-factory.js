(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('flowMngModule').factory('personalFlowFactory', personalFlowFactory);
    personalFlowFactory.$inject = ['$http', 'baseUrl'];

    function personalFlowFactory($http, baseUrl) {
        return {
            getPersonalFlow: function (flowTemplateId) {
                return $http.get(baseUrl + 'api/PersonalFlow/GetPersonalFlowByFlowTemplateId', {
                    params: {
                        flowTemplateId: flowTemplateId
                    }
                });
            },
            getPersonalFlowById: function (employeeId, flowTemplateId) {
                return $http.get(baseUrl + 'api/PersonalFlow/GetPersonalFlowById', {
                    params: {
                        employeeId: employeeId,
                        flowTemplateId: flowTemplateId
                    }
                });
            },
            createPersonalFlow: function (model) {
                return $http.post(baseUrl + 'api/PersonalFlow/CreatePersonalFlow', model);
            },
            deletePersonalFlow: function (employeeId, flowTemplateId) {
                return $http['delete'](baseUrl + 'api/PersonalFlow/DeletePersonalFlow', {
                    params: {
                        employeeId: employeeId,
                        flowTemplateId: flowTemplateId
                    }
                });
            },
            getEmployeeFlowSetting: function (flowTemplateId, departmentId, groupId, isMonthlyPeriod) {
                return $http.get(baseUrl + 'api/PersonalFlow/GetEmployeeFlowSetting', {
                    params: {
                        flowTemplateId: flowTemplateId,
                        departmentId: departmentId,
                        groupId: groupId,
                        isMonthlyPeriod: isMonthlyPeriod
                    }
                });
            }
        };
    }
})(angular);