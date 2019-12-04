'use strict';
/*global epaModule*/

epaModule.factory('RatingByKPIModel',
['$uibModal', 'ratingByKPIFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory',
function ($uibModal, ratingByKPIFactory, utilitiesFactory, notificationFactory, errorFactory) {
    var ratingByKPIModel = this;

    ratingByKPIModel.periods = [];
    ratingByKPIModel.targetTemplates = [];
    ratingByKPIModel.kpis = [];
    ratingByKPIModel.epaInfos = [];
    ratingByKPIModel.permission = 0;

    ratingByKPIModel.getUserPermissionOnRatingByKPI = function () {
        return ratingByKPIFactory.getUserPermissionOnRatingByKPI().success(function (permission) {
            ratingByKPIModel.permission = parseInt(permission);
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    ratingByKPIModel.getPeriodByEvaluator = function () {
        return ratingByKPIFactory.getPeriodByEvaluator().success(function (periods) {
            ratingByKPIModel.periods = periods;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    ratingByKPIModel.getTargetTemplateByPeriod = function (periodId) {
        return ratingByKPIFactory.getTargetTemplateByPeriod(periodId).success(function (targetTemplates) {
            ratingByKPIModel.targetTemplates = targetTemplates;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    ratingByKPIModel.getKPIByTargetTemplate = function (periodId, targetTemplateId) {
        return ratingByKPIFactory.getKPIByTargetTemplate(periodId, targetTemplateId).success(function (kpis) {
            ratingByKPIModel.kpis = kpis;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    ratingByKPIModel.getEmployeeEPAByKPI = function (periodId, targetTemplateId, kpiId, groupId, employeeIds) {
        return ratingByKPIFactory.getEmployeeEPAByKPI(periodId, targetTemplateId, kpiId, groupId, employeeIds).success(function (epaInfos) {
            ratingByKPIModel.epaInfos = epaInfos;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    ratingByKPIModel.openPopupConfirmSaveChanges = function (vm) {
        vm.modalInstance = $uibModal.open({
            templateUrl: 'SourceApp/Components/EPA/RatingByKPI/Templates/modalConfirmSaveChanges.html',
            controller: "ModalConfirmSaveChangesController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                kpiSelected: function () {
                    return vm.kpiSelected;
                },
                epaInfos: function () {
                    return vm.epaInfos;
                }
            },
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    ratingByKPIModel.validateScoreInput = function (score, minScore, maxScore, message) {
        var isValid = true;

        if (utilitiesFactory.parseFloat(score) > utilitiesFactory.parseFloat(maxScore)
            || utilitiesFactory.parseFloat(score) < utilitiesFactory.parseFloat(minScore)) {
            isValid = false;

            var msg = utilitiesFactory.formatString(message, minScore, maxScore);
            notificationFactory.error('', msg);
        }

        return isValid;
    };

    ratingByKPIModel.scoreSpreadForKPIGroupHasKPIOptional = function (kpiGroup, maxStar) {
        var totalStarEvaluated = 0,
            numberKPIOptional = kpiGroup.NumberKPIOptional,
            kpis = [];

        angular.forEach(kpiGroup.KPIs, function (kpi) {
            if (kpi.Rate > 0 && !kpi.AdjustRate) {
                kpis.push(kpi);
                totalStarEvaluated += kpi.Rate;
            } else if (kpi.AdjustRate > 0) {
                kpis.push(kpi);
                totalStarEvaluated += kpi.AdjustRate;
            }

            if (!kpi.IsRequiredEvaluate && ((kpi.Rate > 0 && !kpi.AdjustRate) || (kpi.AdjustRate > 0))) {
                numberKPIOptional -= 1;
            }
        });

        var totalKPIMustEvaluate = kpiGroup.KPIs.length - numberKPIOptional,
            totalStarMustEvaluate = totalKPIMustEvaluate * maxStar,
            averageWeight = utilitiesFactory.splitNumber(kpiGroup.Weight / totalKPIMustEvaluate, 4);

        kpiGroup.TotalStarMustEvaluate = totalStarMustEvaluate;
        kpiGroup.TotalStarEvaluated = totalStarEvaluated;

        angular.forEach(kpis, function (kpi) {
            kpi.Weight = averageWeight;

            if (kpi.Rate > 0 && !kpi.AdjustRate) {
                if (kpi.Rate === maxStar) {
                    kpi.IsMaxScore = true;
                } else {
                    kpi.IsMaxScore = false;
                }

                var score = (averageWeight * kpi.Rate) / maxStar;
                kpi.Score = utilitiesFactory.splitNumber(score, 4);
            } else if (kpi.AdjustRate > 0) {
                if (kpi.AdjustRate === maxStar) {
                    kpi.IsMaxScore = true;
                } else {
                    kpi.IsMaxScore = false;
                }

                var adjustScore = (averageWeight * kpi.AdjustRate) / maxStar;
                kpi.AdjustScore = utilitiesFactory.splitNumber(adjustScore, 4);
            }
        });
    };

    var openPopupNotificationComment = function (message) {
        var modalInstance = $uibModal.open({
            templateUrl: 'SourceApp/Components/EPA/Templates/modalNotification.html',
            controller: "ModalNotificationController as modalNotificationCtrl",
            backdrop: 'static',
            resolve: {
                message: function () {
                    return message;
                }
            },
            windowClass: "modal-confirm-center"
        });

        return modalInstance.result;
    };

    ratingByKPIModel.validateEPA = function (vm) {
        for (var i = 0; i < vm.epaInfos.length; i++) {
            for (var j = 0; j < vm.epaInfos[i].KPIGroup.KPIs.length; j++) {
                var kpi = vm.epaInfos[i].KPIGroup.KPIs[j];

                if (kpi.KPIId === vm.kpiSelected.KPIId
                    && kpi.HasComment && (!kpi.Comment || kpi.Comment.lenght === 0)
                    && (utilitiesFactory.parseFloat(kpi.AdjustScore) > 0 || utilitiesFactory.parseFloat(kpi.Score) > 0)) {
                    if (kpi.IsRequiredComment) {
                        openPopupNotificationComment(vm.evaluateEPAInvalidMessage);
                        return false;
                    } else if (vm.epaInfos[i].RequiredCommentOnMaxScoreForEvaluator && kpi.IsMaxScore) {
                        openPopupNotificationComment(vm.requiredCommentForRatingMaxScore);
                        return false;
                    }
                }
            }
        }

        return true;
    };

    ratingByKPIModel.updateKPIScore = function (vm, modalInstance) {
        return ratingByKPIFactory.updateKPIScore(vm.epaInfos).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
                vm.dataSaved = true;

                if (modalInstance) {
                    modalInstance.close(vm);
                }

                var completedEvaluation = true;
                angular.forEach(vm.epaInfos, function (epaInfo) {
                    angular.forEach(epaInfo.KPIGroup.KPIs, function (kpi) {
                        if (kpi.AdjustRate > 0 || kpi.AdjustScore > 0) {
                            kpi.Rate = kpi.AdjustRate;
                            kpi.Score = kpi.AdjustScore;
                        }

                        if (kpi.Score <= 0 && kpi.KPIId === vm.kpiSelected.KPIId) {
                            completedEvaluation = false;
                        }
                    });

                    if (epaInfo.KPIGroup.HasKPIOptional) {
                        epaInfo.TotalScoreOriginal = epaInfo.TotalScoreOriginal;
                    } else {
                        epaInfo.TotalScoreOriginal = epaInfo.TotalScore;
                    }
                });

                vm.kpiSelected.Evaluated = completedEvaluation;
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return ratingByKPIModel;
}]);