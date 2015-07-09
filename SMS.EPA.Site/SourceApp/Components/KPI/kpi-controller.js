'use strict';
/*global kpiModule*/

kpiModule.controller('KPIController',
['KPIModel', 'KPIGroupModel', 'CommonModel', 'utilitiesFactory',
function (KPIModel, KPIGroupModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.kpis = KPIModel.kpis;
    vm.kpiGroups = KPIGroupModel.kpiGroups;

    vm.filter = {
        kpiGroupId: '',
        isActive: 'true'
    };

    utilitiesFactory.numberEntryList(vm);
    vm.pageSize = 15;
    vm.currentPage = 1;

    var getKPI = function (kpiGroupId, isActive) {
        KPIModel.getKPI(kpiGroupId, isActive).then(function () {
            vm.kpis = KPIModel.kpis;
            utilitiesFactory.searchOnPage(vm, vm.kpis);
        });
    };

    vm.initialize = function () {
        KPIGroupModel.getKPIGroup().then(function () {
            vm.kpiGroups = KPIGroupModel.kpiGroups;
        });

        getKPI(vm.filter.kpiGroupId, vm.filter.isActive);
    };

    vm.initialize();

    vm.query = function (filter) {
        getKPI(filter.kpiGroupId, filter.isActive);
    };

    /// Create KPI.
    vm.createKPI = function () {
        KPIModel.openPopupCreateKPI(vm).then()['finally'](function () {
            vm.modalInstance = '';
            getKPI(vm.filter.kpiGroupId, vm.filter.isActive);
        });
    };

    /// Update KPI.
    vm.updateKPI = function (kpi) {
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
    };

    /// Update status KPI.
    vm.updateStatusKPI = function (kpi) {
        KPIModel.openPopupUpdateStatusKPI(kpi);
    };

    /// Get index of row in table.
    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };
} ]);

kpiModule.controller('CreateKPIController',
['$modalInstance', 'KPIModel', 'KPIGroupModel',
function ($modalInstance, KPIModel, KPIGroupModel) {
    var vm = this;

    vm.currentChosenKPIGroups = [];
    vm.kpiGroups = KPIGroupModel.kpiGroups;

    vm.kpi = {
        Name: '',
        KPIGroupIds: '',
        HasComment: false,
        IsRequiredComment: false,
        IsRequiredEvaluate: false,
        Description: ''
    };

    vm.initialize = function () {
        KPIGroupModel.getKPIGroup(true).then(function () {
            vm.kpiGroups = KPIGroupModel.kpiGroups;
        });
    };

    vm.initialize();

    vm.changeHasComment = function () {
        if (vm.kpi.HasComment === false) {
            vm.kpi.IsRequiredComment = false;
        }
    };

    vm.save = function (isCreateAndContinued) {
        if (KPIModel.validateKPI(vm.kpi, vm.currentChosenKPIGroups)) {
            vm.kpi.KPIGroupIds = vm.currentChosenKPIGroups.join(',');
            KPIModel.insertKPI(vm, isCreateAndContinued, $modalInstance);
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
} ]);

kpiModule.controller('UpdateKPIController',
['$modalInstance', 'KPIModel', 'KPIGroupModel', 'kpi',
function ($modalInstance, KPIModel, KPIGroupModel, kpi) {
    var vm = this;

    vm.currentChosenKPIGroups = [];
    vm.kpiGroups = KPIGroupModel.kpiGroups;

    vm.kpi = {
        KPIId: kpi.KPIId,
        Name: kpi.Name,
        KPIGroupIds: '',
        HasComment: kpi.HasComment,
        IsRequiredComment: kpi.IsRequiredComment,
        IsRequiredEvaluate: kpi.IsRequiredEvaluate,
        Description: kpi.Description,
        KPIGroups: []
    };

    vm.initialize = function () {
        KPIGroupModel.getKPIGroup(true).then(function () {
            vm.kpiGroups = KPIGroupModel.kpiGroups;

            angular.forEach(kpi.KPIGroups, function (kpiGroup) {
                vm.currentChosenKPIGroups.push(kpiGroup.KPIGroupId);

                var index = vm.kpiGroups.map(function (e) { return e.KPIGroupId; }).indexOf(kpiGroup.KPIGroupId);
                if (index === -1) {
                    vm.kpiGroups.push(kpiGroup);
                }
            });
        });
    };

    vm.initialize();

    var getChosenKPIGroups = function () {
        var kpiGroups = [];

        angular.forEach(vm.currentChosenKPIGroups, function (kpiGroupId) {
            angular.forEach(vm.kpiGroups, function (kpiGroup) {
                if (kpiGroup.KPIGroupId === kpiGroupId) {
                    kpiGroups.push(kpiGroup);
                }
            });
        });

        return kpiGroups;
    };

    vm.changeHasComment = function () {
        if (vm.kpi.HasComment === false) {
            vm.kpi.IsRequiredComment = false;
        }
    };

    vm.save = function () {
        if (KPIModel.validateKPI(vm.kpi, vm.currentChosenKPIGroups)) {
            vm.kpi.KPIGroupIds = vm.currentChosenKPIGroups.join(',');
            vm.kpi.KPIGroups = getChosenKPIGroups();
            KPIModel.updateKPI(vm.kpi, $modalInstance);
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
} ]);