(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('app').factory('departmentService', departmentService);

    departmentService.$inject = ['$http'];

    function departmentService($http) {
        var baseUrl = 'https://localhost:44301/';
        return {
            insert: insert
        };

        function insert(department) {
            var authorization = 'Bearer ' + sessionStorage.getItem('Token');

            return $http.post(baseUrl + 'Department/Update', JSON.stringify(department),
                {
                    headers: {
                        'Authorization': authorization,
                        'Content-Type': 'application/json'
                    }
                });
        }
    }
})(angular);