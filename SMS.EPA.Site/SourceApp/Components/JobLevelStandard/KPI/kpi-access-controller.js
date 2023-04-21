(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('JobLevelStandardKPIController', kpiController);
    kpiController.$inject = ['JobLevelStandardKPIModel', 'CommonModel', 'utilitiesFactory', 'historyFactory', 'lockScreenService'];

    function kpiController(JobLevelStandardKPIModel, CommonModel, utilitiesFactory, historyFactory, lockScreenService) {
        var vm = this;

        vm.currentPage = 1;
        vm.filter = {
            isActive: 'true'
        };
        vm.policyEnum = utilitiesFactory.policy;
        vm.pageSize = 20;
        vm.permission = 0;
        vm.jobLevelStandardKpis = [];
        vm.backToList = backToList;
        vm.createKPI = createKPI;
        vm.deleteKPI = deleteKPI;
        vm.getIndex = getIndex;
        vm.query = query;
        vm.updateKPI = updateKPI;
        vm.updateStatusKPI = updateStatusKPI;

        utilitiesFactory.numberEntryList(vm);
        utilitiesFactory.maxLengthDescription(vm);
        initialize();


        function backToList() {
            historyFactory.back();
        }

        function createKPI() {
            JobLevelStandardKPIModel.openPopupCreateKPI(vm).then()['finally'](function () {
                vm.modalInstance = '';
                getKPI(vm.filter.isActive);
            });
        }

        function deleteKPI(kpi) {
            JobLevelStandardKPIModel.openPopupDeleteKPI(vm, kpi);
        }

        function getIndex(index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        function getKPI(isActive) {
            lockScreenService.start();
            JobLevelStandardKPIModel.getJobLevelStandKPI(isActive).then(function () {
                vm.jobLevelStandardKpis = JobLevelStandardKPIModel.jobLevelStandardKpis;
                utilitiesFactory.searchOnPage(vm, vm.jobLevelStandardKpis);
                lockScreenService.stop();
            });
        }

        function initialize() {
            JobLevelStandardKPIModel.getUserPermissionOnKPI().then(function () {
                vm.permission = JobLevelStandardKPIModel.permission;
            });

            getKPI(vm.filter.isActive);
        }

        function query(filter) {
            vm.searchKey = '';
            vm.currentPage = 1;
            getKPI(vm.filter.isActive);
        }

        function updateKPI(kpi) {
            JobLevelStandardKPIModel.openPopupUpdateKPI(vm, kpi).then(function (updatedKPI) {
                kpi.Name = updatedKPI.Name;
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }

        function updateStatusKPI(kpi) {
            JobLevelStandardKPIModel.openPopupUpdateStatusKPI(kpi);
        }
    }
})(angular);