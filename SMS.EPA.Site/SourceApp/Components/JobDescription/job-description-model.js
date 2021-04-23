(function (angular) {
    'use strict';

    angular.module('jobDescriptionModule').factory('JobDescriptionModel', jobDescriptionModel);
    jobDescriptionModel.$inject = ['jobDescriptionFactory', 'errorFactory'];

    function jobDescriptionModel(jobDescriptionFactory, errorFactory) {
        var model = {
            permission: 0,
            getUserPermission: getUserPermission
        };

        return model;

        function getUserPermission() {
            return jobDescriptionFactory.getUserPermission().success(function (permission) {
                model.permission = permission;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }
    }
})(angular);