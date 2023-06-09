(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('JLSKPIListController', controller);
    controller.$inject = ['$scope', 'JobLevelStandardTemplateModel', 'utilitiesFactory'];

    function controller($scope, TemplateModel, utilitiesFactory) {
        let vm = this;

        vm.templateId = $scope.updateCtrl.templateId;
        vm.kpis = TemplateModel.kpis;

        function getKPI(id) {
            TemplateModel.getKPI(id, false).then(function () {
                vm.kpis = TemplateModel.kpis;
                TemplateModel.checkSelectedAllKPI(vm, vm.kpis);
            });
        }

        vm.checkAll = function (isSelectAll) {
            angular.forEach(vm.kpis, function (item) {
                item.IsSelected = isSelectAll;
            });
        };

        vm.check = function (item) {
            let selected = item.Selected;
            let isSelectedAll = true;

            if (!selected) {
                item.Weight = '';
            }

            for (let i = 0; i < vm.kpis.length; i++) {
                if (!vm.kpis[i].IsSelected) {
                    isSelectedAll = false;
                    break;
                }
            }

            vm.selectedAll = isSelectedAll;
        };

        vm.scoreSpread = function (kpis) {
            const kpiSelected = [];

            angular.forEach(kpis, function (kpi) {
                if (kpi.IsSelected) {
                    kpiSelected.push(kpi);
                }
            });

            if (kpiSelected.length > 0) {
                let averageWeight = 100 / kpiSelected.length;
                averageWeight = utilitiesFactory.splitNumber(averageWeight, 4);

                angular.forEach(kpiSelected, function (kpi) {
                    kpi.Weight = averageWeight;
                });
            }
        };

        vm.preventEnter = function (event) {
            if (event.which === 13) {
                event.preventDefault();
                event.stopPropagation();
            }
        };

        vm.save = function (template) {
            TemplateModel.updateTemplateKPI(template);
        };

        function initialize() {
            getKPI(vm.templateId);
        }

        initialize();
    }
})(angular);