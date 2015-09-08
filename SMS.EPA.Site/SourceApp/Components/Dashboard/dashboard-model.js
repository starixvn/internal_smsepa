'use strict';
/*exported dashboardModule*/

dashboardModule.factory('DashboardModel',
['dashboardFactory', 'notificationFactory', 'errorFactory',
function (dashboardFactory, notificationFactory, errorFactory) {
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