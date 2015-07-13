'use strict';
/*global targetTemplateModule*/

targetTemplateModule.controller('TargetTemplateController',
['TargetTemplateModel', 'utilitiesFactory', 'CommonModel',
function (TargetTemplateModel, utilitiesFactory, CommonModel) {
    var vm = this;

    vm.targetTemplates = TargetTemplateModel.targetTemplates;

    vm.currentPage = 1;
    vm.pageSize = 20;

    utilitiesFactory.numberEntryList(vm);

    var getTargetTemplate = function (targetTypeId, isActive) {
        TargetTemplateModel.getTargetTemplate(targetTypeId, isActive).then(function () {
            vm.targetTemplates = TargetTemplateModel.targetTemplates;
            utilitiesFactory.searchOnPage(vm, vm.targetTemplates);
        });
    };

    getTargetTemplate(null, null);

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };

    vm.create = function () {
        vm.createTargetTemplateUrl = '/TargetTemplate/Create';
    };

    vm.confirmActive = function (targetTemplate) {
        TargetTemplateModel.openPopupUpdateStatusTargetTemplate(targetTemplate);
    };

    vm.update = function (targetTemplate) {
        vm.updateTargetTemplateUrl = '/TargetTemplate/Update/' + targetTemplate.TargetTemplateId;
    };

    vm.deleteTargetTemplate = function (targetTemplate) {
        TargetTemplateModel.openPopupDeleteTargetTemplate(vm, targetTemplate);
    };
} ]);

targetTemplateModule.controller('CreateTargetTemplateController',
['$window', 'TargetTemplateModel', 'CommonModel',
function ($window, TargetTemplateModel, CommonModel) {
    var vm = this;

    vm.targetTemplate = {
        Name: '',
        TargetTypeId: '',
        IsActive: true,
        Description: ''
    };
    vm.targetTypes = CommonModel.targetTypes;

    CommonModel.getTargetType(true).then(function () {
        vm.targetTypes = CommonModel.targetTypes;
    });

    vm.save = function (targetTemplate) {
        if (TargetTemplateModel.validateTargetTemplate(targetTemplate)) {
            TargetTemplateModel.createTargetTemplate(targetTemplate).then(function () {
                $window.history.back();
            });
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };
} ]);

targetTemplateModule.controller('UpdateTargetTemplateController',
['$window', '$routeParams', 'TargetTemplateModel',
function ($window, $routeParams, TargetTemplateModel) {
    var vm = this;

    vm.targetTemplateId = parseInt($routeParams.id);
    vm.targetTemplate = TargetTemplateModel.targetTemplate;

    TargetTemplateModel.getTargetTemplateById(vm.targetTemplateId).then(function () {
        vm.targetTemplate = TargetTemplateModel.targetTemplate;
    });

    vm.save = function (targetTemplate) {
        if (TargetTemplateModel.validateTargetTemplate(targetTemplate)) {
            TargetTemplateModel.updateTargetTemplate(targetTemplate).then(function () {
                $window.history.back();
            });
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };
} ]);

targetTemplateModule.controller('GeneralInformationController',
['TargetTemplateModel', 'CommonModel',
function (TargetTemplateModel, CommonModel) {
    var vm = this;
    vm.targetTypes = CommonModel.targetTypes;

    CommonModel.getTargetType(true).then(function () {
        vm.targetTypes = CommonModel.targetTypes;
    });
} ]);

targetTemplateModule.controller('KPIListController',
['$scope', 'TargetTemplateModel',
function ($scope, TargetTemplateModel) {
    var vm = this;

    vm.targetTemplateId = $scope.updateCtrl.targetTemplateId;
    vm.kpiList = TargetTemplateModel.kpiList;

    var getKPIOfTargetTemplate = function (targetTemplateId) {
        TargetTemplateModel.getKPIOfTargetTemplate(targetTemplateId).then(function () {
            vm.kpiList = TargetTemplateModel.kpiList;
        });
    };

    getKPIOfTargetTemplate(vm.targetTemplateId);

    vm.addKPIGroup = function () {
        TargetTemplateModel.openPopupAddKPIGroup(vm, vm.targetTemplateId).then(function () {
            getKPIOfTargetTemplate(vm.targetTemplateId);
        })['finally'](function () {
            vm.modalInstance = '';
        });
    };

    vm.checkAll = function (selectedAll) {
        angular.forEach(vm.kpiList, function (kpiGroup) {
            kpiGroup.GroupSelected = selectedAll;
            vm.checkAllGroup(selectedAll, kpiGroup);
        });
    };

    vm.checkAllGroup = function (groupSelected, group) {
        angular.forEach(group.KPIs, function (kpi) {
            kpi.Selected = groupSelected;
        });
    };

    vm.scoreSpread = function (group) {
        var kpiList = group.KPIs,
            totalKPISelected = 0;

        angular.forEach(kpiList, function (kpi) {
            if (kpi.Selected) {
                totalKPISelected += 1;
            }
        });

        if (totalKPISelected > 0) {
            var averageWeight = 100 / totalKPISelected;

            angular.forEach(kpiList, function (kpi) {
                if (kpi.Selected) {
                    kpi.Weight = averageWeight;
                }
            });
        }
    };

    vm.removeKPIGroup = function (kpiGroup) {
        TargetTemplateModel.openPopupDeleteKPIGroup(vm.targetTemplateId, kpiGroup).then(function () {
            getKPIOfTargetTemplate(vm.targetTemplateId);
        });
    };
} ]);

targetTemplateModule.controller('AddedKPIGroupController',
['$modalInstance', 'TargetTemplateModel', 'targetTemplateId',
function ($modalInstance, TargetTemplateModel, targetTemplateId) {
    var vm = this;

    vm.targetTemplateId = targetTemplateId;
    vm.kpiGroups = TargetTemplateModel.kpiGroups;
    vm.selectedAll = false;

    TargetTemplateModel.getKPIGroupOfTargetTemplate(vm.targetTemplateId).then(function () {
        vm.kpiGroups = TargetTemplateModel.kpiGroups;
    });

    vm.checkAll = function (selectedAll) {
        angular.forEach(vm.kpiGroups, function (group) {
            group.Selected = selectedAll;
        });
    };

    vm.save = function (kpiGroups) {
        if (TargetTemplateModel.validateKPIGroupWeight(kpiGroups)) {
            var model = {
                targetTemplateId: vm.targetTemplateId,
                KPIGroups: kpiGroups
            };
            TargetTemplateModel.addKPIGroupWeight(model).then(function () {
                $modalInstance.close();
            });
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
} ]);