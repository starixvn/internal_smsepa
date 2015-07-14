'use strict';
/* global appModule */

appModule.factory('resourceFactory', ['$http', '$q', 'baseUrl', function ($http, $q, baseUrl) {
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
} ]);