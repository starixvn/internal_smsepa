(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').directive('jobDescription', jobDescription);
    jobDescription.$inject = [];

    function jobDescription() {
        return {
            restrict: 'E',
            scope: {
                jobDescriptions: '='
            },
            controller: function () {
            },
            controllerAs: 'jobDescriptionCtrl',
            bindToController: true,
            templateUrl: '/SourceApp/Components/EPA/Share/Directives/Templates/job-description.tmpl.html'
        };
    }
})(angular);