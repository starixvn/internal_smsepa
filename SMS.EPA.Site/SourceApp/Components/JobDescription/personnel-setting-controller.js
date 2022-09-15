(function () {
    'use strict';

    angular.module('jobDescriptionModule').controller('PersonnelSettingController', personnelSettingController);
    personnelSettingController.$inject = ['$scope', 'CommonModel', 'JobDescriptionModel', 'utilitiesFactory', 'lockScreenService'];

    function personnelSettingController($scope, CommonModel, JobDescriptionModel, utilitiesFactory, lockScreenService) {
        let vm = this;

        vm.modalInstance = '';
        vm.isFirstLoad = true;
        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.filter = {
            teamIds: null,
            employee: null,
            showAll: false
        };
        vm.orgChart = [];
        vm.employeeSettingInfos = [];

        utilitiesFactory.numberEntryList(vm);

        $scope.$watch(function () { return $scope.jobDescriptionCtrl.dataTab === 2; },
            function (newVal) {
                if (newVal && vm.isFirstLoad) {
                    initialize();
                }
            }
        );

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        vm.getEmployee = function () {
            return CommonModel.searchEmployee(vm.searchKeyword, vm.filter.teamIds);
        }

        vm.unSelectEmployee = function () {
            if (vm.searchKeyword.trim() === '') {
                vm.filter.employee = null;
            }
        }

        vm.query = function () {
            var employeeId = null;
            vm.searchKey = '';
            vm.currentPage = 1;

            if (vm.filter.employee) {
                employeeId = vm.filter.employee.EmployeeId;
            }

            getEmployeeSettingInfo(vm.filter.teamIds, employeeId, vm.filter.showAll);
        }

        vm.copyEmployeeSetting = function (employee) {
            JobDescriptionModel.openPopupCopyEmployeeSetting(vm, employee).then(function (isSuccessful) {
                if (isSuccessful) {
                    vm.query();
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        };

        function getEmployeeSettingInfo(teamIds, employeeId, showAll) {
            lockScreenService.start();
            JobDescriptionModel.getEmployeeSettingInfo(teamIds, employeeId, showAll).then(function () {
                    vm.employeeSettingInfos = JobDescriptionModel.employeeSettingInfos;
                    utilitiesFactory.searchOnPage(vm, vm.employeeSettingInfos);
                    lockScreenService.stop();
                });
        }

        function initialize() {
            CommonModel.getOrgChart().then(function () {
                vm.orgChart = CommonModel.orgChart;
            });

            getEmployeeSettingInfo(vm.filter.teamIds, null, vm.filter.showAll);
            vm.isFirstLoad = false;
        }

        vm.onSelectTeam = function (teamIds) {
            vm.filter.teamIds = teamIds;
        };
    }
})();