(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').directive('epaEvaluationProcess', evaluationProcess);
    evaluationProcess.$inject = ['utilitiesFactory'];

    function evaluationProcess(utilitiesFactory) {
        return {
            restrict: 'E',
            scope: {
                epaInfo: '='
            },
            controller: function () {
                var vm = this;

                vm.statusTypes = utilitiesFactory.statusEndProcess;
                vm.getClassProgress = getClassProgress;

                function getClassProgress(status) {
                    if (status === vm.statusTypes.InProgress) {
                        return 'in-process';
                    } else if (status === vm.statusTypes.EndProcess) {
                        return 'end-process';
                    } else {
                        return 'open';
                    }
                }
            },
            controllerAs: 'evaluationProcessCtrl',
            bindToController: true,
            templateUrl: '/SourceApp/Components/EPA/Share/Directives/Templates/evaluationProcess.tmpl.html'
        };
    }
})(angular);