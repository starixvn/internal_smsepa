'use strict';

(function (angular) {
    'use strict';

    function lockScreenService() {
        var service = {
            start: function () {
                $('.loading-process').fadeIn();
            },
            stop: function () {
                $('.loading-process').fadeOut(300);
            }
        };

        return service;
    }

    lockScreenService.$inject = [];

    angular
        .module('appModule')
        .factory('lockScreenService', lockScreenService);
})(angular);