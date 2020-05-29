(function (angular) {
    'use strict';

    angular.module('feedbackReportModule').factory('feedbackReportFactory', feedbackReportFactory);
    feedbackReportFactory.$inject = ['$http', 'baseUrl'];

    function feedbackReportFactory($http, baseUrl) {
        return {
            getData: getData,
            exportExcel: exportExcel
        };

        function getData(criteria) {
            return $http.post(baseUrl + 'api/feedbackReport/getData', criteria);
        }

        function exportExcel(criteria) {
            return $http({
                url: baseUrl + 'exportFeedbackReport/exportExcel',
                method: 'POST',
                responseType: 'blob',
                data: criteria
            });
        }
    }
})(angular);