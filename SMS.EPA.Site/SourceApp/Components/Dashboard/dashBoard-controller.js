'use strict';
/*exported dashboardModule*/

dashboardModule.controller('DashboardController',
['DashboardModel',
function (DashboardModel) {
    var vm = this;

    vm.showScore = false;

    vm.viewEPADetail = function (epa) {
        vm.viewEPADetailUrl = '/SelfEvaluation/Detail/' + epa.EPAId + '?actorUserId=' + vm.employee.EmployeeId;;
    };

    vm.viewEPAResult = function (epa) {
        vm.viewEPAResultUrl = '/EPA/Detail/' + epa.EPAId;
    };

    vm.initialize = function () {
        DashboardModel.getDashboard().then(function () {
            var dashboardInfo = DashboardModel.dashboardInfo;

            vm.employee = dashboardInfo.Employee;
            vm.myEPAs = dashboardInfo.MyEPAs;
        });
    };

    vm.initialize();
}]);