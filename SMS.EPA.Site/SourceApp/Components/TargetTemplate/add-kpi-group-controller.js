(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('targetTemplateModule').controller('AddedKPIGroupController', addedKPIGroupController);
    addedKPIGroupController.$inject = ['$uibModalInstance', '$translate', 'TargetTemplateModel', 'targetTemplateId', 'maxWeight'];

    function addedKPIGroupController($uibModalInstance, $translate, TargetTemplateModel, targetTemplateId, maxWeight) {
        var vm = this;

        vm.targetTemplateId = targetTemplateId;
        vm.maxWeight = maxWeight;
        vm.kpiGroups = [];
        vm.selectedAll = false;

        vm.cancel = function () {
            $uibModalInstance.dismiss("cancel");
        };

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
                } else {
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
                    $uibModalInstance.close();
                });
            }
        };

        function initialize() {
            $translate('TotalWeightWarning').then(function (message) {
                vm.totalWeightWarningMessage = message;
            });

            TargetTemplateModel.getKPIGroupOfTargetTemplate(vm.targetTemplateId).then(function () {
                vm.kpiGroups = TargetTemplateModel.kpiGroups;
                TargetTemplateModel.checkSelectedAllKPIGroup(vm, vm.kpiGroups);
            });
        }

        initialize();
    }
})(angular);