'use strict';
/*global dashboardModule*/

dashboardModule.controller('DashboardController',
['DashboardModel', 'CommonModel', 'utilitiesFactory', 'baseUrl',
function (DashboardModel, CommonModel, utilitiesFactory, baseUrl) {
    var vm = this;

    vm.showScore = false;

    vm.getClassProgress = function (statusEndProcess) {
        if (statusEndProcess === 0) {
            return 'in-process';
        }
        else if (statusEndProcess === 1) {
            return 'end-process';
        }
        else {
            return 'open';
        }
    };

    vm.viewEPADetail = function (epa) {
        vm.viewEPADetailUrl = baseUrl + 'SelfEvaluation/Detail/' + epa.EPAId;
    };

    vm.viewEPAResult = function (epa) {
        vm.viewEPAResultUrl = baseUrl + 'EPA/Detail/' + epa.EPAId;
    };

    vm.viewOtherViolationDetail = function (epa) {
        CommonModel.openPopupViewOtherViolationDetail(epa.EPAId);
    };

    vm.initialize = function () {
        DashboardModel.getDashboard().then(function () {
            var dashboardInfo = DashboardModel.dashboardInfo;

            vm.employee = dashboardInfo.Employee;
            vm.employee.Avatar = baseUrl + vm.employee.Avatar;
            vm.myEPAs = dashboardInfo.MyEPAs;

            angular.forEach(vm.myEPAs, function (myEPA) {
                if (myEPA.Period.IsEndPeriod) {
                    myEPA.FinalScore = utilitiesFactory.parseFloat(myEPA.FinalScore);
                } else {
                    myEPA.FinalScore = null;
                }
            });
        });
    };

    vm.initialize();
}]);