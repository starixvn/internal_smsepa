'use strict';
/*exported dashboardModule*/

dashboardModule.controller('DashboardController',
['DashboardModel',
function (DashboardModel) {
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
                var finalScore, disciplineScore;
                finalScore = myEPA.FinalScore ? parseFloat(myEPA.FinalScore) : 0;

                myEPA.FinalScore = finalScore;

                if (myEPA.HasDisciplineScore) {
                    disciplineScore = myEPA.MaxDisciplineScore - myEPA.DisciplineScore;

                    if (disciplineScore < 0) {
                        disciplineScore = 0;
                    }

                    myEPA.FinalScore += disciplineScore;
                }
            });
        });
    };

    vm.initialize();
}]);