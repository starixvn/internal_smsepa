(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('targetTemplateModule').controller('KPIListController', kpiListController);
    kpiListController.$inject = ['$scope', '$translate', 'TargetTemplateModel', 'utilitiesFactory'];

    function kpiListController($scope, $translate, TargetTemplateModel, utilitiesFactory) {
        var vm = this;

        vm.targetTemplateId = $scope.updateCtrl.targetTemplateId;
        vm.kpiList = TargetTemplateModel.kpiList;

        $scope.$watch(function () { return $scope.updateCtrl.targetTemplate.MaxWeightSelf; },
            function (newVal) {
                vm.maxWeight = newVal;
            });

        function getKPIOfTargetTemplate(targetTemplateId) {
            TargetTemplateModel.getKPIOfTargetTemplate(targetTemplateId, false).then(function () {
                vm.kpiList = TargetTemplateModel.kpiList;
                TargetTemplateModel.checkSelectedAllKPI(vm, vm.kpiList);
            });
        }

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

        function getTotalWeight(kpiList) {
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
                            } else {
                                groupWeight += parseFloat(item.Weight);
                            }
                        }
                    }

                    totalWeight += groupWeight;
                }
            });

            return utilitiesFactory.parseFloat(totalWeight);
        }

        vm.save = function (targetTemplate) {
            var totalWeight = getTotalWeight(vm.kpiList);
            TargetTemplateModel.updateTargetTemplate(targetTemplate, totalWeight, vm);
        };

        function initialize() {
            $translate('TotalWeightLessThanMaxWeightWarning').then(function (message) {
                vm.totalWeightLessThanMaxWeightWarningMessage = message;
            });

            $translate('TotalWeightWarning').then(function (message) {
                vm.totalWeightWarningMessage = message;
            });

            getKPIOfTargetTemplate(vm.targetTemplateId);
        }

        initialize();
    }
})(angular);