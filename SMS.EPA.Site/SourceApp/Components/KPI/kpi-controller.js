'use strict';

(function (angular) {
    angular.module('kpiModule').controller('KPIController', KPIController);
    KPIController.$inject = ['$routeParams', 'KPIModel', 'KPIGroupModel', 'CommonModel', 'utilitiesFactory', 'historyFactory'];

    angular.module('kpiModule').controller('CreateKPIController', CreateKPIController);
    CreateKPIController.$inject = ['$modalInstance', 'KPIModel', 'CommonModel', 'kpiGroup'];

    angular.module('kpiModule').controller('UpdateKPIController', UpdateKPIController);
    UpdateKPIController.$inject = ['$modalInstance', 'KPIModel', 'CommonModel', 'kpi'];

    function KPIController($routeParams, KPIModel, KPIGroupModel, CommonModel, utilitiesFactory, historyFactory) {
        var vm = this;

        vm.backToList = backToList;
        vm.createKPI = createKPI;
        vm.currentPage = 1;
        vm.deleteKPI = deleteKPI;
        vm.filter = {
            kpiGroupId: $routeParams.groupId ? parseInt($routeParams.groupId) : '',
            isActive: 'true',
            isRequiredEvaluate: ''
        };
        vm.getIndex = getIndex;
        vm.kpiGroup = {};
        vm.kpiGroups = [];
        vm.kpis = [];
        vm.pageSize = 15;
        vm.permission = 0;
        vm.policyEnum = utilitiesFactory.policy;
        vm.query = query;
        vm.readLess = readLess;
        vm.readMore = readMore;
        vm.routeParam = {
            kpiGroupId: $routeParams.groupId
        };
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
            KPIModel.getKPI(kpiGroupId, isActive, isRequiredEvaluate).then(function () {
                vm.kpis = KPIModel.kpis;
                utilitiesFactory.searchOnPage(vm, vm.kpis);
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

    function CreateKPIController($modalInstance, KPIModel, CommonModel, kpiGroup) {
        var vm = this;

        vm.cancel = cancel;
        vm.changeHasComment = changeHasComment;
        vm.currentChosenKPIGroups = [];
        vm.kpiGroups = [];
        vm.kpi = {
            Name: '',
            KPIGroupIds: '',
            HasComment: true,
            IsRequiredComment: true,
            IsRequiredEvaluate: true,
            Description: ''
        };
        vm.isDisable = false;
        vm.save = save;

        initialize();

        function initialize() {
            if (kpiGroup.KPIGroupId) {
                vm.currentChosenKPIGroups.push(kpiGroup.KPIGroupId);
                vm.isDisable = true;
            }

            CommonModel.getKPIGroup(true).then(function () {
                vm.kpiGroups = CommonModel.kpiGroups;

                if (kpiGroup.KPIGroupId) {
                    var index = vm.kpiGroups.map(function (e) { return e.KPIGroupId; }).indexOf(kpiGroup.KPIGroupId);
                    if (index === -1) {
                        vm.kpiGroups.push(kpiGroup);
                    }
                }
            });
        }

        function cancel() {
            $modalInstance.dismiss("cancel");
        }

        function changeHasComment() {
            if (vm.kpi.HasComment === false) {
                vm.kpi.IsRequiredComment = false;
            }
        }

        function save(isCreateAndContinued) {
            if (KPIModel.validateKPI(vm.kpi, vm.currentChosenKPIGroups)) {
                vm.kpi.KPIGroupIds = vm.currentChosenKPIGroups.join(',');
                KPIModel.createKPI(vm, isCreateAndContinued, kpiGroup, $modalInstance);
            }
        }
    }

    function UpdateKPIController($modalInstance, KPIModel, CommonModel, kpi) {
        var vm = this;

        vm.cancel = cancel;
        vm.changeHasComment = changeHasComment;
        vm.currentChosenKPIGroups = [];
        vm.kpi = {
            KPIId: kpi.KPIId,
            Name: kpi.Name,
            KPIGroupIds: '',
            HasComment: kpi.HasComment,
            IsRequiredComment: kpi.IsRequiredComment,
            IsRequiredEvaluate: kpi.IsRequiredEvaluate,
            IsActive: kpi.IsActive,
            Description: kpi.Description,
            KPIGroups: []
        };
        vm.kpiGroups = [];
        vm.save = save;

        initialize();

        function cancel() {
            $modalInstance.dismiss("cancel");
        }

        function changeHasComment() {
            if (vm.kpi.HasComment === false) {
                vm.kpi.IsRequiredComment = false;
            }
        }

        function getChosenKPIGroups() {
            var kpiGroups = [];

            angular.forEach(vm.currentChosenKPIGroups, function (kpiGroupId) {
                angular.forEach(vm.kpiGroups, function (kpiGroup) {
                    if (kpiGroup.KPIGroupId === kpiGroupId) {
                        kpiGroups.push(kpiGroup);
                    }
                });
            });

            return kpiGroups;
        }

        function initialize() {
            CommonModel.getKPIGroup(true).then(function () {
                vm.kpiGroups = CommonModel.kpiGroups;

                angular.forEach(kpi.KPIGroups, function (kpiGroup) {
                    vm.currentChosenKPIGroups.push(kpiGroup.KPIGroupId);

                    var index = vm.kpiGroups.map(function (e) { return e.KPIGroupId; }).indexOf(kpiGroup.KPIGroupId);
                    if (index === -1) {
                        vm.kpiGroups.push(kpiGroup);
                    }
                });
            });
        }

        function save() {
            if (KPIModel.validateKPI(vm.kpi, vm.currentChosenKPIGroups)) {
                vm.kpi.KPIGroupIds = vm.currentChosenKPIGroups.join(',');
                vm.kpi.KPIGroups = getChosenKPIGroups();
                KPIModel.updateKPI(vm.kpi, $modalInstance);
            }
        }
    }
})(angular);