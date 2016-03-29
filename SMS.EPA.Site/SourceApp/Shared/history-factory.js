'use strict';
(function (angular) {
    function historyFactory($window) {
        var isBack = false;
        var model = {};

        var service = {
            saveState: function (filter) {
                model = filter;
                isBack = false;
            },

            getState: function () {
                if (isBack && model) {
                    isBack = false;

                    return model;
                }

                return null;
            },

            back: function () {
                isBack = true;
                $window.history.back();
            }
        };

        return service;
    }

    historyFactory.$inject = ['$window'];

    angular
        .module('appModule')
        .factory('historyFactory', historyFactory);
})(angular);
