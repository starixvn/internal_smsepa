(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('epaModule').controller('EmployeeEPAController', employeeEPAController);
    employeeEPAController.$inject = ['SelfEvalutionModel', 'lockScreenService', 'baseUrl', 'utilitiesFactory'];

    function employeeEPAController(SelfEvalutionModel, lockScreenService, baseUrl, utilitiesFactory) {
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

        function viewEPADetail(epaInfo, event) {
            var url = baseUrl + 'SelfEvaluation/Detail/' + epaInfo.EPAId;
            utilitiesFactory.handleOpenLink(event, url);
        }

        function viewEPAResult(epaInfo, event) {
            var url = baseUrl + 'EPA/Detail/' + epaInfo.EPAId;
            utilitiesFactory.handleOpenLink(event, url);
        }
    }
})(angular);