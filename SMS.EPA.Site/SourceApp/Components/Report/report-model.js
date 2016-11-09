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

        function getEmployeeByActorId(query, periodIds, departmentId, isLocal) {
            return reportFactory.getEmployeeByActorId(query, periodIds, departmentId, isLocal).success(function (employees) {
                model.employees = employees;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getEPADetailReport(periodId, departmentId, isLocal, employeeId, phaseIds, sortColumnName, sortDirection) {
            return reportFactory.getEPADetailReport(periodId, departmentId, isLocal, employeeId, phaseIds, sortColumnName, sortDirection)
            .success(function (epaDetail) {
                model.epaDetail = epaDetail;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getEPASummaryReport(periodIds, departmentId, isLocal, employeeId, sortColumnName, sortDirection) {
            return reportFactory.getEPASummaryReport(periodIds, departmentId, isLocal, employeeId, sortColumnName, sortDirection)
            .success(function (epaSummary) {
                model.epaSummary = epaSummary;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getMonthlyEPADetailReport(periodId, departmentId, isLocal, phaseIds, sortColumnName, sortDirection) {
            return reportFactory.getMonthlyEPADetailReport(periodId, departmentId, isLocal, phaseIds, sortColumnName, sortDirection)
            .success(function (epaDetail) {
                model.epaDetail = epaDetail;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getMonthlyEPASummaryReport(periodIds, departmentId, isLocal, sortColumnName, sortDirection) {
            return reportFactory.getMonthlyEPASummaryReport(periodIds, departmentId, isLocal, sortColumnName, sortDirection)
            .success(function (epaSummary) {
                model.epaSummary = epaSummary;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }
    }
})(angular);