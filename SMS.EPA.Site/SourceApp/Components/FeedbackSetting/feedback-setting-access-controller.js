(function (angular) {
    'use strict';

    angular.module('feedbackSettingModule').controller('FeedbackSettingController', feedbackSettingController);
    feedbackSettingController.$inject = ['$uibModal', 'CommonModel', 'errorFactory', 'utilitiesFactory',
        'feedbackSettingFactory', 'lockScreenService'];

    function feedbackSettingController($uibModal, CommonModel, errorFactory, utilitiesFactory, feedbackSettingFactory, lockScreenService) {
        var vm = this;

        vm.periodOptions = [];
        vm.filter = {
            periodId: 0,
            departmentId: '',
            employeeId: '',
            employee: null
        };
        vm.treeDepartment = {};
        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.isHost = false;
        vm.settings = [];

        function getSetting(filter) {
            lockScreenService.start();

            feedbackSettingFactory.getSetting(filter.periodId, filter.departmentId, filter.employeeId).success(function (data) {
                vm.isHost = data.isHost;
                vm.settings = data.settings;
                utilitiesFactory.searchOnPage(vm, vm.settings);
                lockScreenService.stop();
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function initialize() {
            utilitiesFactory.numberEntryList(vm);

            CommonModel.getDepartment().then(function () {
                vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
                vm.filter.departmentId = vm.treeDepartment[0].DepartmentId;

                CommonModel.getPeriod(utilitiesFactory.targetType.SemiAnnual, true).then(function () {
                    vm.periodOptions = CommonModel.periods.sort(function (a, b) {
                        return new Date(b.StartDate) - new Date(a.StartDate);
                    });

                    if (vm.periodOptions.length > 0 && !vm.filter.periodId) {
                        vm.filter.periodId = vm.periodOptions[0].PeriodId;
                    }

                    getSetting(vm.filter);
                });
            });
        }

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.getEmployee = function () {
            return CommonModel.getAllEmployee(vm.searchEmployee.trim());
        };

        vm.unSelectEmployee = function () {
            if (vm.searchEmployee.trim() === '') {
                vm.filter.employee = null;
                vm.filter.employeeId = null;
            }
        };

        vm.query = function () {
            getSetting(vm.filter);
        };

        vm.edit = function (setting) {
            var modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/FeedbackSetting/Templates/update.html',
                controller: 'UpdateFeedbackSettingController as updateFeedbackSettingCtrl',
                backdrop: 'static',
                resolve: {
                    setting: function () {
                        return setting;
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            modalInstance.result.then(function (isSuccessful) {
                if (isSuccessful) {
                    vm.query();
                }
            })['finally'](function () {
                modalInstance = null;
            });
        };

        initialize();
    }
})(angular);