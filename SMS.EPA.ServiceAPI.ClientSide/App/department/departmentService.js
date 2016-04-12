(function () {
    'use strict'

    angular
        .module('app')
        .factory('departmentService', departmentService);

    departmentService.$inject = ['$http'];

    function departmentService($http) {
        var baseUrl = 'http://localhost:63238/';
        return {
            insert: insert
        };

        function insert(department) {
            var object = 'name=' + department.Name + '&id=1';
            var authorization = 'Bearer ' + sessionStorage.getItem('Token');

            return $http.post(baseUrl + 'Department/Insert', object,
                {
                    headers: {
                        'Authorization': authorization,
                        'Content-Type': 'application/x-www-form-urlencoded'
                    }
                });
        }
    }

})();