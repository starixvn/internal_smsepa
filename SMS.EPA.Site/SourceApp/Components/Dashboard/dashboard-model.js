(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('dashboardModule').factory('DashboardModel', dashboardModel);
    dashboardModel.$inject = ['dashboardFactory', 'errorFactory'];

    function dashboardModel(dashboardFactory, errorFactory) {
        var dashboardInfo = {};

        var model = {
            dashboardInfo: dashboardInfo,
            getDashboard: getDashboard
        };

        return model;

        function getDashboard() {
            return dashboardFactory.getDashboard().success(function (dashboardInfo) {
                model.dashboardInfo = dashboardInfo;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }
    }
})(angular);