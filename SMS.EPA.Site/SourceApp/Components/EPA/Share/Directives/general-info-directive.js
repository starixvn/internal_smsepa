(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').directive('epaGeneralInfomation', generalInfomation);
    generalInfomation.$inject = [];

    function generalInfomation() {
        return {
            restrict: 'E',
            scope: {
                employeeInfo: '=',
                evaluatedDate: '=',
                saved: '=',
                isViewDetail: '='
            },
            controller: function () {
            },
            controllerAs: 'generalInfoCtrl',
            bindToController: true,
            templateUrl: '/SourceApp/Components/EPA/Share/Directives/Templates/generalInfo.tmpl.html'
        };
    }
})(angular);