'use strict';
/*exported appModule*/

var appModule = angular.module("appModule", ['employeeModule', 'commonModule', 'kpiModule', 'kpiGroupModule', 'roleModule',
'targetTemplateModule', 'flowMngModule', 'targetEmployeeSettingModule', 'departmentModule', 'epaModule', 'periodModule',
'dashboardModule', 'attendanceModule', 'reportModule', 'epaHistoryModule', 'monthlyPeriodModule', 'errorModule',
'ui.bootstrap.contextMenu', 'ui.bootstrap', 'angular.chosen', 'pascalprecht.translate', 'angularUtils.directives.dirPagination',
'ngRoute', 'multi-select', 'purplefox.numeric', 'angularSpinner'])
.config(['$translateProvider', '$locationProvider', 'paginationTemplateProvider', '$routeProvider',
    'datepickerConfig', 'datepickerPopupConfig', '$provide', 'usSpinnerConfigProvider',
function ($translateProvider, $locationProvider, paginationTemplateProvider, $routeProvider,
    datepickerConfig, datepickerPopupConfig, $provide, usSpinnerConfigProvider) {
    $translateProvider.useLoader('resourceFactory', {});
    $routeProvider.when('/Home/AccessDenied', { templateUrl: '/SourceApp/Shared/Templates/AccessDenied.html' });
    $translateProvider.preferredLanguage('en');
    $locationProvider.html5Mode(true);
    datepickerConfig.showWeeks = false;
    datepickerPopupConfig.showButtonBar = false;
    datepickerPopupConfig.datepickerPopup = 'dd MMM, yyyy';
    paginationTemplateProvider.setPath('SourceApp/Shared/Templates/dirPagination.tpl.html');
    var baseUrl = $("base").first().attr("href");
    $provide.value("baseUrl", baseUrl);
    usSpinnerConfigProvider.setDefaults({ color: 'blue' });
    usSpinnerConfigProvider.setTheme('bigBlue', { color: '#4CA4C9', radius: 20 });
}]);