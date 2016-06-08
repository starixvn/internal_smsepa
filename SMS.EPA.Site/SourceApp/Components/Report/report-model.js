'use strict';
/*global reportModule*/

reportModule.factory('ReportModel',
['reportFactory', 'notificationFactory', 'errorFactory',
function (reportFactory, notificationFactory, errorFactory) {
    var reportModel = {};

    reportModel.epaSummary = [];
    reportModel.epaDetail = [];
    reportModel.periods = [];
    reportModel.pdfFileName = "";

    reportModel.requestExportEPA = function (epaId) {
        return reportFactory.requestExportEPA(epaId)
            .success(function (fileName) {
                reportModel.pdfFileName = fileName;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
    };

    reportModel.requestExportSemiEPA = function (epaId) {
        return reportFactory.requestExportSemiEPA(epaId)
            .success(function (fileName) {
                reportModel.pdfFileName = fileName;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
    };

    reportModel.getAllPeriodIncludeImport = function () {
        return reportFactory.getAllPeriodIncludeImport()
        .success(function (periods) {
            reportModel.periods = periods;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };


    reportModel.getEPASummaryReport = function (periodIds, departmentId, isLocal, sortColumnName, sortDirection, pageIndex, itemsPerPage) {
        return reportFactory.getEPASummaryReport(periodIds, departmentId, isLocal, sortColumnName, sortDirection, pageIndex, itemsPerPage)
        .success(function (epaSummary) {
            reportModel.epaSummary = epaSummary;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reportModel.getMonthlyEPASummaryReport = function (periodIds, departmentId, isLocal, pageIndex, itemsPerPage) {
        return reportFactory.getMonthlyEPASummaryReport(periodIds, departmentId, isLocal, pageIndex, itemsPerPage)
        .success(function (epaSummary) {
            reportModel.epaSummary = epaSummary;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reportModel.getEPADetailReport = function (periodId, departmentId, isLocal, phaseIds, sortColumnName, sortDirection, pageIndex, itemsPerPage) {
        return reportFactory.getEPADetailReport(periodId, departmentId, isLocal, phaseIds, sortColumnName, sortDirection, pageIndex, itemsPerPage)
        .success(function (epaDetail) {
            reportModel.epaDetail = epaDetail;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    reportModel.getMonthlyEPADetailReport = function (periodId, departmentId, isLocal, phaseIds, pageIndex, itemsPerPage) {
        return reportFactory.getMonthlyEPADetailReport(periodId, departmentId, isLocal, phaseIds, pageIndex, itemsPerPage)
        .success(function (epaDetail) {
            reportModel.epaDetail = epaDetail;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return reportModel;
}]);