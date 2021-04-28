(function () {
    'use strict';

    angular.module('jobDescriptionModule').controller('GroupSettingController', groupSettingController);
    groupSettingController.$inject = ['$scope', 'CommonModel', 'JobDescriptionModel', 'utilitiesFactory', 'lockScreenService'];

    function groupSettingController($scope, CommonModel, JobDescriptionModel, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.isFirstLoad = true;
        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.groupSettingInfos = [];

        utilitiesFactory.numberEntryList(vm);

        $scope.$watch(function () { return $scope.jobDescriptionCtrl.dataTab === 1; },
            function (newVal) {
                if (newVal && vm.isFirstLoad) {
                    initialize();
                }
            }
        );

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        vm.edit = function (event, group) {
            utilitiesFactory.handleOpenLink(event, 'JobDescription/EditGroup/' + group.GroupId);
        }

        function initialize() {
            lockScreenService.start();
            JobDescriptionModel.getGroupSettingInfo().then(
                function () {
                    vm.groupSettingInfos = JobDescriptionModel.groupSettingInfos;
                    utilitiesFactory.searchOnPage(vm, vm.groupSettingInfos);
                    vm.isFirstLoad = false;
                    lockScreenService.stop();
                });
        }
    }
})();