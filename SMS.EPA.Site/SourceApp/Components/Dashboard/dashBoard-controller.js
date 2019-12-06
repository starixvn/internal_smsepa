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
        'baseUrl'
    ];

    function dashboardController(
        $location,
        DashboardModel,
        CommonModel,
        utilitiesFactory,
        lockScreenService,
        baseUrl) {
        var vm = this;

        vm.epaStatusEnum = utilitiesFactory.epaStatus;
        vm.processStatus = utilitiesFactory.processStatus;
        vm.targetTypeEnum = utilitiesFactory.targetType;
        vm.todoList = {};

        vm.getClassProgress = getClassProgress;
        vm.goToEvaluatorListing = goToEvaluatorListing;
        vm.goToManagementListing = goToManagementListing;
        vm.goToReviewerListing = goToReviewerListing;
        vm.selfEvaluationDetail = selfEvaluationDetail;
        vm.viewEPAResult = viewEPAResult;

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

        function getClassProgress(processStatus) {
            if (processStatus === vm.processStatus.InProgress) {
                return 'in-process';
            } else if (processStatus === vm.processStatus.Closed) {
                return 'end-process';
            } else {
                return 'open';
            }
        }

        function goToEvaluatorListing(periodId, finished, isMonthly, event) {
            var params = {
                periodId: periodId,
                isMonthly: isMonthly,
                statusProcess: finished
            };

            utilitiesFactory.openLinkWithParams(event, 'Evaluator/Index', params);
        }

        function goToManagementListing(periodId, finished, event) {
            var params = {
                periodId: periodId,
                statusProcess: finished
            };

            utilitiesFactory.openLinkWithParams(event, 'Management/Index', params);
        }

        function goToReviewerListing(periodId, finished, isMonthly, event) {
            var params = {
                periodId: periodId,
                isMonthly: isMonthly,
                statusProcess: finished
            };

            utilitiesFactory.openLinkWithParams(event, 'Reviewer/Index', params);
        }

        function selfEvaluationDetail(epaId, event) {
            utilitiesFactory.openLink(event, 'SelfEvaluation/Detail/' + epaId);
        }

        function viewEPAResult(epaId, event) {
            utilitiesFactory.openLink(event, 'EPA/Detail/' + epaId);
        }
    }
})(angular);