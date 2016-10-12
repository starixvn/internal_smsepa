(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('epaModule').controller('EmployeeEPAController', employeeEPAController);
    employeeEPAController.$inject = ['SelfEvalutionModel', 'lockScreenService', 'baseUrl'];

    function employeeEPAController(SelfEvalutionModel, lockScreenService, baseUrl) {
        var vm = this;

        vm.epaInfos = [];

        vm.viewEPADetail = viewEPADetail;
        vm.viewEPAResult = viewEPAResult;

        initialize();

        function initialize() {
            lockScreenService.start();

            SelfEvalutionModel.getEPAByEmployee().then(function () {
                vm.epaInfos = SelfEvalutionModel.epaInfos;
                lockScreenService.stop();
            });
        }

        function viewEPADetail(epaInfo) {
            vm.viewEPADetailUrl = baseUrl + 'SelfEvaluation/Detail/' + epaInfo.EPAId;
        }

        function viewEPAResult(epaInfo) {
            vm.viewEPAResultUrl = baseUrl + 'EPA/Detail/' + epaInfo.EPAId;
        }
    }
})(angular);