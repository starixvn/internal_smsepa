(function () {
    'use strict';

    angular.module('jobDescriptionModule').controller('PersonnelSettingController', personnelSettingController);
    personnelSettingController.$inject = ['$scope', 'CommonModel', 'JobDescriptionModel', 'utilitiesFactory', 'lockScreenService'];

    function personnelSettingController($scope, CommonModel, JobDescriptionModel, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.isFirstLoad = true;
        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.filter = {
            groupId: null,
            employee: null,
            showAll: false
        };
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
            var groupId = vm.filter.groupId === null ? null : Math.abs(vm.filter.groupId);
            return CommonModel.getAllEmployee(vm.searchEmployee, groupId);
        }

        vm.unSelectEmployee = function () {
            if (vm.searchEmployee.trim() === '') {
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

            getEmployeeSettingInfo(vm.filter.groupId, employeeId, vm.filter.showAll);
        }

        vm.edit = function (event, employee) {
            utilitiesFactory.handleOpenLink(event, 'JobDescription/EditEmployee/' + employee.EmployeeId);
        }

        function getEmployeeSettingInfo(groupId, employeeId, showAll) {
            lockScreenService.start();
            JobDescriptionModel.getEmployeeSettingInfo(groupId, employeeId, showAll).then(
                function () {
                    vm.employeeSettingInfos = JobDescriptionModel.employeeSettingInfos;
                    utilitiesFactory.searchOnPage(vm, vm.employeeSettingInfos);
                    vm.isFirstLoad = false;
                    lockScreenService.stop();
                });
        }

        function initialize() {
            CommonModel.getDepartment(null, true).then(function () {
                vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
            });

            getEmployeeSettingInfo(vm.filter.groupId, null, vm.filter.showAll);
        }
    }
})();