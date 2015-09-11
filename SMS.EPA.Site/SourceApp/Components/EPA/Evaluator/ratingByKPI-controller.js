'use strict';
/*global epaModule*/

epaModule.controller('RatingByKPIController',
['$window', '$translate', 'RatingByKPIModel', 'CommonModel',
function ($window, $translate, RatingByKPIModel, CommonModel) {
    var vm = this;

    vm.kpis = [];
    vm.memberEPAs = [];
    vm.kpiSelected = null;
    vm.maxStar = 5;
    vm.targetTemplateIdOriginal = null;
    vm.kpiIdOriginal = null;
    vm.dataSaved = false;

    vm.filter = {
        targetTemplateId: '',
        kpiId: '',
        groupId: ''
    };

    vm.getKPIByTargetTemplate = function () {
        RatingByKPIModel.getKPIByTargetTemplate(vm.filter.targetTemplateId).then(function () {
            vm.kpis = RatingByKPIModel.kpis;
        });
    };

    vm.viewKPIInfo = function () {
        var index = vm.kpis.map(function (e) { return e.KPIId; }).indexOf(vm.filter.kpiId);
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

            RatingByKPIModel.calculatePerformanceScore(memberEPA, kpi);
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

        RatingByKPIModel.calculatePerformanceScore(memberEPA, kpi);
    };

    vm.inputScore = function (memberEPA, kpi) {
        kpi.AdjustRate = 0;

        if (!RatingByKPIModel.validateScoreInput(kpi.AdjustScore, kpi.Weight, vm.scoreInputValidMessage)) {
            kpi.AdjustScore = null;
        }

        RatingByKPIModel.calculatePerformanceScore(memberEPA, kpi);
    };

    vm.search = function (filter) {
        vm.targetTemplateIdOriginal = angular.copy(filter.targetTemplateId);
        vm.kpiIdOriginal = angular.copy(filter.kpiId);

        if (RatingByKPIModel.validateFieldFilter(filter)) {
            RatingByKPIModel.getEmployeeEPAByKPI(filter.targetTemplateId, filter.kpiId, filter.groupId).then(function () {
                vm.memberEPAs = RatingByKPIModel.memberEPAs;

                angular.forEach(vm.memberEPAs, function (memberEPA) {
                    memberEPA.TotalScoreOriginal = memberEPA.TotalScore;

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
        }
    };

    vm.save = function (memberEPAs) {
        RatingByKPIModel.updateKPIScore(memberEPAs, vm);
    };

    vm.back = function () {
        $window.history.back();
    };

    vm.initialize = function () {
        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });

        RatingByKPIModel.getTargetTemplateByEvaluator().then(function () {
            vm.targetTemplates = RatingByKPIModel.targetTemplates;
        });

        CommonModel.getAllGroup().then(function () {
            vm.groupOptions = CommonModel.groups;
        });
    };

    vm.initialize();
}]);

epaModule.controller('ModalConfirmSaveChangesController',
['$modalInstance',
function ($modalInstance) {
    var vm = this;

    vm.confirmResult = 0;

    vm.save = function () {
        vm.confirmResult = 1;
        $modalInstance.close(vm);
    };

    vm.ignore = function () {
        vm.confirmResult = 2;
        $modalInstance.close(vm);
    };

    vm.cancel = function () {
        $modalInstance.close(vm);
    };
}]);