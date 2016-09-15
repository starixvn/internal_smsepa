(function (angular) {
    'use strict';

    angular.module('kpiModule').controller('CreateKPIController', createKPIController);
    createKPIController.$inject = ['$modalInstance', 'KPIModel', 'CommonModel', 'kpiGroup'];

    function createKPIController($modalInstance, KPIModel, CommonModel, kpiGroup) {
        var vm = this;

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

        vm.cancel = cancel;
        vm.changeHasComment = changeHasComment;
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
            $modalInstance.dismiss('cancel');
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
})(angular);