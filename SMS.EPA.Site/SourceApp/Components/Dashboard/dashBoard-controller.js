(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('dashboardModule').controller('DashboardController', dashboardController);
    dashboardController.$inject = ['DashboardModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService', 'baseUrl'];

    function dashboardController(DashboardModel, CommonModel, utilitiesFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.epaStatusEnum = utilitiesFactory.epaStatus;
        vm.targetTypeEnum = utilitiesFactory.targetType;
        vm.statusEndProcessEnum = utilitiesFactory.statusEndProcess;

        vm.getClassProgress = getClassProgress;
        vm.viewEPADetail = viewEPADetail;
        vm.viewEPAResult = viewEPAResult;
        vm.viewOtherViolationDetail = viewOtherViolationDetail;

        initialize();

        function initialize() {
            lockScreenService.start();

            DashboardModel.getDashboard().then(function () {
                var dashboardInfo = DashboardModel.dashboardInfo;

                vm.employee = dashboardInfo.Employee;
                vm.employee.Avatar = baseUrl + vm.employee.Avatar;
                vm.evaluationProcesses = [];
                vm.myEPAs = dashboardInfo.MyEPAs;
                vm.monthlyEPAs = dashboardInfo.MonthlyEPAs;

                angular.forEach(vm.myEPAs, function (myEPA) {
                    if (myEPA.Period.IsEndPeriod && myEPA.FinalScore.length > 0) {
                        myEPA.FinalScore = utilitiesFactory.parseFloat(myEPA.FinalScore);
                    } else {
                        myEPA.FinalScore = null;
                        vm.evaluationProcesses.push(myEPA);
                    }
                });

                angular.forEach(vm.monthlyEPAs, function (monthlyEPA) {
                    if (monthlyEPA.Period.IsEndPeriod) {
                        monthlyEPA.FinalScore = utilitiesFactory.parseFloat(monthlyEPA.FinalScore);
                    } else {
                        monthlyEPA.FinalScore = null;
                        vm.evaluationProcesses.push(monthlyEPA);
                    }
                });

                lockScreenService.stop();
            });
        }

        function getClassProgress(statusEndProcess) {
            if (statusEndProcess === vm.statusEndProcessEnum.InProgress) {
                return 'in-process';
            } else if (statusEndProcess === vm.statusEndProcessEnum.EndProcess) {
                return 'end-process';
            } else {
                return 'open';
            }
        }

        function viewEPADetail(epa) {
            vm.viewEPADetailUrl = baseUrl + 'SelfEvaluation/Detail/' + epa.EPAId;
        }

        function viewEPAResult(epa) {
            if (epa.HasMonthlyEPA && epa.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
                vm.viewEPAResultUrl = baseUrl + 'EPA/SemiDetail/' + epa.EPAId;
            } else {
                vm.viewEPAResultUrl = baseUrl + 'EPA/Detail/' + epa.EPAId;
            }
        }

        function viewOtherViolationDetail(epa) {
            CommonModel.openPopupViewOtherViolationDetail(epa.EPAId);
        }
    }
})(angular);