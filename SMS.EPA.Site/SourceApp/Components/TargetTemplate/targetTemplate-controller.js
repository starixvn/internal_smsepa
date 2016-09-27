'use strict';
/*global targetTemplateModule*/

targetTemplateModule.controller('TargetTemplateController',
['TargetTemplateModel', 'CommonModel', 'lockScreenService', 'utilitiesFactory',
function (TargetTemplateModel, CommonModel, lockScreenService, utilitiesFactory) {
    var vm = this;

    vm.targetTypes = CommonModel.targetTypes;
    vm.targetTemplates = TargetTemplateModel.targetTemplates;
    vm.permission = TargetTemplateModel.permission;

    vm.policyEnum = utilitiesFactory.policy;

    vm.filter = {
        targetTypeId: '',
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

    CommonModel.getTargetType().then(function () {
        vm.targetTypes = CommonModel.targetTypes;
    });

    function getTargetTemplate(targetTypeId, isActive) {
        lockScreenService.start();
        TargetTemplateModel.getTargetTemplate(targetTypeId, isActive).then(function () {
            vm.targetTemplates = TargetTemplateModel.targetTemplates;
            utilitiesFactory.searchOnPage(vm, vm.targetTemplates);
            lockScreenService.stop();
        });
    }

    getTargetTemplate(vm.filter.targetTypeId, vm.filter.isActive);

    vm.query = function (filter) {
        vm.searchKey = '';
        vm.currentPage = 1;
        getTargetTemplate(filter.targetTypeId, filter.isActive);
    };

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };

    vm.create = function () {
        TargetTemplateModel.openPopupCreateTargetTemplate(vm).then(function () {
            vm.modalInstance = null;
            getTargetTemplate(vm.filter.targetTypeId, vm.filter.isActive);
        });
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
}]);

targetTemplateModule.controller('UpdateTargetTemplateController',
['$routeParams', 'TargetTemplateModel',
function ($routeParams, TargetTemplateModel) {
    var vm = this;

    vm.targetTemplateId = parseInt($routeParams.id);
    vm.targetTemplate = TargetTemplateModel.targetTemplate;
    vm.originalScoreTypeId = '';

    vm.dataTab = {
        information: $routeParams.viewInfo ? true : false,
        kpi: $routeParams.viewKPIs ? true : false
    };

    vm.getTargetTemplateById = function (targetTemplateId) {
        TargetTemplateModel.getTargetTemplateById(targetTemplateId).then(function () {
            vm.targetTemplate = TargetTemplateModel.targetTemplate;
            vm.originalScoreTypeId = vm.targetTemplate.ScoreRatingTypeId;
        });
    };

    vm.getTargetTemplateById(vm.targetTemplateId);

    vm.tabInfoSelect = function () {
        vm.dataTab.information = true;
        vm.dataTab.kpi = false;
    };

    vm.tabKPISelect = function () {
        vm.dataTab.information = false;
        vm.dataTab.kpi = true;
    };
}]);

targetTemplateModule.controller('TargetTemplateGeneralInformationController',
['$scope', 'TargetTemplateModel', 'CommonModel', 'historyFactory',
function ($scope, TargetTemplateModel, CommonModel, historyFactory) {
    var vm = this;

    vm.scoreRatingTypeOptions = TargetTemplateModel.scoreRatingTypes;
    vm.targetTypes = CommonModel.targetTypes;
    vm.ratingTypes = TargetTemplateModel.ratingTypes;

    CommonModel.getTargetType(true).then(function () {
        vm.targetTypes = CommonModel.targetTypes;
    });

    TargetTemplateModel.getScoreRatingType(null, true).then(function () {
        vm.scoreRatingTypeOptions = TargetTemplateModel.scoreRatingTypes;
    });

    TargetTemplateModel.getRatingType().then(function () {
        vm.ratingTypes = TargetTemplateModel.ratingTypes;
    });

    vm.save = function (targetTemplate) {
        if (TargetTemplateModel.validateTargetTemplate(targetTemplate)) {
            TargetTemplateModel.updateTargetTemplateBasicInfo(targetTemplate, $scope.updateCtrl.originalScoreTypeId).then(function () {
                if (targetTemplate.ScoreRatingTypeId !== $scope.updateCtrl.originalScoreTypeId) {
                    $scope.updateCtrl.getTargetTemplateById(targetTemplate.TargetTemplateId);
                }
            });
        }
    };

    vm.cancel = function () {
        historyFactory.back();
    };
}]);

targetTemplateModule.controller('KPIListController',
['$scope', '$translate', 'TargetTemplateModel', 'utilitiesFactory', 'historyFactory',
function ($scope, $translate, TargetTemplateModel, utilitiesFactory, historyFactory) {
    var vm = this;

    vm.targetTemplateId = $scope.updateCtrl.targetTemplateId;

    var getMessage = function () {
        $translate('TotalWeightLessThanMaxWeightWarning').then(function (message) {
            vm.totalWeightLessThanMaxWeightWarningMessage = message;
        });
    };

    getMessage();

    $scope.$watch(function () { return $scope.updateCtrl.targetTemplate.MaxWeightSelf; },
        function (newVal) {
            vm.maxWeight = newVal;
        });

    vm.kpiList = TargetTemplateModel.kpiList;

    var getKPIOfTargetTemplate = function (targetTemplateId) {
        TargetTemplateModel.getKPIOfTargetTemplate(targetTemplateId, false).then(function () {
            vm.kpiList = TargetTemplateModel.kpiList;
            TargetTemplateModel.checkSelectedAllKPI(vm, vm.kpiList);
        });
    };

    getKPIOfTargetTemplate(vm.targetTemplateId);

    vm.addKPIGroup = function () {
        TargetTemplateModel.openPopupAddKPIGroup(vm, vm.targetTemplateId, vm.maxWeight).then(function () {
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
        var allUnchecked = true,
            allChecked = true;

        angular.forEach(group.KPIs, function (kpi) {
            kpi.Selected = groupSelected;

            if (!kpi.IsRequiredEvaluate && !group.IsSpreadKPI) {
                group.IsSpreadKPI = true;
            }

            if (!groupSelected) {
                kpi.Weight = '';
            }
        });

        angular.forEach(vm.kpiList, function (kpiGroup) {
            if (kpiGroup.Selected) {
                allUnchecked = false;
            }
            else {
                allChecked = false;
            }
        });

        if (allUnchecked) {
            vm.SelectedAll = false;
            group.IsSpreadKPI = group.IsSpreadKPIOriginal;
        }
        if (allChecked) {
            vm.SelectedAll = true;
        }
    };

    vm.check = function (group, item) {
        var selected = item.Selected,
            allGroupUnchecked = true,
            allGroupChecked = true,
            allUnchecked = true,
            allChecked = true;

        if (!selected) {
            item.Weight = '';
        }
        else if (!item.IsRequiredEvaluate) {
            group.IsSpreadKPI = true;
        }

        angular.forEach(group.KPIs, function (kpi) {
            if (kpi.Selected) {
                allGroupUnchecked = false;
            }
            else {
                allGroupChecked = false;
            }
        });

        if (allGroupUnchecked) {
            group.Selected = false;
            group.IsSpreadKPI = group.IsSpreadKPIOriginal;
        }
        if (allGroupChecked) {
            group.Selected = true;
        }

        angular.forEach(vm.kpiList, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIs, function (kpi) {
                if (kpi.Selected) {
                    allUnchecked = false;
                }
                else {
                    allChecked = false;
                }
            });
        });

        if (allUnchecked) {
            vm.SelectedAll = false;
            group.IsSpreadKPI = group.IsSpreadKPIOriginal;
        }
        if (allChecked) {
            vm.SelectedAll = true;
        }
    };

    vm.scoreSpread = function (group) {
        var kpiSelected = [];

        angular.forEach(group.KPIs, function (kpi) {
            if (kpi.Selected) {
                kpiSelected.push(kpi);
            }
        });

        if (kpiSelected.length > 0) {
            var averageWeight = group.Weight / kpiSelected.length;
            averageWeight = utilitiesFactory.splitNumber(averageWeight, 4);

            angular.forEach(kpiSelected, function (kpi) {
                kpi.Weight = averageWeight;
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

    vm.changeHasComment = function (kpi) {
        if (kpi.HasComment === false) {
            kpi.IsRequiredComment = false;
        }
    };

    var getTotalWeight = function (kpiList) {
        var totalWeight = 0;

        angular.forEach(kpiList, function (kpiGroup) {
            if (kpiGroup.KPIs && kpiGroup.KPIs.length > 0) {
                var kpiLength = kpiGroup.KPIs.length,
                    groupWeight = 0;

                for (var i = 0; i < kpiLength; i++) {
                    var item = kpiGroup.KPIs[i];

                    if (item.Selected) {
                        if (kpiGroup.IsSpreadKPI) {
                            groupWeight = kpiGroup.Weight;
                            i = kpiLength;
                        }
                        else {
                            groupWeight += item.Weight;
                        }
                    }
                }

                totalWeight += groupWeight;
            }
        });

        return utilitiesFactory.parseFloat(totalWeight);
    };

    vm.save = function (targetTemplate) {
        var totalWeight = getTotalWeight(vm.kpiList);
        TargetTemplateModel.updateTargetTemplate(targetTemplate, totalWeight, vm);
    };

    vm.cancel = function () {
        historyFactory.back();
    };
}]);

targetTemplateModule.controller('AddedKPIGroupController',
['$modalInstance', '$translate', 'TargetTemplateModel', 'targetTemplateId', 'maxWeight',
function ($modalInstance, $translate, TargetTemplateModel, targetTemplateId, maxWeight) {
    var vm = this;

    vm.targetTemplateId = targetTemplateId;
    vm.maxWeight = maxWeight;
    vm.kpiGroups = TargetTemplateModel.kpiGroups;
    vm.selectedAll = false;

    var getMessage = function () {
        $translate('TotalWeightWarning').then(function (message) {
            vm.totalWeightWarningMessage = message;
        });
    };

    getMessage();

    TargetTemplateModel.getKPIGroupOfTargetTemplate(vm.targetTemplateId).then(function () {
        vm.kpiGroups = TargetTemplateModel.kpiGroups;
        TargetTemplateModel.checkSelectedAllKPIGroup(vm, vm.kpiGroups);
    });

    vm.checkAll = function (selectedAll) {
        angular.forEach(vm.kpiGroups, function (group) {
            group.Selected = selectedAll;

            if (!selectedAll) {
                group.Weight = '';
                group.IsSpreadKPI = false;
            }
        });
    };

    vm.check = function (group) {
        var selected = group.Selected,
            allUnchecked = true,
            allChecked = true;

        if (!selected) {
            group.Weight = '';
            group.IsSpreadKPI = false;
        }

        angular.forEach(vm.kpiGroups, function (group) {
            if (group.Selected) {
                allUnchecked = false;
            }
            else {
                allChecked = false;
            }
        });

        if (allUnchecked) {
            vm.selectedAll = false;
        }

        if (allChecked) {
            vm.selectedAll = true;
        }
    };

    vm.save = function (kpiGroups) {
        if (TargetTemplateModel.validateKPIGroupWeight(kpiGroups, vm.maxWeight, vm.totalWeightWarningMessage)) {
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
}]);

targetTemplateModule.controller('DetailTargetTemplateController',
['$routeParams', 'TargetTemplateModel', 'utilitiesFactory', 'historyFactory',
function ($routeParams, TargetTemplateModel, utilitiesFactory, historyFactory) {
    var vm = this;

    vm.targetTemplateId = parseInt($routeParams.id);
    vm.targetTemplate = TargetTemplateModel.targetTemplate;
    vm.kpiList = TargetTemplateModel.kpiList;

    utilitiesFactory.getDateFormat(vm);

    TargetTemplateModel.getTargetTemplateById(vm.targetTemplateId).then(function () {
        vm.targetTemplate = TargetTemplateModel.targetTemplate;
    });

    TargetTemplateModel.getKPIOfTargetTemplate(vm.targetTemplateId, true).then(function () {
        vm.kpiList = TargetTemplateModel.kpiList;
    });

    vm.back = function () {
        historyFactory.back();
    };
}]);

targetTemplateModule.controller("ModalConfirmChangeTargetTypeController",
['$modalInstance',
 function ($modalInstance) {
     var vm = this;

     vm.ok = function () {
         $modalInstance.close();
     };

     vm.cancel = function () {
         $modalInstance.dismiss("cancel");
     };
 }]);