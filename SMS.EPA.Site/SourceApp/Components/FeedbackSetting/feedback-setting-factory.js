(function (angular) {
    'use strict';

    angular.module('feedbackSettingModule').factory('feedbackSettingFactory', feedbackSettingFactory);
    feedbackSettingFactory.$inject = ['$http', 'baseUrl'];

    function feedbackSettingFactory($http, baseUrl) {
        return {
            getSetting: getSetting,
            updateSetting: updateSetting
        };

        function getSetting(periodId, departmentId, employeeId) {
            return $http.get(baseUrl + 'api/feedback/getFeedbackSetting', {
                params: {
                    periodId: periodId,
                    departmentId: departmentId,
                    employeeId
                }
            });
        }

        function updateSetting(criteria) {
            return $http.post(baseUrl + 'api/feedback/updateFeedbackSetting', criteria);
        }
    }
})(angular);