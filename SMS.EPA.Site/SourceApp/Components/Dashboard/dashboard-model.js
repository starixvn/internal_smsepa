(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('dashboardModule').factory('DashboardModel', dashboardModel);
    dashboardModel.$inject = ['$uibModal', 'dashboardFactory', 'errorFactory'];

    function dashboardModel($uibModal, dashboardFactory, errorFactory) {
        let model = {};
        model.dashboardInfo = {};
        model.todoList = {};
        model.jlsDashboard = {};
        model.announcements = [];

        model.getDashboard = function () {
            return dashboardFactory.getDashboard().success(function (dashboardInfo) {
                model.dashboardInfo = dashboardInfo;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getToDoList = function () {
            return dashboardFactory.getToDoList().success(function (todoList) {
                model.todoList = todoList;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getJLSDashboard = function () {
            return dashboardFactory.getJLSDashboard().success(function (todoList) {
                model.jlsDashboard = todoList;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getAnnouncement = function () {
            return dashboardFactory.getAnnouncement().success(function (announcements) {
                model.announcements = announcements;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.openAnnouncementModal = function (announcements) {
            $uibModal.open({
                templateUrl: 'SourceApp/Components/Dashboard/Templates/announcement-modal.html',
                controller: 'AnnouncementController as announcementCtrl',
                backdrop: 'static',
                resolve: {
                    announcements: function () {
                        return announcements;
                    }
                },
                windowClass: 'modal-confirm-center announcement-modal'
            });
        }

        return model;
    }
})(angular);