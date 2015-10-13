'use strict';
/*global epaModule*/

epaModule.controller('RatingByKPIController',
['$window', '$translate', 'RatingByKPIModel', 'EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function ($window, $translate, RatingByKPIModel, EmployeeEPAModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.kpis = [];
    vm.memberEPAs = [];
    vm.members = [];

    vm.kpiSelected = null;
    vm.maxStar = 5;
    vm.targetTemplateIdOriginal = null;
    vm.kpiIdOriginal = null;
    vm.dataSaved = false;

    vm.filter = {
        targetTemplateId: '',
        kpiId: '',
        groupId: '',
        employeeIds: ''
    };

    vm.orderByCurrentScore = { ascending: false, descending: false };
    vm.orderByAdjustScore = { ascending: false, descending: false };

    var checkDataChanged = function (memberEPAs) {
        var hasChanged = false;

        angular.forEach(memberEPAs, function (memberEPA) {
            angular.forEach(memberEPA.KPIGroup.KPIs, function (kpi) {
                if (kpi.AdjustScore || (kpi.Comment !== kpi.CommentOriginal)) {
                    hasChanged = true;
                }
            });
        });

        return hasChanged;
    };

    vm.getKPIByTargetTemplate = function () {
        if (!vm.dataSaved && checkDataChanged(vm.memberEPAs)) {
            RatingByKPIModel.openPopupConfirmSaveChanges(vm).then(function (result) {
                if (result.confirmResult === 0) {
                    vm.filter.targetTemplateId = angular.copy(vm.targetTemplateIdOriginal);
                    vm.filter.kpiId = angular.copy(vm.kpiIdOriginal);
                } else if (result.confirmResult === 1) {
                    RatingByKPIModel.getKPIByTargetTemplate(vm.filter.targetTemplateId).then(function () {
                        vm.kpis = RatingByKPIModel.kpis;
                    });
                    vm.memberEPAs = [];
                    vm.kpiSelected = null;
                    vm.dataSaved = false;
                }
                else {
                    RatingByKPIModel.getKPIByTargetTemplate(vm.filter.targetTemplateId).then(function () {
                        vm.kpis = RatingByKPIModel.kpis;
                    });
                    vm.memberEPAs = [];
                    vm.kpiSelected = null;
                    vm.dataSaved = false;
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }
        else {
            vm.kpiSelected = null;
            vm.filter.kpiId = '';
            vm.memberEPAs = [];

            RatingByKPIModel.getKPIByTargetTemplate(vm.filter.targetTemplateId).then(function () {
                vm.kpis = RatingByKPIModel.kpis;
            });
        }
    };

    vm.viewKPIInfo = function () {
        var index = vm.kpis.map(function (e) { return e.KPIId; }).indexOf(vm.filter.kpiId);

        if (!vm.kpis[index].IsRequiredEvaluate) {
            vm.kpis[index].HasKPIOptional = true;
        }
        else {
            angular.forEach(vm.kpis, function (kpi) {
                if (kpi.KPIId !== vm.kpis[index].KPIId
                    && kpi.KPIGroup.KPIGroupId === vm.kpis[index].KPIGroup.KPIGroupId
                    && !kpi.IsRequiredEvaluate) {
                    vm.kpis[index].HasKPIOptional = true;
                }
            });
        }

        if (!vm.kpis[index].HasKPIOptional && vm.kpis[index].KPIGroup.IsSpreadKPI) {
            vm.kpis[index].Weight = (vm.kpis[index].KPIGroup.Weight / vm.kpis[index].KPIGroup.NumberKPI).toFixed(2);
        }

        if (!vm.dataSaved && checkDataChanged(vm.memberEPAs)) {
            RatingByKPIModel.openPopupConfirmSaveChanges(vm).then(function (result) {
                if (result.confirmResult === 0) {
                    vm.filter.kpiId = angular.copy(vm.kpiIdOriginal);
                } else if (result.confirmResult === 1) {
                    vm.memberEPAs = [];
                    vm.kpiSelected = null;
                    vm.dataSaved = false;
                }
                else {
                    vm.memberEPAs = [];
                    vm.kpiSelected = null;
                    vm.dataSaved = false;
                }

                index = vm.kpis.map(function (e) { return e.KPIId; }).indexOf(vm.filter.kpiId);
                if (index !== -1) {
                    vm.kpiSelected = vm.kpis[index];
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }
        else {
            index = vm.kpis.map(function (e) { return e.KPIId; }).indexOf(vm.filter.kpiId);
            vm.kpiSelected = vm.kpis[index];

            vm.memberEPAs = [];
        }
    };

    vm.getMember = function (groupId) {
        EmployeeEPAModel.getMemberByEvaluator(groupId).then(function () {
            vm.members = EmployeeEPAModel.members;

            angular.forEach(vm.members, function (member) {
                member.Name = member.NickName + ' (' + member.FullName + ')';
            });
        });
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
            filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
            RatingByKPIModel.getEmployeeEPAByKPI(filter.targetTemplateId, filter.kpiId, filter.groupId, filter.employeeIds).then(function () {
                vm.memberEPAs = RatingByKPIModel.memberEPAs;

                angular.forEach(vm.memberEPAs, function (memberEPA) {
                    if (!memberEPA.KPIGroup.HasKPIOptional && memberEPA.KPIGroup.IsSpreadKPI) {
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

    vm.changeSorting = function (column) {
        if (column === 'CurrentScore') {
            if (!vm.orderByCurrentScore.ascending) {
                vm.orderByCurrentScore.ascending = true;
                vm.orderByCurrentScore.descending = false;

                vm.memberEPAs.sort(function (a, b) {
                    var index, tempa, tempb;

                    index = a.KPIGroup.KPIs.map(function (e) { return e.KPIId; }).indexOf(vm.kpiIdOriginal);
                    tempa = a.KPIGroup.KPIs[index].Score ? parseFloat(a.KPIGroup.KPIs[index].Score) : 0;

                    index = b.KPIGroup.KPIs.map(function (e) { return e.KPIId; }).indexOf(vm.kpiIdOriginal);
                    tempb = b.KPIGroup.KPIs[index].Score ? parseFloat(b.KPIGroup.KPIs[index].Score) : 0;

                    return tempa - tempb;
                });
            }
            else {
                vm.orderByCurrentScore.ascending = false;
                vm.orderByCurrentScore.descending = true;
                vm.memberEPAs.reverse();
            }
        }
        else {
            if (!vm.orderByAdjustScore.ascending) {
                vm.orderByAdjustScore.ascending = true;
                vm.orderByAdjustScore.descending = false;

                vm.memberEPAs.sort(function (a, b) {
                    var index, tempa, tempb;

                    index = a.KPIGroup.KPIs.map(function (e) { return e.KPIId; }).indexOf(vm.kpiIdOriginal);
                    tempa = a.KPIGroup.KPIs[index].AdjustScore ? parseFloat(a.KPIGroup.KPIs[index].AdjustScore) : 0;

                    index = b.KPIGroup.KPIs.map(function (e) { return e.KPIId; }).indexOf(vm.kpiIdOriginal);
                    tempb = b.KPIGroup.KPIs[index].AdjustScore ? parseFloat(b.KPIGroup.KPIs[index].AdjustScore) : 0;

                    return tempa - tempb;
                });
            }
            else {
                vm.orderByAdjustScore.ascending = false;
                vm.orderByAdjustScore.descending = true;
                vm.memberEPAs.reverse();
            }
        }
    };

    vm.selectedCls = function (column) {
        if (column === 'CurrentScore') {
            if (vm.orderByCurrentScore.ascending) {
                return 'sorting-asc';
            }
            else if(vm.orderByCurrentScore.descending) {
                return 'sorting-desc';
            }
        }
        else {
            if (vm.orderByAdjustScore.ascending) {
                return 'sorting-asc';
            }
            else if (vm.orderByAdjustScore.descending) {
                return 'sorting-desc';
            }
        }
    };

    vm.viewDetail = function (memberEPA) {
        var url = '/Evaluator/Detail/' + memberEPA.EPAId;
        $window.open(url, '_blank');
    };

    vm.save = function () {
        RatingByKPIModel.updateKPIScore(vm);
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

        RatingByKPIModel.getGroupByEvaluator().then(function () {
            vm.groupOptions = RatingByKPIModel.groups;
        });

        vm.getMember(vm.filter.groupId);
    };

    vm.initialize();
}]);

epaModule.controller('ModalConfirmSaveChangesController',
['$modalInstance', 'RatingByKPIModel', 'memberEPAs',
function ($modalInstance, RatingByKPIModel, memberEPAs) {
    var vm = this;

    vm.confirmResult = 0;
    vm.memberEPAs = memberEPAs;

    vm.save = function () {
        vm.confirmResult = 1;
        RatingByKPIModel.updateKPIScore(vm, $modalInstance);
    };

    vm.ignore = function () {
        vm.confirmResult = 2;
        $modalInstance.close(vm);
    };

    vm.cancel = function () {
        $modalInstance.close(vm);
    };
}]);