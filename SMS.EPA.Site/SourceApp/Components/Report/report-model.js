'use strict';
/*exported reportModule*/

reportModule.factory('ReportModel',
['reportFactory', 'notificationFactory', 'errorFactory',
function (reportFactory, notificationFactory, errorFactory) {
    var reportModel = {};

    reportModel.epaSummary = [];
    reportModel.epaDetail = [];
    reportModel.periods = [];

    reportModel.getPeriod = function (targetTypeId, isActive) {
        return reportFactory.getPeriod(targetTypeId, isActive)
        .success(function (periods) {
            reportModel.periods = periods;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reportModel.getEPASummaryReport = function (periodIds, departmentId, pageIndex, itemsPerPage) {
        return reportFactory.getEPASummaryReport(periodIds, departmentId, pageIndex, itemsPerPage)
        .success(function (epaSummary) {
            reportModel.epaSummary = epaSummary;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reportModel.getEPADetailReport = function (periodId, departmentId, phaseIds, pageIndex, itemsPerPage) {
        return reportFactory.getEPADetailReport(periodId, departmentId, phaseIds, pageIndex, itemsPerPage)
        .success(function (epaDetail) {
            reportModel.epaDetail = epaDetail;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return reportModel;
}]);