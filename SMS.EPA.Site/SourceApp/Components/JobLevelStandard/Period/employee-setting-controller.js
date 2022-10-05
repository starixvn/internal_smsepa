(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('JobLevelStandardPeriodEmployeeSettingController', employeeSettingController);
    employeeSettingController.$inject = ['$scope', 'JobLevelStandardPeriodModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function employeeSettingController($scope, PeriodModel, CommonModel, utilitiesFactory, lockScreenService) {
        let vm = this;

        vm.period = null;
        vm.employeeSettings = [];
        vm.isFirstLoad = true;
        vm.selectedAll = false;
        vm.orgChart = [];

        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.permission = PeriodModel.permission;
        vm.policy = utilitiesFactory.policy;
        vm.assessmentStatus = utilitiesFactory.jobLevelAssessmentStatus;
        vm.assessmentStatusOptions = [
            { StatusId: vm.assessmentStatus.Pending, Name: 'Pending' },
            { StatusId: vm.assessmentStatus.Verified, Name: 'Verified' },
            { StatusId: vm.assessmentStatus.Approved, Name: 'Approved' }
        ];
        utilitiesFactory.numberEntryList(vm);
        utilitiesFactory.getDateFormat(vm);

        vm.filter = {
            periodId: 0,
            teamIds: null,
            isLocal: '',
            isExcluded: false,
            statusId: ''
        };

        function getEmployeeSettings(filter) {
            lockScreenService.start();

            PeriodModel.getEmployeeSettings(filter)
                .then(function () {
                    vm.employeeSettings = PeriodModel.employeeSettings;
                    utilitiesFactory.searchOnPage(vm, vm.employeeSettings);
                    vm.isFirstLoad = false;
                    lockScreenService.stop();
                });
        }

        $scope.$watch(function () { return $scope.editCtrl.dataTab === 3 && $scope.editCtrl.period; },
            function (newVal) {
                if (newVal && vm.isFirstLoad) {
                    vm.permission = $scope.editCtrl.permission;
                    vm.period = $scope.editCtrl.period;
                    vm.filter.periodId = vm.period.PeriodId;

                    CommonModel.getOrgChart().then(function () {
                        vm.orgChart = CommonModel.orgChart;
                    });

                    getEmployeeSettings(vm.filter);
                }
            });

        vm.query = function () {
            getEmployeeSettings(vm.filter);
        };

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.addEmployee = function () {
            if (vm.permission === vm.policy.Host) {
                PeriodModel.openPopupAddEmployeeSetting(vm).then(function () {
                    vm.modalInstance = null;
                    getEmployeeSettings(vm.filter);
                });
            }
        };

        vm.edit = function (employee) {
            if (vm.permission === vm.policy.Host) {
                PeriodModel.openPopupEditEmployeeSetting(vm, [employee]).then(function () {
                    vm.modalInstance = null;
                    getEmployeeSettings(vm.filter);
                });
            }
        };

        vm.editStatus = function (employee) {
            if (vm.permission === vm.policy.Host) {
                PeriodModel.openPopupEditStatusEmployeeSetting(vm, vm.filter.periodId, employee);
            }
        };

        vm.onSelectTeam = function (teamIds) {
            vm.filter.teamIds = teamIds;
        };

        vm.checkAll = function (selectedAll) {
            for (let employee of vm.filteredItems) {
                employee.Selected = selectedAll;
            }

            for (let employee of vm.employeeSettings) {
                employee.Selected = selectedAll;
            }
        };

        vm.syncSelectedAll = function (employee) {
            const index = vm.employeeSettings.findIndex(x => x.EmployeeId === employee.EmployeeId);
            vm.employeeSettings[index].Selected = employee.Selected;

            const selectedItems = vm.filteredItems.filter(x => x.Selected);
            if (selectedItems.length < vm.filteredItems.length) {
                vm.selectedAll = false;
            } else {
                vm.selectedAll = true;
            }
        };

        vm.editMultiple = function () {
            const employees = vm.employeeSettings.filter(x => x.Selected);

            PeriodModel.editMultipleEmployeeSetting(vm, employees).then(function () {
                vm.modalInstance = null;
                getEmployeeSettings(vm.filter);
            });
        };
    }
})(angular);