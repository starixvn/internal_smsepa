(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').factory('managementEPAFactory', managementEPAFactory);
    managementEPAFactory.$inject = ['$http', 'baseUrl'];

    function managementEPAFactory($http, baseUrl) {
        return {
            adjustScoreByManagement: adjustScoreByManagement,
            getEPAByManagement: getEPAByManagement,
            getEPAInfoByManagement: getEPAInfoByManagement,
            getGroupByManagement: getGroupByManagement,
            getMemberByManagement: getMemberByManagement,
            getUserPermissionOnManagement: getUserPermissionOnManagement
        };

        function adjustScoreByManagement(criteria) {
            return $http.put(baseUrl + 'api/ManagementEPA/AdjustScoreByManagement', criteria);
        }

        function getEPAByManagement(groupId, isLocal, employeeIds, statusProcess) {
            return $http.get(baseUrl + 'api/ManagementEPA/GetEPAByManagement', {
                params: {
                    groupId: groupId,
                    isLocal: isLocal,
                    employeeIds: employeeIds,
                    statusProcess: statusProcess
                }
            });
        }

        function getEPAInfoByManagement(epaId) {
            return $http.get(baseUrl + 'api/ManagementEPA/GetEPAInfoByManagement', { params: { epaId: epaId } });
        }

        function getGroupByManagement() {
            return $http.get(baseUrl + 'api/ManagementEPA/GetGroupByManagement');
        }

        function getMemberByManagement(groupId, isLocal) {
            return $http.get(baseUrl + 'api/ManagementEPA/GetMemberByManagement', {
                params: {
                    groupId: groupId,
                    isLocal: isLocal
                }
            });
        }

        function getUserPermissionOnManagement() {
            return $http.get(baseUrl + 'api/ManagementEPA/GetUserPermissionOnManagement');
        }
    }
})(angular);