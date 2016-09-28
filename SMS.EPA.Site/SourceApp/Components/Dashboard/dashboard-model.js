(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('dashboardModule').factory('DashboardModel', dashboardModel);
    dashboardModel.$inject = ['dashboardFactory', 'errorFactory'];

    function dashboardModel(dashboardFactory, errorFactory) {
        var dashboardInfo = {};
        var todoList = {};

        var model = {
            dashboardInfo: dashboardInfo,
            getDashboard: getDashboard,
            getToDoList: getToDoList,
            todoList: todoList
        };

        return model;

        function getDashboard() {
            return dashboardFactory.getDashboard().success(function (dashboardInfo) {
                model.dashboardInfo = dashboardInfo;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getToDoList() {
            return dashboardFactory.getToDoList().success(function (todoList) {
                model.todoList = todoList;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }
    }
})(angular);