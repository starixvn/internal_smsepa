(function (angular) {
    'use strict';

    angular.module('appModule').factory('resourceFactory', resourceFactory);
    resourceFactory.$inject = ['$http', '$q', 'baseUrl'];

    function resourceFactory($http, $q, baseUrl) {
        return function (options) {
            var deferred = $q.defer();

            $http.get(baseUrl + 'api/Resources/Get?lang=' + options.key)
                .success(function (data) {
                    if (data) {
                        data = angular.fromJson(angular.fromJson(data));
                    }

                    deferred.resolve(data);
                }).error(deferred.reject);

            return deferred.promise;
        };
    }
})(angular);