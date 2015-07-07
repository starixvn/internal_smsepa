'use strict';
/*exported appModule*/

var appModule = angular.module("appModule", ['ui.bootstrap', 'pascalprecht.translate', 'angularUtils.directives.dirPagination', 'ngRoute'])
.config(['$translateProvider', '$locationProvider', 'paginationTemplateProvider', '$routeProvider', 'datepickerConfig', 'datepickerPopupConfig',
function ($translateProvider, $locationProvider, paginationTemplateProvider, $routeProvider, datepickerConfig, datepickerPopupConfig) {
    $translateProvider.useLoader('resourceFactory', {});
    $routeProvider.when('/Home/AccessDenied', { templateUrl: '/SourceApp/Shared/Templates/AccessDenied.html' });
    $translateProvider.preferredLanguage('en');
    $locationProvider.html5Mode(true);
    datepickerConfig.showWeeks = false;
    datepickerPopupConfig.showButtonBar = false;
    paginationTemplateProvider.setPath('SourceApp/Shared/Templates/dirPagination.tpl.html');
} ]);