(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('kpiModule').controller('KPIController', kpiController);
    kpiController.$inject = ['$routeParams', 'KPIModel', 'KPIGroupModel', 'CommonModel', 'utilitiesFactory', 'historyFactory', 'lockScreenService'];

    function kpiController($routeParams, KPIModel, KPIGroupModel, CommonModel, utilitiesFactory, historyFactory, lockScreenService) {
        var vm = this;

        vm.currentPage = 1;
        vm.filter = {
            kpiGroupId: $routeParams.groupId ? parseInt($routeParams.groupId) : '',
            isActive: 'true',
            isRequiredEvaluate: ''
        };
        vm.kpiGroup = {};
        vm.kpiGroups = [];
        vm.kpis = [];
        vm.policyEnum = utilitiesFactory.policy;
        vm.pageSize = 15;
        vm.permission = 0;
        vm.routeParam = {
            kpiGroupId: $routeParams.groupId
        };

        vm.backToList = backToList;
        vm.createKPI = createKPI;
        vm.deleteKPI = deleteKPI;
        vm.getIndex = getIndex;
        vm.query = query;
        vm.readLess = readLess;
        vm.readMore = readMore;
        vm.updateKPI = updateKPI;
        vm.updateRequiredEvaluate = updateRequiredEvaluate;
        vm.updateStatusKPI = updateStatusKPI;

        utilitiesFactory.numberEntryList(vm);
        utilitiesFactory.maxLengthDescription(vm);
        initialize();

        function backToList() {
            historyFactory.back();
        }

        function createKPI() {
            KPIModel.openPopupCreateKPI(vm, vm.kpiGroup).then()['finally'](function () {
                vm.modalInstance = '';
                getKPI(vm.filter.kpiGroupId, vm.filter.isActive, vm.filter.isRequiredEvaluate);
            });
        }

        function deleteKPI(kpi) {
            KPIModel.openPopupDeleteKPI(vm, kpi);
        }

        function getIndex(index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        function getKPI(kpiGroupId, isActive, isRequiredEvaluate) {
            lockScreenService.start();
            KPIModel.getKPI(kpiGroupId, isActive, isRequiredEvaluate).then(function () {
                vm.kpis = KPIModel.kpis;
                utilitiesFactory.searchOnPage(vm, vm.kpis);
                lockScreenService.stop();
            });
        }

        function initialize() {
            KPIModel.getUserPermissionOnKPI().then(function () {
                vm.permission = KPIModel.permission;
            });

            CommonModel.getKPIGroup().then(function () {
                vm.kpiGroups = CommonModel.kpiGroups;
            });

            if (vm.routeParam.kpiGroupId) {
                vm.isViewAll = false;
                vm.filter.isActive = '';

                KPIGroupModel.getKPIGroupById(vm.routeParam.kpiGroupId).then(function () {
                    vm.kpiGroup = KPIGroupModel.kpiGroup;

                    getKPI(vm.filter.kpiGroupId, vm.filter.isActive, vm.filter.isRequiredEvaluate);
                });
            } else {
                vm.kpiGroup = {};
                vm.isViewAll = true;

                getKPI(vm.filter.kpiGroupId, vm.filter.isActive, vm.filter.isRequiredEvaluate);
            }
        }

        function query(filter) {
            vm.searchKey = '';
            vm.currentPage = 1;
            getKPI(filter.kpiGroupId, filter.isActive, filter.isRequiredEvaluate);
        }

        function readMore(item) {
            utilitiesFactory.readMore(item);
        }

        function readLess(item) {
            utilitiesFactory.readLess(item);
        }

        function updateKPI(kpi) {
            KPIModel.openPopupUpdateKPI(vm, kpi).then(function (updatedKPI) {
                kpi.Name = updatedKPI.Name;
                kpi.HasComment = updatedKPI.HasComment;
                kpi.IsRequiredComment = updatedKPI.IsRequiredComment;
                kpi.IsRequiredEvaluate = updatedKPI.IsRequiredEvaluate;
                kpi.Description = updatedKPI.Description;
                kpi.KPIGroups = updatedKPI.KPIGroups;
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }

        function updateRequiredEvaluate(kpi) {
            KPIModel.openPopupUpdateRequiredEvaluateKPI(kpi);
        }

        function updateStatusKPI(kpi) {
            KPIModel.openPopupUpdateStatusKPI(kpi);
        }
    }
})(angular);