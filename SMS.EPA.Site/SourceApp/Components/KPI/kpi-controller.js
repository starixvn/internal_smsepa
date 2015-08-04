'use strict';
/*global kpiModule*/

kpiModule.controller('KPIController',
['$window', '$routeParams', 'KPIModel', 'KPIGroupModel', 'CommonModel', 'utilitiesFactory',
function ($window, $routeParams, KPIModel, KPIGroupModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.kpis = KPIModel.kpis;
    vm.targetTypes = CommonModel.targetTypes;
    vm.kpiGroups = CommonModel.kpiGroups;
    vm.kpiGroup = KPIGroupModel.kpiGroup;
    vm.permission = KPIModel.permission;

    vm.routeParam = {
        kpiGroupId: $routeParams.groupId
    };

    vm.filter = {
        targetTypeId: '',
        kpiGroupId: vm.routeParam.kpiGroupId ? parseInt(vm.routeParam.kpiGroupId) : '',
        isActive: 'true',
        isRequiredEvaluate : ''
    };

    utilitiesFactory.numberEntryList(vm);
    vm.pageSize = 15;
    vm.currentPage = 1;

    var getKPI = function (targetTypeId, kpiGroupId, isActive, isRequiredEvaluate) {
        KPIModel.getKPI(targetTypeId, kpiGroupId, isActive, isRequiredEvaluate).then(function () {
            vm.kpis = KPIModel.kpis;
            utilitiesFactory.searchOnPage(vm, vm.kpis);
        });
    };

    vm.backToList = function () {
        $window.history.back();
    };

    vm.initialize = function () {
        KPIModel.getUserPermissionOnKPI().then(function () {
            vm.permission = KPIModel.permission;
        });

        CommonModel.getTargetType().then(function () {
            vm.targetTypes = CommonModel.targetTypes;
        });

        CommonModel.getKPIGroup().then(function () {
            vm.kpiGroups = CommonModel.kpiGroups;
        });

        if (vm.routeParam.kpiGroupId) {
            vm.isViewAll = false;
            vm.filter.isActive = '';

            KPIGroupModel.getKPIGroupById(vm.routeParam.kpiGroupId).then(function () {
                vm.kpiGroup = KPIGroupModel.kpiGroup;
                vm.filter.targetTypeId = vm.kpiGroup.TargetType.TargetTypeId;

                getKPI(vm.filter.targetTypeId, vm.filter.kpiGroupId, vm.filter.isActive, vm.filter.isRequiredEvaluate);
            });
        }
        else {
            vm.kpiGroup = {};
            vm.isViewAll = true;
            vm.filter.targetTypeId = '';

            getKPI(vm.filter.targetTypeId, vm.filter.kpiGroupId, vm.filter.isActive, vm.filter.isRequiredEvaluate);
        }
    };

    vm.initialize();

    vm.getKPIGroupByTargetType = function (targetTypeId) {
        CommonModel.getKPIGroup(targetTypeId).then(function () {
            vm.kpiGroups = CommonModel.kpiGroups;

            if (vm.filter.kpiGroupId) {
                var index = vm.kpiGroups.map(function (e) { return e.KPIGroupId; }).indexOf(vm.filter.kpiGroupId);
                if (index === -1) {
                    vm.filter.kpiGroupId = '';
                }
            }
        });
    };

    vm.query = function (filter) {
        getKPI(filter.targetTypeId, filter.kpiGroupId, filter.isActive, filter.isRequiredEvaluate);
    };

    /// Create KPI.
    vm.createKPI = function () {
        KPIModel.openPopupCreateKPI(vm, vm.kpiGroup).then()['finally'](function () {
            vm.modalInstance = '';
            getKPI(vm.filter.targetTypeId, vm.filter.kpiGroupId, vm.filter.isActive, vm.filter.isRequiredEvaluate);
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

    // Event read more / less text.
    utilitiesFactory.maxLengthDescription(vm);

    vm.readMore = function (item) {
        utilitiesFactory.readMore(item);
    };

    vm.readLess = function (item) {
        utilitiesFactory.readLess(item);
    };

    /// Get index of row in table.
    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };
} ]);

kpiModule.controller('CreateKPIController',
['$modalInstance', 'KPIModel', 'CommonModel', 'kpiGroup',
function ($modalInstance, KPIModel, CommonModel, kpiGroup) {
    var vm = this;

    vm.currentChosenKPIGroups = [];
    vm.targetTypes = CommonModel.targetTypes;
    vm.kpiGroups = CommonModel.kpiGroups;
    vm.isDisable = false;

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
        if (kpiGroup.KPIGroupId) {
            vm.filter.targetTypeId = kpiGroup.TargetType.TargetTypeId;
            vm.currentChosenKPIGroups.push(kpiGroup.KPIGroupId);
            vm.isDisable = true;
        }

        CommonModel.getTargetType(true).then(function () {
            vm.targetTypes = CommonModel.targetTypes;

            if (kpiGroup.KPIGroupId) {
                var index = vm.targetTypes.map(function (e) { return e.TargetTypeId; }).indexOf(kpiGroup.TargetType.TargetTypeId);
                if (index === -1) {
                    vm.targetTypes.push(kpiGroup.TargetType);
                }
            }
        });

        CommonModel.getKPIGroup(null, true).then(function () {
            vm.kpiGroups = CommonModel.kpiGroups;

            if (kpiGroup.KPIGroupId) {
                var index = vm.kpiGroups.map(function (e) { return e.KPIGroupId; }).indexOf(kpiGroup.KPIGroupId);
                if (index === -1) {
                    vm.kpiGroups.push(kpiGroup);
                }
            }
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
        CommonModel.getKPIGroup(targetTypeId, true).then(function () {
            vm.kpiGroups = CommonModel.kpiGroups;

            var temps = [];
            if (vm.currentChosenKPIGroups.length > 0) {
                $.each(vm.currentChosenKPIGroups, function (index, kpiGroupId) {
                    if (checkExist(vm.kpiGroups, kpiGroupId)) {
                        temps.push(kpiGroupId);
                    }
                });
            }

            vm.currentChosenKPIGroups = temps;
        });
    };

    vm.changeHasComment = function () {
        if (vm.kpi.HasComment === false) {
            vm.kpi.IsRequiredComment = false;
        }
    };

    vm.save = function (isCreateAndContinued) {
        if (KPIModel.validateKPI(vm.kpi, vm.currentChosenKPIGroups)) {
            vm.kpi.KPIGroupIds = vm.currentChosenKPIGroups.join(',');
            KPIModel.createKPI(vm, isCreateAndContinued, kpiGroup, $modalInstance);
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
} ]);

kpiModule.controller('UpdateKPIController',
['$modalInstance', 'KPIModel', 'CommonModel', 'kpi',
function ($modalInstance, KPIModel, CommonModel, kpi) {
    var vm = this;

    vm.currentChosenKPIGroups = [];
    vm.kpiGroups = CommonModel.kpiGroups;

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

    vm.initialize = function () {
        CommonModel.getKPIGroup(null, true).then(function () {
            vm.kpiGroups = CommonModel.kpiGroups;

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