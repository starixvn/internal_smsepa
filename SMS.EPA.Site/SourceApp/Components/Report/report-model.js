(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('reportModule').factory('ReportModel', reportModel);
    reportModel.$inject = ['reportFactory', 'notificationFactory', 'errorFactory'];

    function reportModel(reportFactory, notificationFactory, errorFactory) {
        var model = {
            epaDetail: [],
            epaSummary: [],
            pdfFileName: '',
            periods: [],
            getAllPeriodIncludeImport: getAllPeriodIncludeImport,
            getEPADetailReport: getEPADetailReport,
            getEPASummaryReport: getEPASummaryReport,
            getMonthlyEPADetailReport: getMonthlyEPADetailReport,
            getMonthlyEPASummaryReport: getMonthlyEPASummaryReport
        };

        return model;

        function getAllPeriodIncludeImport() {
            return reportFactory.getAllPeriodIncludeImport().success(function (periods) {
                model.periods = periods;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getEPADetailReport(periodId, departmentId, isLocal, phaseIds, sortColumnName, sortDirection, pageIndex, itemsPerPage) {
            return reportFactory.getEPADetailReport(periodId, departmentId, isLocal, phaseIds, sortColumnName, sortDirection, pageIndex, itemsPerPage)
            .success(function (epaDetail) {
                model.epaDetail = epaDetail;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getEPASummaryReport(periodIds, departmentId, isLocal, sortColumnName, sortDirection, pageIndex, itemsPerPage) {
            return reportFactory.getEPASummaryReport(periodIds, departmentId, isLocal, sortColumnName, sortDirection, pageIndex, itemsPerPage)
            .success(function (epaSummary) {
                model.epaSummary = epaSummary;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getMonthlyEPADetailReport(periodId, departmentId, isLocal, phaseIds, sortColumnName, sortDirection, pageIndex, itemsPerPage) {
            return reportFactory.getMonthlyEPADetailReport(periodId, departmentId, isLocal, phaseIds, sortColumnName, sortDirection, pageIndex, itemsPerPage)
            .success(function (epaDetail) {
                model.epaDetail = epaDetail;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getMonthlyEPASummaryReport(periodIds, departmentId, isLocal, sortColumnName, sortDirection, pageIndex, itemsPerPage) {
            return reportFactory.getMonthlyEPASummaryReport(periodIds, departmentId, isLocal, sortColumnName, sortDirection, pageIndex, itemsPerPage)
            .success(function (epaSummary) {
                model.epaSummary = epaSummary;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }
    }
})(angular);