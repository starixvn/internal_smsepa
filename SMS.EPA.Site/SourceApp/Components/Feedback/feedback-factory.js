(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('feedbackModule').factory('feedbackFactory', feedbackFactory);
    feedbackFactory.$inject = ['$http', 'baseUrl'];

    function feedbackFactory($http, baseUrl) {
        return {
            getFeedback: getFeedback,
            getFeedbackById: getFeedbackById,
            updateFeedback: updateFeedback
        };

        function getFeedback(periodId) {
            return $http.get(baseUrl + 'api/feedback/getFeedback', { params: { periodId: periodId } });
        }

        function getFeedbackById(feedbackId) {
            return $http.get(baseUrl + 'api/feedback/getById', { params: { feedbackId: feedbackId } });
        }

        function updateFeedback(feedback) {
            return $http.post(baseUrl + 'api/feedback/update', feedback);
        }
    }
})(angular);