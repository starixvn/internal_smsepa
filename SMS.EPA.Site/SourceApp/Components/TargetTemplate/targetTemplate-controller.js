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
}]);

targetTemplateModule.controller('CreateTargetTemplateController',
['$window', '$location', 'TargetTemplateModel', 'CommonModel', 'baseUrl',
function ($window, $location, TargetTemplateModel, CommonModel, baseUrl) {
    var vm = this;
    vm.scoreRatingTypeOptions = TargetTemplateModel.scoreRatingTypes;
    vm.permission = TargetTemplateModel.permission;

    if (vm.permission !== 2) {
        $location.path(baseUrl + 'Home/AccessDenied');
    }

    vm.targetTemplate = {
        Name: '',
        TargetTypeId: '',
        ScoreRatingTypeId: '',
        IsActive: true,
        Description: ''
    };
    vm.targetTypes = CommonModel.targetTypes;

    CommonModel.getTargetType(true).then(function () {
        vm.targetTypes = CommonModel.targetTypes;
    });

    TargetTemplateModel.getScoreRatingType(null, true).then(function () {
        vm.scoreRatingTypeOptions = TargetTemplateModel.scoreRatingTypes;
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
['$scope', '$window', 'TargetTemplateModel', 'CommonModel',
function ($scope, $window, TargetTemplateModel, CommonModel) {
    var vm = this;

    vm.scoreRatingTypeOptions = TargetTemplateModel.scoreRatingTypes;
    vm.targetTypes = CommonModel.targetTypes;

    CommonModel.getTargetType(true).then(function () {
        vm.targetTypes = CommonModel.targetTypes;
    });

    TargetTemplateModel.getScoreRatingType(null, true).then(function () {
        vm.scoreRatingTypeOptions = TargetTemplateModel.scoreRatingTypes;
    });

    vm.save = function (targetTemplate) {
        if (TargetTemplateModel.validateTargetTemplate(targetTemplate)) {
            TargetTemplateModel.updateTargetTemplateBasicInformation(targetTemplate).then(function () {
                if (targetTemplate.ScoreRatingTypeId !== $scope.updateCtrl.originalScoreTypeId) {
                    $scope.updateCtrl.getTargetTemplateById(targetTemplate.TargetTemplateId);
                }
            });
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };
}]);

targetTemplateModule.controller('KPIListController',
['$scope', '$window', 'TargetTemplateModel',
function ($scope, $window, TargetTemplateModel) {
    var vm = this;

    vm.targetTemplateId = $scope.updateCtrl.targetTemplateId;

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
        var kpiSelected = [],
            totalKPISelected = 0;

        angular.forEach(group.KPIs, function (kpi) {
            if (kpi.Selected) {
                totalKPISelected += 1;
                kpiSelected.push(kpi);
            }
        });

        if (totalKPISelected > 0) {
            var averageWeight = (group.Weight / totalKPISelected).toFixed(2);

            angular.forEach(kpiSelected, function (kpi) {
                kpi.Weight = averageWeight;
            });

            if ((averageWeight * totalKPISelected) > group.Weight) {
                kpiSelected[totalKPISelected - 1].Weight = group.Weight - (averageWeight * (totalKPISelected - 1));
            }
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

    vm.save = function (targetTemplate) {
        TargetTemplateModel.updateTargetTemplate(targetTemplate);
    };

    vm.cancel = function () {
        $window.history.back();
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

    TargetTemplateModel.getKPIOfTargetTemplate(vm.targetTemplateId, true).then(function () {
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