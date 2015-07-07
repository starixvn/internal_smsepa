'use strict';
/* global appModule */

appModule.controller("ResourceController", ['$scope', '$translate', function ($scope, $translate) {
    $scope.changeLanguage = function (langKey) {
        $translate.use(langKey);
    };
} ]);
