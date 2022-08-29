'use strict';
/*exported epaModule*/

angular.module('employeeModule', ['ngRoute'])
    .config(['$routeProvider', function ($routeProvider) {
        var baseUrl = $('base').first().attr('href');
        $routeProvider.when('/Employee/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/Employee/Templates/index.html', caseInsensitiveMatch: true });
    }]);

angular.module('commonModule', ['ngRoute']);

angular.module('kpiModule', ['ngRoute'])
    .config(['$routeProvider', function ($routeProvider) {
        var baseUrl = $('base').first().attr('href');
        $routeProvider.when('/KPI/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/KPI/Templates/index.html', caseInsensitiveMatch: true });
    }]);

angular.module('kpiGroupModule', ['ngRoute'])
    .config(['$routeProvider', function ($routeProvider) {
        var baseUrl = $('base').first().attr('href');
        $routeProvider.when('/KPIGroup/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/KPIGroup/Templates/index.html', caseInsensitiveMatch: true });
    }]);

angular.module('roleModule', ['ngRoute'])
    .config(['$routeProvider', function ($routeProvider) {
        var baseUrl = $('base').first().attr('href');
        $routeProvider.when('/Role/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/Role/Templates/index.html', caseInsensitiveMatch: true });
    }]);

angular.module('targetTemplateModule', ['ngRoute'])
    .config(['$routeProvider', function ($routeProvider) {
        var baseUrl = $('base').first().attr('href');
        $routeProvider.when('/TargetTemplate/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/TargetTemplate/Templates/index.html', caseInsensitiveMatch: true });
        $routeProvider.when('/TargetTemplate/Update/:id',
            { templateUrl: baseUrl + 'SourceApp/Components/TargetTemplate/Templates/update.html', caseInsensitiveMatch: true });
        $routeProvider.when('/TargetTemplate/Detail/:id',
            { templateUrl: baseUrl + 'SourceApp/Components/TargetTemplate/Templates/detail.html', caseInsensitiveMatch: true });
    }]);

angular.module('flowMngModule', ['ngRoute'])
    .config(['$routeProvider', function ($routeProvider) {
        var baseUrl = $('base').first().attr('href');
        $routeProvider.when('/Flow/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/FlowManagement/Templates/index.html', caseInsensitiveMatch: true });
        $routeProvider.when('/Flow/Update/:id',
            { templateUrl: baseUrl + 'SourceApp/Components/FlowManagement/Templates/update.html', caseInsensitiveMatch: true });
    }]);

angular.module('targetEmployeeSettingModule', ['ngRoute'])
    .config(['$routeProvider', function ($routeProvider) {
        var baseUrl = $('base').first().attr('href');
        $routeProvider.when('/TargetEmployeeSetting/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/TargetEmployeeSetting/Templates/index.html', caseInsensitiveMatch: true });
    }]);

angular.module('departmentModule', ['ngRoute'])
    .config(['$routeProvider', function ($routeProvider) {
        var baseUrl = $('base').first().attr('href');
        $routeProvider.when('/Department/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/Department/Templates/index.html', caseInsensitiveMatch: true });
    }]);

var epaModule = angular.module('epaModule', ['ngRoute'])
    .config(['$routeProvider', function ($routeProvider) {
        var baseUrl = $('base').first().attr('href');
        $routeProvider.when('/SelfEvaluation/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/EPA/Self/Templates/index.html', caseInsensitiveMatch: true });
        $routeProvider.when('/SelfEvaluation/Detail/:id',
            { templateUrl: baseUrl + 'SourceApp/Components/EPA/Self/Templates/detail.html', caseInsensitiveMatch: true });
        $routeProvider.when('/Evaluator/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/EPA/Evaluator/Templates/index.html', caseInsensitiveMatch: true });
        $routeProvider.when('/Evaluator/Detail/:id',
            { templateUrl: baseUrl + 'SourceApp/Components/EPA/Evaluator/Templates/detail.html', caseInsensitiveMatch: true });
        $routeProvider.when('/Reviewer/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/EPA/Reviewer/Templates/index.html', caseInsensitiveMatch: true });
        $routeProvider.when('/Reviewer/Detail/:id',
            { templateUrl: baseUrl + 'SourceApp/Components/EPA/Reviewer/Templates/detail.html', caseInsensitiveMatch: true });
        $routeProvider.when('/Management/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/EPA/Management/Templates/index.html', caseInsensitiveMatch: true });
        $routeProvider.when('/Management/Detail/:id',
            { templateUrl: baseUrl + 'SourceApp/Components/EPA/Management/Templates/detail.html', caseInsensitiveMatch: true });
        $routeProvider.when('/EPA/Detail/:id',
            { templateUrl: baseUrl + 'SourceApp/Components/EPA/Templates/viewDetail.html', caseInsensitiveMatch: true });
        $routeProvider.when('/RatingByKPI/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/EPA/RatingByKPI/Templates/index.html', caseInsensitiveMatch: true });
        $routeProvider.when('/OldEPA/Convert',
            { templateUrl: baseUrl + 'SourceApp/Components/EPA/Convert/Templates/detail.html', caseInsensitiveMatch: true });
        $routeProvider.when('/EPA/ResetScore/:id',
            { templateUrl: baseUrl + 'SourceApp/Components/EPA/ResetScore/Templates/detail.html', caseInsensitiveMatch: true });
        $routeProvider.when('/ViewEPAByKPI/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/EPA/EPAByKPI/Templates/index.html', caseInsensitiveMatch: true });
        $routeProvider.when('/UpdateFinalScore/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/EPA/UpdateFinalScore/Templates/index.html', caseInsensitiveMatch: true });
    }]);

angular.module('periodModule', ['ngRoute'])
    .config(['$routeProvider', function ($routeProvider) {
        var baseUrl = $('base').first().attr('href');
        $routeProvider.when('/Period/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/Period/Templates/index.html', caseInsensitiveMatch: true });
        $routeProvider.when('/Period/Update/:id',
            { templateUrl: baseUrl + 'SourceApp/Components/Period/Templates/edit.html', caseInsensitiveMatch: true });
    }]);

angular.module('dashboardModule', ['ngRoute'])
    .config(['$routeProvider', function ($routeProvider) {
        var baseUrl = $('base').first().attr('href');
        $routeProvider.when('/',
            { templateUrl: baseUrl + 'SourceApp/Components/Dashboard/Templates/index.html', caseInsensitiveMatch: true });
    }]);

angular.module('reportModule', ['ngRoute'])
    .config(['$routeProvider', function ($routeProvider) {
        var baseUrl = $('base').first().attr('href');
        $routeProvider.when('/Report/EPASummaryReport',
            { templateUrl: baseUrl + 'SourceApp/Components/Report/Templates/EPASummaryReport.html', caseInsensitiveMatch: true });
        $routeProvider.when('/Report/EPADetailReport',
            { templateUrl: baseUrl + 'SourceApp/Components/Report/Templates/EPADetailReport.html', caseInsensitiveMatch: true });
        $routeProvider.when('/Report/MonthlyEPADetailReport',
            { templateUrl: baseUrl + 'SourceApp/Components/Report/Templates/MonthlyEPADetailReport.html', caseInsensitiveMatch: true });
        $routeProvider.when('/Report/MonthlyEPASummaryReport',
            { templateUrl: baseUrl + 'SourceApp/Components/Report/Templates/MonthlyEPASummaryReport.html', caseInsensitiveMatch: true });
    }]);

angular.module('epaHistoryModule', ['ngRoute'])
    .config(['$routeProvider', function ($routeProvider) {
        var baseUrl = $('base').first().attr('href');
        $routeProvider.when('/EPAHistory/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/EPAHistory/Templates/epaHistory.html', caseInsensitiveMatch: true });
        $routeProvider.when('/MonthlyEPAHistory/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/EPAHistory/Templates/monthlyEPAHistory.html', caseInsensitiveMatch: true });
    }]);

angular.module('monthlyPeriodModule', ['ngRoute'])
    .config(['$routeProvider', function ($routeProvider) {
        var baseUrl = $('base').first().attr('href');
        $routeProvider.when('/MonthlyPeriod/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/MonthlyPeriod/Templates/index.html', caseInsensitiveMatch: true });
        $routeProvider.when('/MonthlyPeriod/Update/:id',
            { templateUrl: baseUrl + 'SourceApp/Components/MonthlyPeriod/Templates/edit.html', caseInsensitiveMatch: true });
    }]);

angular.module('disciplineModule', ['ngRoute'])
    .config(['$routeProvider', function ($routeProvider) {
        var baseUrl = $('base').first().attr('href');
        $routeProvider.when('/Discipline/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/Discipline/Templates/index.html', caseInsensitiveMatch: true });
        $routeProvider.when('/DisciplineSummary/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/Discipline/Templates/disciplineSummary.html', caseInsensitiveMatch: true });
    }]);

angular.module('jobDescriptionModule', ['ngRoute'])
    .config(['$routeProvider', function ($routeProvider) {
        var baseUrl = $('base').first().attr('href');
        $routeProvider.when('/JobDescription/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/JobDescription/Templates/index.html', caseInsensitiveMatch: true });
        $routeProvider.when('/JobDescription/EditGroup/:id',
            { templateUrl: baseUrl + 'SourceApp/Components/JobDescription/Templates/edit-group.html', caseInsensitiveMatch: true });
        $routeProvider.when('/JobDescription/EditEmployee/:id',
            { templateUrl: baseUrl + 'SourceApp/Components/JobDescription/Templates/edit-personnel.html', caseInsensitiveMatch: true });
    }]);

angular.module('jobLevelModule', ['ngRoute'])
    .config(['$routeProvider', function ($routeProvider) {
        var baseUrl = $('base').first().attr('href');
        $routeProvider.when('/JobLevel/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/JobLevel/Templates/index.html', caseInsensitiveMatch: true });
    }]);

angular.module('jobLevelStandardModule', ['ngRoute'])
    .config(['$routeProvider', function ($routeProvider) {
        var baseUrl = $('base').first().attr('href');
        $routeProvider.when('/JobLevelStandard/JobLevelSetting/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/JobLevelStandard/JobLevelSetting/Templates/index.html', caseInsensitiveMatch: true });

        $routeProvider.when('/JobLevelStandard/Period/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/JobLevelStandard/Period/Templates/index.html', caseInsensitiveMatch: true });
        $routeProvider.when('/JobLevelStandard/Period/Edit/:id',
            { templateUrl: baseUrl + 'SourceApp/Components/JobLevelStandard/Period/Templates/edit.html', caseInsensitiveMatch: true });

        $routeProvider.when('/JobLevelStandard/Assessment/Index',
            { templateUrl: baseUrl + 'SourceApp/Components/JobLevelStandard/Assessment/Templates/index.html', caseInsensitiveMatch: true });
    }]);