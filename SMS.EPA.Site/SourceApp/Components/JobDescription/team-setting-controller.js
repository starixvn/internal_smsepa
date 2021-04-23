(function () {
    'use strict';

    angular.module('jobDescriptionModule').controller('TeamSettingController', teamSettingController);
    teamSettingController.$inject = ['$scope', 'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function teamSettingController($scope, CommonModel, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.isFirstLoad = true;
        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.teams = [];

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

        function initialize() {
            utilitiesFactory.searchOnPage(vm, vm.teams);
        }
    }
})();
