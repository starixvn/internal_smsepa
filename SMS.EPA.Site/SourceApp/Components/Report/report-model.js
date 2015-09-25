'use strict';
/*exported reportModule*/

reportModule.factory('ReportModel',
['reportFactory', 'notificationFactory', 'errorFactory',
function (reportFactory, notificationFactory, errorFactory) {
    var reportModel = {};

    reportModel.epaSummaries = {};

    reportModel.getEPASummaryReport = function () {
        return reportFactory.getEPASummaryReport()
        .success(function (epaSummaries) {
            reportModel.epaSummaries = epaSummaries;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return reportModel;
}]);