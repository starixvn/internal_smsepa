(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('kpiModule').controller('UpdateKPIController', updateKPIController);
    updateKPIController.$inject = ['$modalInstance', 'KPIModel', 'CommonModel', 'kpi'];

    function updateKPIController($modalInstance, KPIModel, CommonModel, kpi) {
        var vm = this;

        vm.currentChosenKPIGroups = [];
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
        vm.kpiGroups = [];

        vm.cancel = cancel;
        vm.changeHasComment = changeHasComment;
        vm.save = save;

        initialize();

        function cancel() {
            $modalInstance.dismiss('cancel');
        }

        function changeHasComment() {
            if (vm.kpi.HasComment === false) {
                vm.kpi.IsRequiredComment = false;
            }
        }

        function getChosenKPIGroups() {
            var kpiGroups = [];

            angular.forEach(vm.currentChosenKPIGroups, function (kpiGroupId) {
                angular.forEach(vm.kpiGroups, function (kpiGroup) {
                    if (kpiGroup.KPIGroupId === kpiGroupId) {
                        kpiGroups.push(kpiGroup);
                    }
                });
            });

            return kpiGroups;
        }

        function initialize() {
            CommonModel.getKPIGroup(true).then(function () {
                vm.kpiGroups = CommonModel.kpiGroups;

                angular.forEach(kpi.KPIGroups, function (kpiGroup) {
                    vm.currentChosenKPIGroups.push(kpiGroup.KPIGroupId);

                    var index = vm.kpiGroups.map(function (e) { return e.KPIGroupId; }).indexOf(kpiGroup.KPIGroupId);
                    if (index === -1) {
                        vm.kpiGroups.push(kpiGroup);
                    }
                });
            });
        }

        function save() {
            if (KPIModel.validateKPI(vm.kpi, vm.currentChosenKPIGroups)) {
                vm.kpi.KPIGroupIds = vm.currentChosenKPIGroups.join(',');
                vm.kpi.KPIGroups = getChosenKPIGroups();
                KPIModel.updateKPI(vm.kpi, $modalInstance);
            }
        }
    }
})(angular);