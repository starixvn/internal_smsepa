'use strict';
/*global monthlyPeriodModule*/

monthlyPeriodModule.factory('MonthlyPeriodModel',
['monthlyPeriodFactory', 'errorFactory', 'notificationFactory',
function (monthlyPeriodFactory, errorFactory, notificationFactory) {
    var monthlyPeriodModel = {};

    monthlyPeriodModel.periods = [];
    monthlyPeriodModel.permission = 0;

    monthlyPeriodModel.calendar = {
        fromDateOpened: false,
        toDateOpened: false
    };

    monthlyPeriodModel.getPermissionOnMonthlyPeriod = function () {
        return monthlyPeriodFactory.getPermissionOnMonthlyPeriod()
        .success(function (permisson) {
            monthlyPeriodModel.permission = parseInt(permisson);
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    monthlyPeriodModel.openPopupFromDate = function (event) {
        event.preventDefault();
        event.stopPropagation();

        monthlyPeriodModel.calendar.fromDateOpened = true;
        monthlyPeriodModel.calendar.toDateOpened = false;
    };

    monthlyPeriodModel.openPopupToDate = function (event) {
        event.preventDefault();
        event.stopPropagation();

        monthlyPeriodModel.calendar.fromDateOpened = false;
        monthlyPeriodModel.calendar.toDateOpened = true;
    };

    monthlyPeriodModel.getMonthlyPeriod = function (fromDate, toDate) {
        return monthlyPeriodFactory.getMonthlyPeriod(fromDate, toDate)
        .success(function (periods) {
            monthlyPeriodModel.periods = periods;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return monthlyPeriodModel;
}]);