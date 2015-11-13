'use strict';
/*global dashboardModule*/

dashboardModule.factory('DashboardModel',
['$modal', 'dashboardFactory', 'notificationFactory', 'errorFactory',
function ($modal, dashboardFactory, notificationFactory, errorFactory) {
    var dashboardModel = {};

    dashboardModel.dashboardInfo = {};

    dashboardModel.getDashboard = function () {
        return dashboardFactory.getDashboard()
        .success(function (dashboardInfo) {
            dashboardModel.dashboardInfo = dashboardInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return dashboardModel;
}]);