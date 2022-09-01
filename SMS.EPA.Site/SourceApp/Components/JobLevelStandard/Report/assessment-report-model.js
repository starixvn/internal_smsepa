(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelStandardModule').factory('AssessmentReportModel', assessmentReportModel);
    assessmentReportModel.$inject = ['assessmentReportFactory', 'errorFactory'];

    function assessmentReportModel(assessmentReportFactory, errorFactory) {
        let model = {};

        model.periods = [];
        model.orgChart = [];
        model.kpis = [];
        model.assessments = [];

        model.getPeriods = function () {
            return assessmentReportFactory.getPeriods().success(function (periods) {
                model.periods = periods;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getOrgChart = function (periodId) {
            return assessmentReportFactory.getOrgChart(periodId).success(function (data) {
                model.orgChart = data;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getAssessments = function (params) {
            return assessmentReportFactory.getAssessments(params).success(function (data) {
                model.kpis = data.kpis;
                model.assessments = data.assessments;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        return model;
    }
})(angular);