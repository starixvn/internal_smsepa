'use strict';
/*global epaModule*/

epaModule.controller('RatingByKPIController',
['$window', '$timeout', '$translate', 'RatingByKPIModel', 'EvaluationModel', 'CommonModel', 'utilitiesFactory', 'baseUrl', 'historyFactory',
function ($window, $timeout, $translate, RatingByKPIModel, EvaluationModel, CommonModel, utilitiesFactory, baseUrl, historyFactory) {
    var vm = this;

    vm.periods = [];
    vm.targetTemplates = [];
    vm.kpis = [];
    vm.epaInfos = [];
    vm.kpiSelected = null;
    vm.maxStar = utilitiesFactory.maxStar;
    vm.targetTemplateIdOriginal = null;
    vm.kpiIdOriginal = null;
    vm.dataSaved = false;
    vm.permission = 0;
    vm.policyEnum = utilitiesFactory.policy;
    vm.ratingTypeEnum = utilitiesFactory.ratingType;
    vm.hasRatingByStar = false;
    vm.showRatingScoreByStar = true;
    vm.currentPage = 1;
    vm.pageSize = 20;
    utilitiesFactory.numberEntryList(vm);

    vm.filter = {
        periodId: '',
        targetTemplateId: '',
        kpiId: '',
        groupId: '',
        employeeIds: ''
    };

    vm.orderByCurrentScore = { ascending: false, descending: false };
    vm.orderByAdjustScore = { ascending: false, descending: false };

    vm.getCSSSelected = function (kpiId) {
        var cssClass = '';

        if (vm.kpiSelected !== null && kpiId === vm.kpiSelected.KPIId) {
            cssClass = 'alert-danger';
        }

        return cssClass;
    };

    vm.toggleCollapseExpand = function () {
        if (!vm.hasTreeEvent) {
            $('.tree li.parent_li > span').on('click', function (e) {
                var children = $(this).parent('li.parent_li').find(' > ul > li');
                if (children.is(":visible")) {
                    children.hide('fast');
                    $(this).attr('title', 'Expand this branch').find(' > i').addClass('glyphicon-plus-sign').removeClass('glyphicon-minus-sign');
                } else {
                    children.show('fast');
                    $(this).attr('title', 'Collapse this branch').find(' > i').addClass('glyphicon-minus-sign').removeClass('glyphicon-plus-sign');
                }

                e.stopPropagation();
            });

            vm.hasTreeEvent = true;
        }
    };

    function checkDataChanged(epaInfos) {
        var hasChanged = false;

        angular.forEach(epaInfos, function (epaInfo) {
            angular.forEach(epaInfo.KPIGroup.KPIs, function (kpi) {
                if (utilitiesFactory.parseFloat(kpi.AdjustScore) > 0 || (kpi.Comment !== kpi.CommentOriginal)) {
                    hasChanged = true;
                }
            });
        });

        return hasChanged;
    }

    vm.getKPIByTargetTemplate = function () {
        if (!vm.dataSaved && checkDataChanged(vm.epaInfos)) {
            RatingByKPIModel.openPopupConfirmSaveChanges(vm).then(function (result) {
                if (result.confirmResult === 0) {
                    vm.filter.targetTemplateId = angular.copy(vm.targetTemplateIdOriginal);
                    vm.filter.kpiId = angular.copy(vm.kpiIdOriginal);
                } else if (result.confirmResult === 1) {
                    RatingByKPIModel.getKPIByTargetTemplate(vm.filter.periodId, vm.filter.targetTemplateId).then(function () {
                        vm.kpis = RatingByKPIModel.kpis;
                    });
                    vm.epaInfos = [];
                    vm.filteredItems = [];
                    vm.kpiSelected = null;
                    vm.kpiIdOriginal = null;
                    vm.dataSaved = false;
                } else {
                    RatingByKPIModel.getKPIByTargetTemplate(vm.filter.periodId, vm.filter.targetTemplateId).then(function () {
                        vm.kpis = RatingByKPIModel.kpis;
                    });
                    vm.epaInfos = [];
                    vm.filteredItems = [];
                    vm.kpiSelected = null;
                    vm.kpiIdOriginal = null;
                    vm.dataSaved = false;
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        } else {
            vm.kpiSelected = null;
            vm.kpiIdOriginal = null;
            vm.filter.kpiId = '';
            vm.epaInfos = [];
            vm.filteredItems = [];

            RatingByKPIModel.getKPIByTargetTemplate(vm.filter.periodId, vm.filter.targetTemplateId).then(function () {
                vm.kpis = RatingByKPIModel.kpis;
            });
        }
    };

    vm.cancelRating = function (epaInfo, kpi) {
        kpi.AdjustRate = 0;
        kpi.AdjustScore = null;
        $("#star_" + epaInfo.Employee.EmployeeId).rating('update', kpi.AdjustRate);

        if (epaInfo.KPIGroup.HasKPIOptional) {
            RatingByKPIModel.scoreSpreadForKPIGroupHasKPIOptional(epaInfo.KPIGroup, vm.maxStar);

            var totalScoreEvaluated = epaInfo.KPIGroup.Weight * (epaInfo.KPIGroup.TotalStarEvaluated / epaInfo.KPIGroup.TotalStarMustEvaluate),
                totalScoreOriginal = utilitiesFactory.parseFloat(epaInfo.TotalScoreOriginal);

            epaInfo.TotalScore = utilitiesFactory.parseFloat(totalScoreOriginal + totalScoreEvaluated);
        } else {
            if (kpi.Rate === vm.maxStar) {
                kpi.IsMaxScore = true;
            } else {
                kpi.IsMaxScore = false;
            }
            epaInfo.TotalScore = utilitiesFactory.parseFloat(epaInfo.TotalScoreOriginal);
        }
    };

    vm.inputScore = function (epaInfo, kpi) {
        var kpiWeight = utilitiesFactory.splitNumber(kpi.Weight, 2),
            adjustScore = utilitiesFactory.splitNumber(kpi.AdjustScore, 2);

        if (!RatingByKPIModel.validateScoreInput(adjustScore, 0, kpiWeight, vm.scoreInputValidMessage)) {
            kpi.AdjustScore = null;
        } else {
            if (adjustScore === kpiWeight) {
                kpi.IsMaxScore = true;
            } else {
                kpi.IsMaxScore = false;
            }
        }
        kpi.AdjustRate = 0;
        var currentScore = utilitiesFactory.splitNumber(kpi.Score, 2);

        if (currentScore !== adjustScore) {
            vm.dataSaved = false;
        }

        if (currentScore > adjustScore && adjustScore > 0) {
            epaInfo.TotalScore = utilitiesFactory.parseFloat(epaInfo.TotalScoreOriginal) - (currentScore - adjustScore);
        } else if (currentScore < adjustScore && adjustScore > 0) {
            epaInfo.TotalScore = utilitiesFactory.parseFloat(epaInfo.TotalScoreOriginal) + (adjustScore - currentScore);
        } else {
            epaInfo.TotalScore = utilitiesFactory.parseFloat(epaInfo.TotalScoreOriginal);
        }
    };

    vm.KPIChange = function (kpiId) {
        if (kpiId !== vm.kpiIdOriginal) {
            vm.filter.kpiId = kpiId;
            vm.viewKPIInfo();
        }
    };

    vm.viewKPIInfo = function () {
        var index = vm.kpis.map(function (e) { return e.KPIId; }).indexOf(vm.filter.kpiId);

        if (!vm.kpis[index].IsRequiredEvaluate) {
            vm.kpis[index].HasKPIOptional = true;
        } else {
            angular.forEach(vm.kpis, function (kpi) {
                if (kpi.KPIId !== vm.kpis[index].KPIId
                    && kpi.KPIGroup.KPIGroupId === vm.kpis[index].KPIGroup.KPIGroupId
                    && !kpi.IsRequiredEvaluate) {
                    vm.kpis[index].HasKPIOptional = true;
                }
            });
        }

        if (!vm.kpis[index].HasKPIOptional && vm.kpis[index].KPIGroup.IsSpreadKPI) {
            var weightKPI = (vm.kpis[index].KPIGroup.Weight / vm.kpis[index].KPIGroup.NumberKPI);
            vm.kpis[index].Weight = utilitiesFactory.splitNumber(weightKPI, 4);
        }

        if (!vm.dataSaved && checkDataChanged(vm.epaInfos)) {
            RatingByKPIModel.openPopupConfirmSaveChanges(vm).then(function (result) {
                if (result.confirmResult === 0) {
                    vm.filter.kpiId = angular.copy(vm.kpiIdOriginal);
                } else if (result.confirmResult === 1) {
                    vm.epaInfos = [];
                    vm.kpiSelected = null;
                    vm.dataSaved = false;
                    vm.query();
                } else {
                    vm.epaInfos = [];
                    vm.kpiSelected = null;
                    vm.dataSaved = false;
                    vm.query();
                }

                index = vm.kpis.map(function (e) { return e.KPIId; }).indexOf(vm.filter.kpiId);
                if (index !== -1) {
                    vm.kpiSelected = vm.kpis[index];
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        } else {
            index = vm.kpis.map(function (e) { return e.KPIId; }).indexOf(vm.filter.kpiId);
            vm.kpiSelected = vm.kpis[index];
            vm.epaInfos = [];
            vm.query();
        }
    };

    vm.query = function () {
        vm.targetTemplateIdOriginal = angular.copy(vm.filter.targetTemplateId);
        vm.kpiIdOriginal = angular.copy(vm.filter.kpiId);
        vm.orderByCurrentScore = { ascending: false, descending: false };
        vm.orderByAdjustScore = { ascending: false, descending: false };

        RatingByKPIModel.getEmployeeEPAByKPI(vm.filter.periodId, vm.filter.targetTemplateId, vm.filter.kpiId, vm.filter.groupId, null).then(function () {
            vm.epaInfos = RatingByKPIModel.epaInfos;
            utilitiesFactory.searchOnPage(vm, vm.epaInfos);

            angular.forEach(vm.epaInfos, function (epaInfo) {
                if (epaInfo.RatingType.RatingTypeId === vm.ratingTypeEnum.Star
                    || epaInfo.RatingType.RatingTypeId === vm.ratingTypeEnum.HalfStar
                    || (epaInfo.RatingType.RatingTypeId === vm.ratingTypeEnum.Score && epaInfo.KPIGroup.NumberKPIOptional > 0)) {
                    vm.hasRatingByStar = true;
                }

                if (epaInfo.KPIGroup.HasKPIOptional) {
                    var totalStarEvaluated = 0,
                        numberKPIOptional = epaInfo.KPIGroup.NumberKPIOptional,
                        totalScoreOriginal = utilitiesFactory.parseFloat(epaInfo.TotalScoreOriginal);

                    angular.forEach(epaInfo.KPIGroup.KPIs, function (kpi) {
                        if (kpi.Rate > 0) {
                            totalStarEvaluated += kpi.Rate;
                        }

                        if (!kpi.IsRequiredEvaluate && kpi.Rate > 0) {
                            numberKPIOptional -= 1;
                        }
                    });

                    var totalKPIMustEvaluate = epaInfo.KPIGroup.KPIs.length - numberKPIOptional,
                        totalStarMustEvaluate = totalKPIMustEvaluate * vm.maxStar,
                        totalScoreEvaluated = (epaInfo.KPIGroup.Weight * totalStarEvaluated) / totalStarMustEvaluate;

                    totalScoreEvaluated = utilitiesFactory.parseFloat(totalScoreEvaluated);
                    epaInfo.TotalScoreOriginal = utilitiesFactory.parseFloat(totalScoreOriginal - totalScoreEvaluated);
                }

                EvaluationModel.scoreSpread(epaInfo.KPIGroup);
            });
        });
    };

    function sortByCurrentScore(epaInfo1, epaInfo2) {
        var index;

        index = epaInfo1.KPIGroup.KPIs.map(function (e) { return e.KPIId; }).indexOf(vm.kpiIdOriginal);
        var tempa = epaInfo1.KPIGroup.KPIs[index].Score ? parseFloat(epaInfo1.KPIGroup.KPIs[index].Score) : 0;

        index = epaInfo2.KPIGroup.KPIs.map(function (e) { return e.KPIId; }).indexOf(vm.kpiIdOriginal);
        var tempb = epaInfo2.KPIGroup.KPIs[index].Score ? parseFloat(epaInfo2.KPIGroup.KPIs[index].Score) : 0;

        return tempa - tempb;
    }

    function sortByAdjustScore(epaInfo1, epaInfo2) {
        var index;

        index = epaInfo1.KPIGroup.KPIs.map(function (e) { return e.KPIId; }).indexOf(vm.kpiIdOriginal);
        var tempa = epaInfo1.KPIGroup.KPIs[index].AdjustScore ? parseFloat(epaInfo1.KPIGroup.KPIs[index].AdjustScore) : 0;

        index = epaInfo2.KPIGroup.KPIs.map(function (e) { return e.KPIId; }).indexOf(vm.kpiIdOriginal);
        var tempb = epaInfo2.KPIGroup.KPIs[index].AdjustScore ? parseFloat(epaInfo2.KPIGroup.KPIs[index].AdjustScore) : 0;

        return tempa - tempb;
    }

    vm.changeSorting = function (field) {
        switch (field) {
            case 'CurrentScore':
                if (!vm.orderByCurrentScore.ascending) {
                    vm.orderByCurrentScore.ascending = true;
                    vm.orderByCurrentScore.descending = false;
                    vm.epaInfos.sort(sortByCurrentScore);
                } else {
                    vm.orderByCurrentScore.ascending = false;
                    vm.orderByCurrentScore.descending = true;
                    vm.epaInfos.reverse();
                }
                break;
            case 'AdjustScore':
                if (!vm.orderByAdjustScore.ascending) {
                    vm.orderByAdjustScore.ascending = true;
                    vm.orderByAdjustScore.descending = false;
                    vm.epaInfos.sort(sortByAdjustScore);
                } else {
                    vm.orderByAdjustScore.ascending = false;
                    vm.orderByAdjustScore.descending = true;
                    vm.epaInfos.reverse();
                }
                break;
        }
    };

    vm.selectedCls = function (field) {
        var cssClass = '';

        switch (field) {
            case 'CurrentScore':
                if (vm.orderByCurrentScore.ascending) {
                    cssClass = 'sorting-asc';
                } else if (vm.orderByCurrentScore.descending) {
                    cssClass = 'sorting-desc';
                }
                break;
            case 'AdjustScore':
                if (vm.orderByAdjustScore.ascending) {
                    cssClass = 'sorting-asc';
                } else if (vm.orderByAdjustScore.descending) {
                    cssClass = 'sorting-desc';
                }
                break;
        }

        return cssClass;
    };

    vm.viewDetail = function (epaInfo) {
        var url = baseUrl + 'Evaluator/Detail/' + epaInfo.EPAId + '?newTab=' + true;
        $window.open(url, '_blank');
    };

    vm.save = function () {
        if (RatingByKPIModel.validateEPA(vm)) {
            RatingByKPIModel.updateKPIScore(vm);
        }
    };

    vm.back = function () {
        historyFactory.back();
    };

    RatingByKPIModel.getUserPermissionOnRatingByKPI().then(function () {
        vm.permission = RatingByKPIModel.permission;

        if (vm.permission === vm.policyEnum.Host) {
            vm.hoveringLeave = function (epaInfo, kpi) {
                if (kpi.AdjustRate > 0) {
                    if (kpi.Rate !== kpi.AdjustRate) {
                        vm.dataSaved = false;
                    }

                    var totalScoreOriginal = utilitiesFactory.parseFloat(epaInfo.TotalScoreOriginal),
                        totalScoreEvaluated, currentScore, adjustScore;

                    if (!epaInfo.KPIGroup.HasKPIOptional) {
                        currentScore = utilitiesFactory.splitNumber(kpi.Score, 4);
                        adjustScore = (kpi.Weight * kpi.AdjustRate) / vm.maxStar;
                        adjustScore = utilitiesFactory.splitNumber(adjustScore, 4);
                        kpi.AdjustScore = adjustScore;

                        if (kpi.AdjustRate === vm.maxStar) {
                            kpi.IsMaxScore = true;
                        } else {
                            kpi.IsMaxScore = false;
                        }

                        if (currentScore > 0 && currentScore > adjustScore) {
                            epaInfo.TotalScore = totalScoreOriginal - (currentScore - adjustScore);
                        } else {
                            epaInfo.TotalScore = totalScoreOriginal + (adjustScore - currentScore);
                        }
                    } else {
                        RatingByKPIModel.scoreSpreadForKPIGroupHasKPIOptional(epaInfo.KPIGroup, vm.maxStar);
                        totalScoreEvaluated = (epaInfo.KPIGroup.Weight * epaInfo.KPIGroup.TotalStarEvaluated) / epaInfo.KPIGroup.TotalStarMustEvaluate;
                        epaInfo.TotalScore = utilitiesFactory.parseFloat(totalScoreOriginal + totalScoreEvaluated);
                    }
                }
            };
        } else {
            vm.hoveringLeave = null;
        }
    });

    vm.generateRatingStar = function () {
        $timeout(function () {
            $(".rating-loading").rating();
        }, 1);
    };

    vm.getTargetTemplateByPeriod = function (periodId) {
        RatingByKPIModel.getTargetTemplateByPeriod(periodId).then(function () {
            vm.targetTemplates = RatingByKPIModel.targetTemplates;
        });
    };

    function initialize() {
        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });

        $translate('EvaluateEPAInvalid').then(function (message) {
            vm.evaluateEPAInvalidMessage = message;
        });

        $translate('RequiredCommentForRatingMaxScore').then(function (message) {
            vm.requiredCommentForRatingMaxScore = message;
        });

        RatingByKPIModel.getPeriodByEvaluator().then(function () {
            vm.periods = RatingByKPIModel.periods;
        });
    }

    initialize();
}]);

epaModule.controller('ModalConfirmSaveChangesController',
['$translate', '$modalInstance', 'RatingByKPIModel', 'kpiSelected', 'epaInfos',
function ($translate, $modalInstance, RatingByKPIModel, kpiSelected, epaInfos) {
    var vm = this;

    vm.confirmResult = 0;
    vm.kpiSelected = kpiSelected;
    vm.epaInfos = epaInfos;

    vm.save = function () {
        vm.confirmResult = 1;
        if (RatingByKPIModel.validateEPA(vm)) {
            RatingByKPIModel.updateKPIScore(vm, $modalInstance);
        }
    };

    vm.ignore = function () {
        vm.confirmResult = 2;
        $modalInstance.close(vm);
    };

    vm.cancel = function () {
        $modalInstance.close(vm);
    };

    $translate('EvaluateEPAInvalid').then(function (message) {
        vm.evaluateEPAInvalidMessage = message;
    });

    $translate('RequiredCommentForRatingMaxScore').then(function (message) {
        vm.requiredCommentForRatingMaxScore = message;
    });
}]);