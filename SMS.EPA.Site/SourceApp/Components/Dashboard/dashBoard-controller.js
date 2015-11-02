'use strict';
/*exported dashboardModule*/

dashboardModule.controller('DashboardController',
['DashboardModel', 'utilitiesFactory', 'baseUrl',
function (DashboardModel, utilitiesFactory, baseUrl) {
    var vm = this;

    vm.showScore = false;

    vm.viewEPADetail = function (epa) {
        vm.viewEPADetailUrl = baseUrl + 'SelfEvaluation/Detail/' + epa.EPAId;
    };

    vm.viewEPAResult = function (epa) {
        vm.viewEPAResultUrl = baseUrl + 'EPA/Detail/' + epa.EPAId;
    };

    vm.initialize = function () {
        DashboardModel.getDashboard().then(function () {
            var dashboardInfo = DashboardModel.dashboardInfo;

            vm.employee = dashboardInfo.Employee;
            vm.employee.Avatar = baseUrl + vm.employee.Avatar;
            vm.myEPAs = dashboardInfo.MyEPAs;

            angular.forEach(vm.myEPAs, function (myEPA) {
                if (myEPA.Status.StatusId === 1) {
                    myEPA.FinalScore = utilitiesFactory.parseFloat(myEPA.FinalScore);
                } else {
                    myEPA.FinalScore = null;
                }
            });
        });
    };

    vm.initialize();
}]);