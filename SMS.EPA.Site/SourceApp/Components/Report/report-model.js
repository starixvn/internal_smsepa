'use strict';
/*exported reportModule*/

reportModule.factory('ReportModel',
['reportFactory', 'notificationFactory', 'errorFactory',
function (reportFactory, notificationFactory, errorFactory) {
    var reportModel = {};

    reportModel.epaSummary = {};
    reportModel.epaDetail = {};

    reportModel.getEPASummaryReport = function (periodIds, departmentId, pageIndex, itemsPerPage) {
        return reportFactory.getEPASummaryReport(periodIds, departmentId, pageIndex, itemsPerPage)
        .success(function (epaSummary) {
            reportModel.epaSummary = epaSummary;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reportModel.getEPADetailReport = function (periodId, departmentId, pageIndex, itemsPerPage) {
        return reportFactory.getEPADetailReport(periodId, departmentId, pageIndex, itemsPerPage)
        .success(function (epaDetail) {
            reportModel.epaDetail = epaDetail;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return reportModel;
}]);