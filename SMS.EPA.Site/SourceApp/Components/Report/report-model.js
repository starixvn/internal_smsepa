(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('reportModule').factory('ReportModel', reportModel);
    reportModel.$inject = ['reportFactory', 'notificationFactory', 'errorFactory'];

    function reportModel(reportFactory, notificationFactory, errorFactory) {
        var model = {
            employees: [],
            epaDetail: [],
            epaSummary: [],
            pdfFileName: '',
            periods: [],
            getAllPeriodIncludeImport: getAllPeriodIncludeImport,
            getEmployeeByActorId: getEmployeeByActorId,
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

        function getEmployeeByActorId(query, periodIds, teamIds, isLocal) {
            return reportFactory.getEmployeeByActorId(query, periodIds, teamIds, isLocal).success(function (employees) {
                model.employees = employees;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getEPADetailReport(periodId, teamIds, isLocal, employeeId, phaseIds, sortColumnName, sortDirection) {
            return reportFactory.getEPADetailReport(periodId, teamIds, isLocal, employeeId, phaseIds, sortColumnName, sortDirection)
            .success(function (epaDetail) {
                model.epaDetail = epaDetail;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getEPASummaryReport(periodIds, teamIds, isLocal, employeeId, sortColumnName, sortDirection) {
            return reportFactory.getEPASummaryReport(periodIds, teamIds, isLocal, employeeId, sortColumnName, sortDirection)
            .success(function (epaSummary) {
                model.epaSummary = epaSummary;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getMonthlyEPADetailReport(periodId, teamIds, isLocal, phaseIds, sortColumnName, sortDirection) {
            return reportFactory.getMonthlyEPADetailReport(periodId, teamIds, isLocal, phaseIds, sortColumnName, sortDirection)
            .success(function (epaDetail) {
                model.epaDetail = epaDetail;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getMonthlyEPASummaryReport(periodIds, teamIds, isLocal, sortColumnName, sortDirection) {
            return reportFactory.getMonthlyEPASummaryReport(periodIds, teamIds, isLocal, sortColumnName, sortDirection)
            .success(function (epaSummary) {
                model.epaSummary = epaSummary;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }
    }
})(angular);