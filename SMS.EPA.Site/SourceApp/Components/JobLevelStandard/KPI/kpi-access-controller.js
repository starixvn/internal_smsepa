(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('JobLevelStandardKPIController', kpiController);
    kpiController.$inject = ['JobLevelStandardKPIModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function kpiController(KPIModel, CommonModel, utilitiesFactory, lockScreenService) {
        let vm = this;

        vm.currentPage = 1;
        vm.filter = {
            isActive: 'true'
        };
        vm.policyEnum = utilitiesFactory.policy;
        vm.pageSize = 20;
        vm.permission = 0;
        vm.kpis = [];

        vm.createKPI = createKPI;
        vm.deleteKPI = deleteKPI;
        vm.getIndex = getIndex;
        vm.query = query;
        vm.updateKPI = updateKPI;
        vm.updateStatusKPI = updateStatusKPI;

        utilitiesFactory.numberEntryList(vm);
        initialize();

        function createKPI() {
            KPIModel.openPopupCreateKPI(vm).then(function () {
                getKPI(vm.filter.isActive);
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }

        function deleteKPI(kpi) {
            KPIModel.openPopupDeleteKPI(vm, kpi);
        }

        function getIndex(index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        function getKPI(isActive) {
            lockScreenService.start();
            KPIModel.getJobLevelStandKPI(isActive).then(function () {
                vm.kpis = KPIModel.kpis;
                utilitiesFactory.searchOnPage(vm, vm.kpis);
                lockScreenService.stop();
            });
        }

        function initialize() {
            KPIModel.getUserPermissionOnKPI().then(function () {
                vm.permission = KPIModel.permission;
            });

            getKPI(vm.filter.isActive);
        }

        function query(filter) {
            vm.searchKey = '';
            vm.currentPage = 1;
            getKPI(vm.filter.isActive);
        }

        function updateKPI(kpi) {
            KPIModel.openPopupUpdateKPI(vm, kpi).then(function (updatedKPI) {
                kpi.Name = updatedKPI.Name;
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }

        function updateStatusKPI(kpi) {
            KPIModel.openPopupUpdateStatusKPI(kpi);
        }
    }
})(angular);