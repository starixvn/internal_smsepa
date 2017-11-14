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
            getPeriodByReviewer: getPeriodByReviewer,
            getGroupByReviewer: getGroupByReviewer,
            getMemberByReviewer: getMemberByReviewer,
            getUserPermissionOnReviewer: getUserPermissionOnReviewer
        };

        function adjustScoreByReviewer(criteria) {
            return $http.put(baseUrl + 'api/ReviewerEPA/AdjustScoreByReviewer', criteria);
        }

        function getEPAByReviewer(periodId, groupId, isLocal, isMonthly, employeeIds, statusProcess) {
            return $http.get(baseUrl + 'api/ReviewerEPA/GetEPAByReviewer', {
                params: {
                    periodId: periodId,
                    groupId: groupId,
                    isLocal: isLocal,
                    isMonthly: isMonthly,
                    employeeIds: employeeIds,
                    statusProcess: statusProcess
                }
            });
        }

        function getEPAInfoByReviewer(epaId, reviewerId) {
            return $http.get(baseUrl + 'api/ReviewerEPA/GetEPAInfoByReviewer', { params: { epaId: epaId, reviewerId: reviewerId} });
        }

        function getPeriodByReviewer() {
            return $http.get(baseUrl + 'api/ReviewerEPA/GetPeriodByReviewer');
        }

        function getGroupByReviewer(periodId) {
            return $http.get(baseUrl + 'api/ReviewerEPA/GetGroupByReviewer', { params: { periodId: periodId } });
        }

        function getMemberByReviewer(periodId, groupId, isLocal, isMonthly) {
            return $http.get(baseUrl + 'api/ReviewerEPA/GetMemberByReviewer', {
                params: {
                    periodId: periodId,
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