'use strict';
/*global epaModule*/

epaModule.controller('RatingByKPIController',
['$window', '$translate', 'RatingByKPIModel', 'EmployeeEPAModel', 'SelfEvalutorModel', 'CommonModel', 'utilitiesFactory', 'baseUrl',
function ($window, $translate, RatingByKPIModel, EmployeeEPAModel, SelfEvalutorModel, CommonModel, utilitiesFactory, baseUrl) {
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
                if (utilitiesFactory.parseFloat(kpi.AdjustScore) > 0 || (kpi.Comment !== kpi.CommentOriginal)) {
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
        SelfEvalutorModel.getMemberByEvaluator(groupId).then(function () {
            vm.members = SelfEvalutorModel.members;

            angular.forEach(vm.members, function (member) {
                member.Name = member.NickName + ' (' + member.FullName + ')';
            });
        });
    };

    vm.hoveringLeave = function (memberEPA, kpi) {
        if (kpi.AdjustRate > 0) {
            if (kpi.Rate !== kpi.AdjustRate) {
                vm.dataSaved = false;
            }

            var totalScoreOriginal = utilitiesFactory.parseFloat(memberEPA.TotalScoreOriginal),
                totalScoreEvaluated, currentScore, adjustScore;

            if (!memberEPA.KPIGroup.HasKPIOptional) {
                currentScore = utilitiesFactory.parseFloat(kpi.Score);
                adjustScore = utilitiesFactory.parseFloat(kpi.Weight * (kpi.AdjustRate / vm.maxStar));
                kpi.AdjustScore = adjustScore;

                if (kpi.AdjustRate === vm.maxStar) {
                    kpi.IsMaxScore = true;
                } else {
                    kpi.IsMaxScore = false;
                }

                if (currentScore > 0 && currentScore > adjustScore) {
                    memberEPA.TotalScore = totalScoreOriginal - (currentScore - adjustScore);
                } else {
                    memberEPA.TotalScore = totalScoreOriginal + (adjustScore - currentScore);
                }
            } else {
                RatingByKPIModel.scoreSpreadForKPIGroupHasKPIOptional(memberEPA.KPIGroup, vm.maxStar);

                totalScoreEvaluated = memberEPA.KPIGroup.Weight * (memberEPA.KPIGroup.TotalStarEvaluated / memberEPA.KPIGroup.TotalStarMustEvaluate);
                memberEPA.TotalScore = utilitiesFactory.parseFloat(totalScoreOriginal + totalScoreEvaluated);
            }
        }
    };

    vm.cancelRating = function (memberEPA, kpi) {
        kpi.AdjustRate = 0;
        kpi.AdjustScore = null;

        if (memberEPA.KPIGroup.HasKPIOptional) {
            RatingByKPIModel.scoreSpreadForKPIGroupHasKPIOptional(memberEPA.KPIGroup, vm.maxStar);

            var totalScoreEvaluated = memberEPA.KPIGroup.Weight * (memberEPA.KPIGroup.TotalStarEvaluated / memberEPA.KPIGroup.TotalStarMustEvaluate),
                totalScoreOriginal = utilitiesFactory.parseFloat(memberEPA.TotalScoreOriginal);

            memberEPA.TotalScore = utilitiesFactory.parseFloat(totalScoreOriginal + totalScoreEvaluated);
        } else {
            if (kpi.Rate === vm.maxStar) {
                kpi.IsMaxScore = true;
            } else {
                kpi.IsMaxScore = false;
            }
            memberEPA.TotalScore = utilitiesFactory.parseFloat(memberEPA.TotalScoreOriginal);
        }
    };

    vm.inputScore = function (memberEPA, kpi) {
        if (!RatingByKPIModel.validateScoreInput(kpi.AdjustScore, 0, kpi.Weight, vm.scoreInputValidMessage)) {
            kpi.AdjustScore = null;
        }
        else {
            if (kpi.AdjustScore === kpi.Weight) {
                kpi.IsMaxScore = true;
            } else {
                kpi.IsMaxScore = false;
            }
        }
        kpi.AdjustRate = 0;
        var currentScore = utilitiesFactory.parseFloat(kpi.Score),
            adjustScore = utilitiesFactory.parseFloat(kpi.AdjustScore);

        if (currentScore !== adjustScore) {
            vm.dataSaved = false;
        }

        if (currentScore > adjustScore && adjustScore > 0) {
            memberEPA.TotalScore = utilitiesFactory.parseFloat(memberEPA.TotalScoreOriginal) - (currentScore - adjustScore);
        } else if (currentScore < adjustScore && adjustScore > 0) {
            memberEPA.TotalScore = utilitiesFactory.parseFloat(memberEPA.TotalScoreOriginal) + (adjustScore - currentScore);
        } else {
            memberEPA.TotalScore = utilitiesFactory.parseFloat(memberEPA.TotalScoreOriginal);
        }
    };

    vm.search = function (filter) {
        vm.targetTemplateIdOriginal = angular.copy(filter.targetTemplateId);
        vm.kpiIdOriginal = angular.copy(filter.kpiId);

        if (RatingByKPIModel.validateFieldFilter(filter)) {
            filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
            RatingByKPIModel.getEmployeeEPAByKPI(filter.targetTemplateId, filter.kpiId, filter.groupId, filter.employeeIds).then(function () {
                vm.memberEPAs = RatingByKPIModel.memberEPAs;

                angular.forEach(vm.memberEPAs, function (memberEPA) {
                    if (memberEPA.KPIGroup.HasKPIOptional) {
                        var totalScoreEvaluated = 0, totalKPIMustEvaluate, totalStarMustEvaluate, totalStarEvaluated = 0,
                            numberKPIOptional = memberEPA.KPIGroup.NumberKPIOptional,
                            totalScoreOriginal = utilitiesFactory.parseFloat(memberEPA.TotalScoreOriginal);

                        angular.forEach(memberEPA.KPIGroup.KPIs, function (kpi) {
                            if (kpi.Rate > 0) {
                                totalStarEvaluated += kpi.Rate;
                            }

                            if (!kpi.IsRequiredEvaluate && kpi.Rate > 0) {
                                numberKPIOptional -= 1;
                            }
                        });

                        totalKPIMustEvaluate = memberEPA.KPIGroup.KPIs.length - numberKPIOptional;
                        totalStarMustEvaluate = totalKPIMustEvaluate * vm.maxStar;

                        totalScoreEvaluated = utilitiesFactory.parseFloat(memberEPA.KPIGroup.Weight * (totalStarEvaluated / totalStarMustEvaluate));
                        memberEPA.TotalScoreOriginal = utilitiesFactory.parseFloat(totalScoreOriginal - totalScoreEvaluated);
                    }

                    EmployeeEPAModel.scoreSpread(memberEPA.KPIGroup);
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
            else if (vm.orderByCurrentScore.descending) {
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
        var url = baseUrl + 'Evaluator/Detail/' + memberEPA.EPAId;
        $window.open(url, '_blank');
    };

    vm.save = function () {
        if (RatingByKPIModel.validateEPA(vm)) {
            RatingByKPIModel.updateKPIScore(vm);
        }
    };

    vm.back = function () {
        $window.history.back();
    };

    vm.initialize = function () {
        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });

        $translate('EvaluateEPAInvalid').then(function (message) {
            vm.evaluateEPAInvalidMessage = message;
        });

        $translate('RequiredCommentForMaxScoreRating').then(function (message) {
            vm.requiredCommentForMaxScoreRating = message;
        });

        RatingByKPIModel.getTargetTemplateByEvaluator().then(function () {
            vm.targetTemplates = RatingByKPIModel.targetTemplates;
        });

        SelfEvalutorModel.getGroupByEvaluator().then(function () {
            vm.groupOptions = SelfEvalutorModel.groups;
        });

        vm.getMember(vm.filter.groupId);
    };

    vm.initialize();
}]);

epaModule.controller('ModalConfirmSaveChangesController',
['$translate', '$modalInstance', 'RatingByKPIModel', 'kpiSelected', 'memberEPAs',
function ($translate, $modalInstance, RatingByKPIModel, kpiSelected, memberEPAs) {
    var vm = this;

    vm.confirmResult = 0;
    vm.kpiSelected = kpiSelected;
    vm.memberEPAs = memberEPAs;

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

    $translate('RequiredCommentForMaxScoreRating').then(function (message) {
        vm.requiredCommentForMaxScoreRating = message;
    });
}]);