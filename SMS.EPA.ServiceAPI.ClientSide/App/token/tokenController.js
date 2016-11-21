(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('app').controller('TokenController', tokenController);

    tokenController.$inject = ['tokenService'];
    function tokenController(tokenService) {
        var vm = this;

        vm.loginModel = {
            grantType: 'password',
            appId: 'app~1234~APP'
        };
        vm.token = {
            accessToken: '',
            expiresIn: 0
        };

        vm.requestToken = requestToken;
        vm.getExistedToken = getExistedToken;
        vm.deleteToken = deleteToken;
        vm.Title = "Hello token!";

        vm.getExistedToken();

        function requestToken() {
            tokenService.request(vm.loginModel).success(function (data) {
                vm.token.accessToken = data.access_token;
                vm.token.expiresIn = data.expires_in;

                sessionStorage.setItem('Token', vm.token.accessToken);
            }).error(function (response, status) {
                console.log("error: " + status);
            });
        }

        function deleteToken() {
            sessionStorage.removeItem('Token');
            vm.token = {
                accessToken: '',
                expiresIn: 0
            };
        }

        function getExistedToken() {
            var token = sessionStorage.getItem('Token');
            if (token) {
                vm.token.accessToken = token;
            }
        }
    };
})(angular);