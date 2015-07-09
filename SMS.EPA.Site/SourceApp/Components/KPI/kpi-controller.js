'use strict';
/*global kpiModule*/

kpiModule.controller('KPIController',
['KPIModel', 'KPIGroupModel', 'CommonModel', 'utilitiesFactory',
function (KPIModel, KPIGroupModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.kpis = KPIModel.kpis;
    vm.targetTypes = CommonModel.targetTypes;
    vm.kpiGroups = KPIGroupModel.kpiGroups;

    vm.filter = {
        targetTypeId: '',
        kpiGroupId: '',
        isActive: 'true'
    };

    utilitiesFactory.numberEntryList(vm);
    vm.pageSize = 15;
    vm.currentPage = 1;

    var getKPI = function (targetTypeId, kpiGroupId, isActive) {
        KPIModel.getKPI(targetTypeId, kpiGroupId, isActive).then(function () {
            vm.kpis = KPIModel.kpis;
            utilitiesFactory.searchOnPage(vm, vm.kpis);
        });
    };

    vm.initialize = function () {
        CommonModel.getTargetType().then(function () {
            vm.targetTypes = CommonModel.targetTypes;
        });

        KPIGroupModel.getKPIGroup().then(function () {
            vm.kpiGroups = KPIGroupModel.kpiGroups;
        });

        getKPI(vm.filter.targetTypeId, vm.filter.kpiGroupId, vm.filter.isActive);
    };

    vm.initialize();

    vm.getKPIGroupByTargetType = function (targetTypeId) {
        KPIGroupModel.getKPIGroup(targetTypeId).then(function () {
            vm.kpiGroups = KPIGroupModel.kpiGroups;
        });
    };

    vm.query = function (filter) {
        getKPI(filter.targetTypeId, filter.kpiGroupId, filter.isActive);
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

    /// Update required evaluate KPI.
    vm.updateRequiredEvaluate = function (kpi) {
        KPIModel.openPopupUpdateRequiredEvaluateKPI(kpi);
    };

    /// Update status KPI.
    vm.updateStatusKPI = function (kpi) {
        KPIModel.openPopupUpdateStatusKPI(kpi);
    };

    /// Delete KPI.
    vm.deleteKPI = function (kpi) {
        KPIModel.openPopupDeleteKPI(vm, kpi);
    };

    /// Get index of row in table.
    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };
} ]);

kpiModule.controller('CreateKPIController',
['$modalInstance', 'KPIModel', 'KPIGroupModel', 'CommonModel',
function ($modalInstance, KPIModel, KPIGroupModel, CommonModel) {
    var vm = this;

    vm.currentChosenKPIGroups = [];
    vm.targetTypes = CommonModel.targetTypes;
    vm.kpiGroups = KPIGroupModel.kpiGroups;

    vm.filter = {
        targetTypeId: ''
    };

    vm.kpi = {
        Name: '',
        KPIGroupIds: '',
        HasComment: false,
        IsRequiredComment: false,
        IsRequiredEvaluate: false,
        Description: ''
    };

    vm.initialize = function () {
        CommonModel.getTargetType(true).then(function () {
            vm.targetTypes = CommonModel.targetTypes;
        });

        KPIGroupModel.getKPIGroup(null, true).then(function () {
            vm.kpiGroups = KPIGroupModel.kpiGroups;
        });
    };

    vm.initialize();

    // check if kpi group exist in list kpi group
    var checkExist = function (kpiGroups, kpiGroupId) {
        var hasExist = false;

        $.each(kpiGroups, function (index, kpiGroup) {
            if (kpiGroup.KPIGroupId === kpiGroupId) {
                hasExist = true;
            }
        });

        return hasExist;
    };

    vm.getKPIGroupByTargetType = function (targetTypeId) {
        KPIGroupModel.getKPIGroup(targetTypeId, true).then(function () {
            vm.kpiGroups = KPIGroupModel.kpiGroups;

            var temps = [];
            if (vm.currentChosenKPIGroups.length > 0) {
                $.each(vm.currentChosenKPIGroups, function (index, kpiGroupId) {
                    if (checkExist(vm.kpiGroups, kpiGroupId)) {
                        temps.push(kpiGroupId);
                    }
                });
            }

            vm.currentChosenKPIGroups = temps;
            $('.select-kpi-group').trigger('chosen:updated');
        });
    };

    vm.updateTrigger = function () {
        $('.select-kpi-group').trigger('chosen:updated');
    };

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
        KPIGroupModel.getKPIGroup(null, true).then(function () {
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