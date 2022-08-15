(function (angular) {
    'use strict';

    angular.module('appModule',
            [
                'employeeModule',
                'commonModule',
                'kpiModule',
                'kpiGroupModule',
                'roleModule',
                'targetTemplateModule',
                'flowMngModule',
                'targetEmployeeSettingModule',
                'departmentModule',
                'epaModule',
                'periodModule',
                'dashboardModule',
                'reportModule',
                'epaHistoryModule',
                'monthlyPeriodModule',
                'disciplineModule',
                'jobDescriptionModule',
                'jobLevelModule',
                'jobLevelStandardModule',
                'ui.bootstrap.contextMenu',
                'ui.bootstrap',
                'angular.chosen',
                'pascalprecht.translate',
                'angularUtils.directives.dirPagination',
                'ngRoute',
                'ngAnimate',
                'ngTouch',
                'multi-select',
                'purplefox.numeric',
                'angular.filter'
            ])
        .config(appConfig);

    appConfig.$inject = ['$translateProvider',
            '$locationProvider',
            '$routeProvider',
            '$httpProvider',
            '$provide',
            'uibDatepickerConfig',
            'uibDatepickerPopupConfig'];

    function appConfig($translateProvider,
                $locationProvider,
                $routeProvider,
                $httpProvider,
                $provide,
                datepickerConfig,
                datepickerPopupConfig) {
        $translateProvider.useLoader('resourceFactory', {});
        $translateProvider.preferredLanguage('en');

        $httpProvider.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';
        $httpProvider.defaults.headers.common['X-XSRF-Token'] = angular.element('input[name="__RequestVerificationToken"]').attr('value');

        $routeProvider
            .when('/Home/AccessDenied', { templateUrl: '/SourceApp/Shared/Templates/AccessDenied.html' })
            .when('/Home/InternalError', { templateUrl: '/SourceApp/Shared/Templates/InternalError.html' })
            .when('/Home/PageNotFound', { templateUrl: '/SourceApp/Shared/Templates/PageNotFound.html' })
            .otherwise({ templateUrl: '/SourceApp/Shared/Templates/PageNotFound.html' });

        $locationProvider.html5Mode(false);

        datepickerConfig.showWeeks = false;
        datepickerPopupConfig.showButtonBar = false;
        datepickerPopupConfig.datepickerPopup = 'dd MMM, yyyy';

        var baseUrl = $('base').first().attr('href');
        $provide.value('baseUrl', baseUrl);
    }
})(angular);