'use strict';
/*global kpiModule*/

(function (angular) {
    angular.module('kpiModule').factory('kpiFactory', kpiFactory);
    kpiFactory.$inject = ['$http', 'baseUrl'];

    function kpiFactory($http, baseUrl) {
        function createKPI(kpi) {
            return $http.post(baseUrl + 'api/KPI/CreateKPI', kpi);
        }

        function deleteKPI(kpiId) {
            return $http['delete'](baseUrl + 'api/KPI/DeleteKPI', { params: { kpiId: kpiId } });
        }

        function getKPI(kpiGroupId, isActive, isRequiredEvaluate) {
            return $http.get(baseUrl + 'api/KPI/GetKPI', {
                params: {
                    kpiGroupId: kpiGroupId,
                    isActive: isActive,
                    isRequiredEvaluate: isRequiredEvaluate
                }
            });
        }

        function getUserPermissionOnKPI() {
            return $http.get(baseUrl + 'api/KPI/GetUserPermissionOnKPI');
        }

        function updateKPI(kpi) {
            return $http.put(baseUrl + 'api/KPI/UpdateKPI', kpi);
        }

        function updateRequiredEvaluateKPI(kpi) {
            return $http.put(baseUrl + 'api/KPI/UpdateRequiredEvaluateKPI', kpi);
        }

        function updateStatusKPI(kpi) {
            return $http.put(baseUrl + 'api/KPI/UpdateStatusKPI', kpi);
        }

        return {
            createKPI: createKPI,
            deleteKPI: deleteKPI,
            getKPI: getKPI,
            getUserPermissionOnKPI: getUserPermissionOnKPI,
            updateKPI: updateKPI,
            updateRequiredEvaluateKPI: updateRequiredEvaluateKPI,
            updateStatusKPI: updateStatusKPI
        };
    }
})(angular);

kpiModule.directive('updateTrigger', function () {
    return {
        link: function (scope, iElement, attrs) {
            scope.$watch(attrs.ngModel, function () {
                iElement.trigger('chosen:updated');
            });
        }
    };
});