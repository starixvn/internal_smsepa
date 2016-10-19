(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('periodModule').controller('ProcessSettingPeriodController', processSettingPeriodController);
    processSettingPeriodController.$inject = ['$scope', 'PeriodModel', 'CommonModel', 'lockScreenService'];

    function processSettingPeriodController($scope, PeriodModel, CommonModel, lockScreenService) {
        var vm = this;

        vm.isFirstLoad = true;
        vm.period = {};
        vm.processSettings = [];
        vm.remindDay = CommonModel.remindDay;

        function getProcessSetting(periodId) {
            lockScreenService.start();

            PeriodModel.getProcessSettingByPeriodId(periodId).then(function () {
                vm.processSettings = PeriodModel.processSettings;
                vm.isFirstLoad = false;
                lockScreenService.stop();
            });
        }

        $scope.$watch(function () { return $scope.updateCtrl.period && $scope.updateCtrl.dataTab.processSetting; },
            function (newVal) {
                if (newVal) {
                    vm.period = $scope.updateCtrl.period;

                    if (vm.isFirstLoad && vm.period.PeriodId) {
                        getProcessSetting(vm.period.PeriodId);
                    }
                }
            });

        vm.save = function () {
            var periodProcessSetting = {
                PeriodId: vm.period.PeriodId,
                Processes: vm.processSettings
            };

            PeriodModel.updatePeriodProcessSetting(periodProcessSetting);
        };
    }
})(angular);