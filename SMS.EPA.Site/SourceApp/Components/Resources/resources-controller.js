(function (angular) {
    'use strict';

    angular.module('appModule').controller('ResourceController', resourceController);
    resourceController.$inject = ['$scope', '$translate'];

    function resourceController($scope, $translate) {
        $scope.changeLanguage = function (langKey) {
            $translate.use(langKey);
        };
    }
})(angular);