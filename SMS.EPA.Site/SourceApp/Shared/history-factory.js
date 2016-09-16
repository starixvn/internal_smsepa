(function (angular) {
    'use strict';

    angular.module('appModule').factory('historyFactory', historyFactory);
    historyFactory.$inject = ['$window'];

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
})(angular);