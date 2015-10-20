'use strict';
/*exported dashboardModule*/

dashboardModule.controller('DashboardController',
['DashboardModel', 'utilitiesFactory',
function (DashboardModel, utilitiesFactory) {
    var vm = this;

    vm.showScore = false;

    vm.viewEPADetail = function (epa) {
        vm.viewEPADetailUrl = '/SelfEvaluation/Detail/' + epa.EPAId;
    };

    vm.viewEPAResult = function (epa) {
        vm.viewEPAResultUrl = '/EPA/Detail/' + epa.EPAId;
    };

    vm.initialize = function () {
        DashboardModel.getDashboard().then(function () {
            var dashboardInfo = DashboardModel.dashboardInfo;

            vm.employee = dashboardInfo.Employee;
            vm.myEPAs = dashboardInfo.MyEPAs;

            angular.forEach(vm.myEPAs, function (myEPA) {
                var disciplineScore = myEPA.MaxDisciplineScore - myEPA.DisciplineScore;

                if (disciplineScore < 0) {
                    disciplineScore = 0;
                }

                myEPA.FinalScore = utilitiesFactory.parseFloat(myEPA.FinalScore);

                if (myEPA.HasDisciplineScore) {
                    myEPA.FinalScore += disciplineScore;
                }
            });
        });
    };

    vm.initialize();
}]);