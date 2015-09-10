'use strict';
/*global epaModule*/

epaModule.controller('RatingByKPIController',
['$window', 'RatingByKPIModel', 'CommonModel',
function ($window, RatingByKPIModel, CommonModel) {
    var vm = this;

    vm.kpis = RatingByKPIModel.kpis;
    vm.memberEPAs = RatingByKPIModel.memberEPAs;
    vm.kpiSelected = null;
    vm.maxStar = 5;

    vm.filter = {
        targetTemplateId: '',
        kpiId: '',
        groupId: ''
    };

    vm.getKPIByTargetTemplate = function (targetTemplateId) {
        RatingByKPIModel.getKPIByTargetTemplate(targetTemplateId).then(function () {
            vm.kpis = RatingByKPIModel.kpis;
        });
    };

    vm.viewKPIInfo = function (kpiId) {
        var index = vm.kpis.map(function (e) { return e.KPIId; }).indexOf(kpiId);

        if (index !== -1) {
            vm.kpiSelected = vm.kpis[index];
        }
    };

    vm.hoveringLeave = function (memberEPA, kpiGroup, kpi) {
        if (kpi.AdjustRate > 0) {
            if (kpiGroup.TotalKPIOptional === 0) {
                var score = kpi.Weight * (kpi.AdjustRate / vm.maxStar);

                kpi.AdjustScore = (score % 1) === 0 ? score : score.toFixed(2);
            }
            else {
                var weight, totalKPIOptional = angular.copy(kpiGroup.TotalKPIOptional);

                angular.forEach(kpiGroup.KPIs, function (item) {
                    if (!item.IsRequiredEvaluate && item.AdjustRate > 0) {
                        totalKPIOptional -= 1;
                    }
                });

                weight = kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional);

                angular.forEach(kpiGroup.KPIs, function (item) {
                    if (item.AdjustRate > 0) {
                        var score = weight * (item.AdjustRate / vm.maxStar);

                        item.AdjustScore = (score % 1) === 0 ? score : score.toFixed(2);
                    }
                });
            }

            RatingByKPIModel.calculatePerformanceScore(memberEPA);
        }
    };

    vm.cancelRating = function (memberEPA, kpiGroup, kpi) {
        kpi.AdjustRate = 0;
        kpi.AdjustScore = null;

        if (kpiGroup.TotalKPIOptional > 0) {
            var weight, totalKPIOptional = angular.copy(kpiGroup.TotalKPIOptional);

            angular.forEach(kpiGroup.KPIs, function (item) {
                if (!item.IsRequiredEvaluate && item.AdjustRate > 0) {
                    totalKPIOptional -= 1;
                }
            });

            weight = kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional);

            angular.forEach(kpiGroup.KPIs, function (item) {
                if (item.AdjustRate > 0) {
                    var score = weight * (item.AdjustRate / vm.maxStar);

                    item.AdjustScore = (score % 1) === 0 ? score : score.toFixed(2);
                }
            });
        }

        RatingByKPIModel.calculatePerformanceScore(memberEPA);
    };

    vm.search = function (filter) {
        RatingByKPIModel.getEmployeeEPAByKPI(filter.targetTemplateId, filter.kpiId, filter.groupId).then(function () {
            vm.memberEPAs = RatingByKPIModel.memberEPAs;

            angular.forEach(vm.memberEPAs, function (memberEPA) {
                if (!RatingByKPIModel.checkKPIOptional(memberEPA.KPIGroup) && memberEPA.KPIGroup.IsSpreadKPI) {
                    var weight = memberEPA.KPIGroup.Weight / memberEPA.KPIGroup.KPIs.length;
                    angular.forEach(memberEPA.KPIGroup.KPIs, function (kpi) {
                        if ((weight % 1) === 0) {
                            kpi.Weight = weight;
                        }
                        else {
                            kpi.Weight = weight.toFixed(2);
                        }
                    });
                }
            });
        });
    };

    vm.initialize = function () {
        RatingByKPIModel.getTargetTemplateByEvaluator().then(function () {
            vm.targetTemplates = RatingByKPIModel.targetTemplates;
        });

        CommonModel.getAllGroup().then(function () {
            vm.groupOptions = CommonModel.groups;
        });
    };

    vm.initialize();
}]);