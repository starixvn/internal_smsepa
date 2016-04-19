(function () {
    'use strict';

    angular
        .module('app')
        .factory('tokenService', tokenService);

    tokenService.$inject = ['$http'];

    function tokenService($http) {
        var baseUrl = 'https://localhost:44301/';
        
        return {
            request: request
        };

        function request(model) {
            var object = 'grant_type='+ model.grantType+ '&username='+ model.appId+ '&password='+ model.appId ;
            return $http.post(baseUrl + 'token', object, {
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
            });
        }
    }
})();