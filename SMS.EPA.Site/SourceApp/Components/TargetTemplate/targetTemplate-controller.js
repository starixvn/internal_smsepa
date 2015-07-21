'use strict';
/*global targetTemplateModule*/

targetTemplateModule.controller('TargetTemplateController',
['TargetTemplateModel', 'utilitiesFactory', 'CommonModel',
function (TargetTemplateModel, utilitiesFactory, CommonModel) {
    var vm = this;

    vm.targetTemplates = TargetTemplateModel.targetTemplates;
    vm.permission = TargetTemplateModel.permission;
    vm.filter = {
        targetTypeId: null,
        isActive: 'true'
    };

    vm.currentPage = 1;
    vm.pageSize = 20;

    utilitiesFactory.numberEntryList(vm);

    // Event read more / less text.
    utilitiesFactory.maxLengthDescription(vm);

    vm.readMore = function (item) {
        utilitiesFactory.readMore(item);
    };

    vm.readLess = function (item) {
        utilitiesFactory.readLess(item);
    };

    TargetTemplateModel.getPermissionOnTargetTemplate().then(function () {
        vm.permission = TargetTemplateModel.permission;
    });

    var getTargetTemplate = function (targetTypeId, isActive) {
        TargetTemplateModel.getTargetTemplate(targetTypeId, isActive).then(function () {
            vm.targetTemplates = TargetTemplateModel.targetTemplates;
            utilitiesFactory.searchOnPage(vm, vm.targetTemplates);
        });
    };

    getTargetTemplate(vm.filter.targetTypeId, vm.filter.isActive);

    vm.query = function (filter) {
        getTargetTemplate(filter.targetTypeId, filter.isActive);
    };

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };

    vm.create = function () {
        vm.createTargetTemplateUrl = '/TargetTemplate/Create';
    };

    vm.confirmActive = function (targetTemplate) {
        TargetTemplateModel.openPopupUpdateStatusTargetTemplate(targetTemplate);
    };

    vm.update = function (targetTemplate, viewKPIs) {
        if (!viewKPIs) {
            vm.updateTargetTemplateUrl = '/TargetTemplate/Update/' + targetTemplate.TargetTemplateId + '?viewInfo';
        }
        else {
            vm.updateTargetTemplateUrl = '/TargetTemplate/Update/' + targetTemplate.TargetTemplateId + '?viewKPIs';
        }
    };

    vm.deleteTargetTemplate = function (targetTemplate) {
        TargetTemplateModel.openPopupDeleteTargetTemplate(vm, targetTemplate);
    };

    vm.detail = function (targetTemplate) {
        vm.detailTargetTemplateUrl = '/TargetTemplate/Detail/' + targetTemplate.TargetTemplateId;
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
    vm.originalTargetTypeId = '';

    vm.dataTab = {
        information: $routeParams.viewInfo ? true : false,
        kpi: $routeParams.viewKPIs ? true : false
    };

    TargetTemplateModel.getTargetTemplateById(vm.targetTemplateId).then(function () {
        vm.targetTemplate = TargetTemplateModel.targetTemplate;
        vm.originalTargetTypeId = vm.targetTemplate.TargetTypeId;
    });

    vm.tabInfoSelect = function () {
        vm.dataTab.information = true;
        vm.dataTab.kpi = false;
    };

    vm.tabKPISelect = function () {
        vm.dataTab.information = false;
        vm.dataTab.kpi = true;
    };

    vm.save = function (targetTemplate) {
        if (TargetTemplateModel.validateTargetTemplate(targetTemplate)) {
            if (vm.targetTemplate.TargetTypeId !== vm.originalTargetTypeId) {
                TargetTemplateModel.openPopupConfirmChangeTargetType(vm).then(function () {
                    TargetTemplateModel.updateTargetTemplate(targetTemplate).then(function () {
                        vm.originalTargetTypeId = vm.targetTemplate.TargetTypeId;
                        TargetTemplateModel.changeTargetType = true;
                    });
                })["finally"](function () {
                    vm.modalInstance = '';
                });
            }
            else {
                TargetTemplateModel.updateTargetTemplate(targetTemplate);
            }
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

    $scope.$watch(function () {
        return TargetTemplateModel.changeTargetType;
    }, function (newVal) {
        if (newVal) {
            getKPIOfTargetTemplate(vm.targetTemplateId);
            TargetTemplateModel.changeTargetType = false;
        }
    });

    vm.targetTemplateId = $scope.updateCtrl.targetTemplateId;
    vm.kpiList = TargetTemplateModel.kpiList;

    var getKPIOfTargetTemplate = function (targetTemplateId) {
        TargetTemplateModel.getKPIOfTargetTemplate(targetTemplateId).then(function () {
            vm.kpiList = TargetTemplateModel.kpiList;
            TargetTemplateModel.checkSelectedAllKPI(vm, vm.kpiList);
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
            kpiGroup.Selected = selectedAll;
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
            var averageWeight = group.Weight / totalKPISelected;

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

    vm.preventEnter = function (event) {
        if (event.which === 13) {
            event.preventDefault();
            event.stopPropagation();
        }
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
        TargetTemplateModel.checkSelectedAllKPIGroup(vm, vm.kpiGroups);
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

targetTemplateModule.controller('DetailTargetTemplateController',
['$window', '$routeParams', 'TargetTemplateModel', 'utilitiesFactory',
function ($window, $routeParams, TargetTemplateModel, utilitiesFactory) {
    var vm = this;

    vm.targetTemplateId = parseInt($routeParams.id);
    vm.targetTemplate = TargetTemplateModel.targetTemplate;
    vm.kpiList = TargetTemplateModel.kpiList;

    utilitiesFactory.getDateFormat(vm);

    TargetTemplateModel.getTargetTemplateById(vm.targetTemplateId).then(function () {
        vm.targetTemplate = TargetTemplateModel.targetTemplate;
    });

    TargetTemplateModel.getKPIOfTargetTemplate(vm.targetTemplateId).then(function () {
        vm.kpiList = TargetTemplateModel.kpiList;
    });

    vm.back = function () {
        $window.history.back();
    };
}]);

targetTemplateModule.controller("ModalConfirmChangeTargetTypeController", ['$modalInstance', 
 function ($modalInstance) {
     var vm = this;

     vm.ok = function () {
         $modalInstance.close();
     };

     vm.cancel = function () {
         $modalInstance.dismiss("cancel");
     };
 }]);