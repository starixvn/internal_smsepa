(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').factory('reviewerEPAFactory', reviewerEPAFactory);
    reviewerEPAFactory.$inject = ['$http', 'baseUrl'];

    function reviewerEPAFactory($http, baseUrl) {
        return {
            adjustScoreByReviewer: adjustScoreByReviewer,
            getEPAByReviewer: getEPAByReviewer,
            getEPAInfoByReviewer: getEPAInfoByReviewer,
            getGroupByReviewer: getGroupByReviewer,
            getMemberByReviewer: getMemberByReviewer,
            getUserPermissionOnReviewer: getUserPermissionOnReviewer
        };

        function adjustScoreByReviewer(criteria) {
            return $http.put(baseUrl + 'api/ReviewerEPA/AdjustScoreByReviewer', criteria);
        }

        function getEPAByReviewer(groupId, isLocal, isMonthly, employeeIds, statusProcess) {
            return $http.get(baseUrl + 'api/ReviewerEPA/GetEPAByReviewer', {
                params: {
                    groupId: groupId,
                    isLocal: isLocal,
                    isMonthly: isMonthly,
                    employeeIds: employeeIds,
                    statusProcess: statusProcess
                }
            });
        }

        function getEPAInfoByReviewer(epaId) {
            return $http.get(baseUrl + 'api/ReviewerEPA/GetEPAInfoByReviewer', { params: { epaId: epaId } });
        }

        function getGroupByReviewer() {
            return $http.get(baseUrl + 'api/ReviewerEPA/GetGroupByReviewer');
        }

        function getMemberByReviewer(groupId, isLocal, isMonthly) {
            return $http.get(baseUrl + 'api/ReviewerEPA/GetMemberByReviewer', {
                params: {
                    groupId: groupId,
                    isLocal: isLocal,
                    isMonthly: isMonthly
                }
            });
        }

        function getUserPermissionOnReviewer() {
            return $http.get(baseUrl + 'api/ReviewerEPA/GetUserPermissionOnReviewer');
        }
    }
})(angular);