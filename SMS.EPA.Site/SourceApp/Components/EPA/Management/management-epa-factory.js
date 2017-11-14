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
            getPeriodByManagement: getPeriodByManagement,
            getGroupByManagement: getGroupByManagement,
            getMemberByManagement: getMemberByManagement,
            getUserPermissionOnManagement: getUserPermissionOnManagement
        };

        function adjustScoreByManagement(criteria) {
            return $http.put(baseUrl + 'api/ManagementEPA/AdjustScoreByManagement', criteria);
        }

        function getEPAByManagement(periodId, groupId, isLocal, employeeIds, statusProcess) {
            return $http.get(baseUrl + 'api/ManagementEPA/GetEPAByManagement', {
                params: {
                    periodId: periodId,
                    groupId: groupId,
                    isLocal: isLocal,
                    employeeIds: employeeIds,
                    statusProcess: statusProcess
                }
            });
        }

        function getEPAInfoByManagement(epaId, managementId) {
            return $http.get(baseUrl + 'api/ManagementEPA/GetEPAInfoByManagement', { params: { epaId: epaId, managementId: managementId } });
        }

        function getPeriodByManagement() {
            return $http.get(baseUrl + 'api/ManagementEPA/GetPeriodByManagement');
        }

        function getGroupByManagement(periodId) {
            return $http.get(baseUrl + 'api/ManagementEPA/GetGroupByManagement', { params: { periodId: periodId } });
        }

        function getMemberByManagement(periodId, groupId, isLocal) {
            return $http.get(baseUrl + 'api/ManagementEPA/GetMemberByManagement', {
                params: {
                    periodId: periodId,
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