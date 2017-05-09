(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('dashboardModule').controller('DashboardController', dashboardController);
    dashboardController.$inject =
    [
        '$location',
        'DashboardModel',
        'CommonModel',
        'utilitiesFactory',
        'lockScreenService',
        'baseUrl',
        'currentUserId'
    ];

    function dashboardController(
        $location,
        DashboardModel,
        CommonModel,
        utilitiesFactory,
        lockScreenService,
        baseUrl,
        currentUserId) {
        var vm = this;

        vm.epaStatusEnum = utilitiesFactory.epaStatus;
        vm.statusEndProcessEnum = utilitiesFactory.statusEndProcess;
        vm.targetTypeEnum = utilitiesFactory.targetType;
        vm.todoList = {};

        vm.getClassProgress = getClassProgress;
        vm.goToEvaluatorListing = goToEvaluatorListing;
        vm.goToManagementListing = goToManagementListing;
        vm.goToReviewerListing = goToReviewerListing;
        vm.selfEvaluationDetail = selfEvaluationDetail;
        vm.viewEPAResult = viewEPAResult;
        vm.viewOtherViolationDetail = viewOtherViolationDetail;
        vm.currentUserId = currentUserId;

        initialize();

        function initialize() {
            lockScreenService.start();

            DashboardModel.getToDoList().then(function () {
                vm.todoList = DashboardModel.todoList;
            });

            DashboardModel.getDashboard().then(function () {
                var dashboardInfo = DashboardModel.dashboardInfo;

                vm.employee = dashboardInfo.Employee;
                vm.employee.Avatar = baseUrl + vm.employee.Avatar;
                vm.evaluationProcesses = [];
                vm.myEPAs = dashboardInfo.MyEpaInfos;
                vm.monthlyEPAs = dashboardInfo.MonthlyEpaInfos;
                currentUserId.value = dashboardInfo.Employee.EmployeeId;

                angular.forEach(vm.myEPAs, function (myEPA) {
                    if (myEPA.IsClosed) {
                        myEPA.FinalScore = myEPA.FinalScore.length > 0 ? utilitiesFactory.parseFloat(myEPA.FinalScore) : null;
                    } else {
                        myEPA.FinalScore = null;
                        vm.evaluationProcesses.push(myEPA);
                    }
                });

                angular.forEach(vm.monthlyEPAs, function (monthlyEPA) {
                    if (monthlyEPA.IsClosed) {
                        monthlyEPA.FinalScore = monthlyEPA.FinalScore.length > 0 ? utilitiesFactory.parseFloat(monthlyEPA.FinalScore) : null;
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

        function goToEvaluatorListing(finished, isMonthly) {
            $location.url(baseUrl + 'Evaluator/Index?isMonthly=' + isMonthly + '&statusProcess=' + finished);
        }

        function goToManagementListing(finished) {
            $location.url(baseUrl + 'Management/Index?statusProcess=' + finished);
        }

        function goToReviewerListing(finished, isMonthly) {
            $location.url(baseUrl + 'Reviewer/Index?isMonthly=' + isMonthly + '&statusProcess=' + finished);
        }

        function selfEvaluationDetail(epa) {
            $location.url(baseUrl + 'SelfEvaluation/Detail/' + epa.EPAId);
        }

        function viewEPAResult(epa) {
            $location.url(baseUrl + 'EPA/Detail/' + epa.EPAId);
        }

        function viewOtherViolationDetail(epa) {
            CommonModel.openPopupViewOtherViolationDetail(epa.EPAId);
        }
    }
})(angular);