(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('JLSKPIListController', controller);
    controller.$inject = ['$scope', '$translate', 'JobLevelStandardTemplateModel', 'utilitiesFactory'];

    function controller($scope, $translate, JobLevelStandardTemplateModel, utilitiesFactory) {
        var vm = this;

        vm.templateId = $scope.updateCtrl.templateId;
        vm.kpiList = JobLevelStandardTemplateModel.kpiList;

        $scope.$watch(function () { return $scope.updateCtrl.targetTemplate && $scope.updateCtrl.targetTemplate.MaxWeightSelf; },
            function (newVal) {
                vm.maxWeight = newVal;
            });

        function getAllKPIList(id) {
            JobLevelStandardTemplateModel.getKPIListSelected(id, false).then(function () {
                vm.kpiList = JobLevelStandardTemplateModel.kpiList;
                JobLevelStandardTemplateModel.checkSelectedAllKPI(vm, vm.kpiList);
            });
        }

        vm.checkAll = function (isSelectAll) {
            angular.forEach(vm.kpiList, function (item) {
                item.IsSelected = isSelectAll;
            });
        };

        vm.check = function (item) {
            var selected = item.Selected;
            var isSelectedAll = true

            if (!selected) {
                item.Weight = '';
            }

            for (let i = 0; i < vm.kpiList.length; i++) {
                if (!vm.kpiList[i].IsSelected) {
                    isSelectedAll = false;
                    break;
                }
            }

            vm.SelectedAll = isSelectedAll;
        };

        vm.scoreSpread = function (kpiList) {
            var kpiSelected = [];

            angular.forEach(kpiList, function (kpi) {
                if (kpi.IsSelected) {
                    kpiSelected.push(kpi);
                }
            });

            if (kpiSelected.length > 0) {
                var averageWeight = 100 / kpiSelected.length;
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

        vm.save = function (template) {
            JobLevelStandardTemplateModel.updateTemplateKPI(template);
        };

        function initialize() {
            getAllKPIList(vm.templateId);
        }

        initialize();
    }
})(angular);