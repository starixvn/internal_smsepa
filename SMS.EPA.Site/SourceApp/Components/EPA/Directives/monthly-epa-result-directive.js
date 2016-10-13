(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').directive('epaMonthlyResult', monthlyResult);
    monthlyResult.$inject = ['utilitiesFactory'];

    function monthlyResult(utilitiesFactory) {
        return {
            restrict: 'E',
            scope: {
                epaInfo: '='
            },
            controller: function () {
                var vm = this;

                vm.statusTypes = utilitiesFactory.statusEndProcess;
            },
            controllerAs: 'monthlyResultCtrl',
            bindToController: true,
            templateUrl: '/SourceApp/Components/EPA/Directives/Templates/monthlyEPAs.tmpl.html'
        };
    }
})(angular);