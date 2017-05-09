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
                vm.monthlyEPA = {};

                if (vm.epaInfo) {
                    vm.monthlyEPA = vm.epaInfo.MonthlyEPA;
                }
            },
            controllerAs: 'monthlyResultCtrl',
            bindToController: true,
            templateUrl: '/SourceApp/Components/EPA/Share/Directives/Templates/monthlyEPAs.tmpl.html'
        };
    }
})(angular);