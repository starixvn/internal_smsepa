(function (angular) {
    'use strict';

    angular.module('feedbackReportModule').controller('FeedbackReportController', feedbackReportController);
    feedbackReportController.$inject = ['feedbackReportFactory', 'errorFactory', 'utilitiesFactory', 'lockScreenService',
        'CommonModel'];

    function feedbackReportController(feedbackReportFactory, errorFactory, utilitiesFactory, lockScreenService,
        CommonModel) {
        var vm = this;

        vm.periodOptions = [];
        vm.filter = {
            periodId: '',
            departmentId: '',
            employee: null
        };
        vm.pageOption = utilitiesFactory.pageOption;
        vm.treeDepartment = {};
        vm.feedbacks = [];

        function getData() {
            lockScreenService.start();
            var criteria = {
                periodId: vm.filter.periodId,
                departmentId: vm.filter.departmentId,
                employeeId: vm.filter.employee ? vm.filter.employee.EmployeeId : null
            };
            feedbackReportFactory.getData(criteria).success(function (feedbacks) {
                vm.feedbacks = feedbacks;
                lockScreenService.stop();
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function initialize() {
            CommonModel.getDepartment().then(function () {
                vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
                vm.filter.departmentId = vm.treeDepartment[0].DepartmentId;
            });

            CommonModel.getPeriod(utilitiesFactory.targetType.SemiAnnual, true).then(function () {
                vm.periodOptions = CommonModel.periods.sort(function (a, b) {
                    return new Date(b.StartDate) - new Date(a.StartDate);
                });

                if (vm.periodOptions.length > 0 && !vm.filter.periodId) {
                    vm.filter.periodId = vm.periodOptions[0].PeriodId;
                }

                getData();
            });
        }

        vm.getEmployee = function () {
            return CommonModel.getAllEmployee(vm.searchEmployee.trim());
        };

        vm.unSelectEmployee = function () {
            if (vm.searchEmployee.trim() === '') {
                vm.filter.employee = null;
            }
        };

        vm.query = function () {
            getData();
        };

        vm.exportExcel = function () {
            var index = vm.periodOptions.findIndex(x => x.PeriodId === vm.filter.periodId);
            var period = vm.periodOptions[index].Name;
            var criteria = {
                periodId: vm.filter.periodId,
                period: period,
                departmentId: vm.filter.departmentId,
                employeeId: vm.filter.employee ? vm.filter.employee.EmployeeId : null
            };
            feedbackReportFactory.exportExcel(criteria).success(function (data) {
                var blob = new Blob([data]);
                var url = window.URL.createObjectURL(blob);
                var link = document.createElement('a');
                link.href = url;
                link.setAttribute('download', '360 Degree EPA Report.xlsx');
                document.body.appendChild(link);
                link.click();
                link.parentNode.removeChild(link);
                window.URL.revokeObjectURL(url);

                lockScreenService.stop();
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        initialize();
    }
})(angular);