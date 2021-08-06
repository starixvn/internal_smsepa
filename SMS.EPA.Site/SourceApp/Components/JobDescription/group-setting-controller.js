(function () {
    'use strict';

    angular.module('jobDescriptionModule').controller('GroupSettingController', groupSettingController);
    groupSettingController.$inject = ['$scope', 'CommonModel', 'JobDescriptionModel', 'utilitiesFactory', 'lockScreenService'];

    function groupSettingController($scope, CommonModel, JobDescriptionModel, utilitiesFactory, lockScreenService) {
        let vm = this;

        vm.modalInstance = '';
        vm.isFirstLoad = true;
        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.groupSettingInfos = [];

        utilitiesFactory.numberEntryList(vm);

        function getGroupSettingInfo() {
            lockScreenService.start();
            JobDescriptionModel.getGroupSettingInfo().then(
                function () {
                    vm.groupSettingInfos = JobDescriptionModel.groupSettingInfos;
                    utilitiesFactory.searchOnPage(vm, vm.groupSettingInfos);
                    lockScreenService.stop();
                });
        }

        function initialize() {
            getGroupSettingInfo();
            vm.isFirstLoad = false;
        }

        $scope.$watch(function () { return $scope.jobDescriptionCtrl.dataTab === 1; },
            function (newVal) {
                if (newVal && vm.isFirstLoad) {
                    initialize();
                }
            }
        );

        vm.copyGroupSetting = function (group) {
            JobDescriptionModel.openPopupCopyGroupSetting(vm, group).then(function (isSuccessful) {
                if (isSuccessful) {
                    getGroupSettingInfo();
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        };

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };
    }
})();