'use strict';
/*global epaModule*/

epaModule.factory('RatingByKPIModel',
['$modal', 'ratingByKPIFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory',
function ($modal, ratingByKPIFactory, utilitiesFactory, notificationFactory, errorFactory) {
    var ratingByKPIModel = this;

    ratingByKPIModel.groups = [];
    ratingByKPIModel.targetTemplates = [];
    ratingByKPIModel.kpis = [];
    ratingByKPIModel.memberEPAs = [];

    ratingByKPIModel.getGroupByEvaluator = function () {
        return ratingByKPIFactory.getGroupByEvaluator()
        .success(function (groups) {
            ratingByKPIModel.groups = groups;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    ratingByKPIModel.getTargetTemplateByEvaluator = function () {
        return ratingByKPIFactory.getTargetTemplateByEvaluator()
        .success(function (targetTemplates) {
            ratingByKPIModel.targetTemplates = targetTemplates;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    ratingByKPIModel.getKPIByTargetTemplate = function (targetTemplateId) {
        return ratingByKPIFactory.getKPIByTargetTemplate(targetTemplateId)
        .success(function (kpis) {
            ratingByKPIModel.kpis = kpis;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    ratingByKPIModel.validateFieldFilter = function (filter) {
        if (!filter.targetTemplateId) {
            notificationFactory.error("TargetTemplateRequired");

            return false;
        }
        else if (!filter.kpiId) {
            notificationFactory.error("KPIRequired");

            return false;
        }

        return true;
    };

    ratingByKPIModel.getEmployeeEPAByKPI = function (targetTemplateId, kpiId, groupId, employeeIds) {
        return ratingByKPIFactory.getEmployeeEPAByKPI(targetTemplateId, kpiId, groupId, employeeIds)
        .success(function (memberEPAs) {
            ratingByKPIModel.memberEPAs = memberEPAs;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    ratingByKPIModel.openPopupConfirmSaveChanges = function (vm) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/EPA/RatingByKPI/Templates/modalConfirmSaveChanges.html',
            controller: "ModalConfirmSaveChangesController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                kpiSelected: function(){
                    return vm.kpiSelected;
                },
                memberEPAs: function () {
                    return vm.memberEPAs;
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
        var score, averageWeight, totalKPIMustEvaluate, totalStarMustEvaluate, totalStarEvaluated = 0,
            numberKPIOptional = kpiGroup.NumberKPIOptional;
        var kpis = [];

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

        totalKPIMustEvaluate = kpiGroup.KPIs.length - numberKPIOptional;
        totalStarMustEvaluate = totalKPIMustEvaluate * maxStar;
        averageWeight = utilitiesFactory.parseFloat(kpiGroup.Weight / totalKPIMustEvaluate);
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
                kpi.Score = utilitiesFactory.parseFloat(averageWeight * (kpi.Rate / maxStar));
            } else if (kpi.AdjustRate > 0) {
                if (kpi.AdjustRate === maxStar) {
                    kpi.IsMaxScore = true;
                } else {
                    kpi.IsMaxScore = false;
                }
                kpi.AdjustScore = utilitiesFactory.parseFloat(averageWeight * (kpi.AdjustRate / maxStar));
            }
        });

        if (((averageWeight * totalKPIMustEvaluate) > kpiGroup.Weight) && (totalStarEvaluated === totalStarMustEvaluate)) {
            score = averageWeight - ((averageWeight * totalKPIMustEvaluate) - kpiGroup.Weight);
            kpis[kpis.length - 1].Weight = utilitiesFactory.parseFloat(score);
            kpis[kpis.length - 1].AdjustScore = kpis[kpis.length - 1].Weight;
        } else if (((averageWeight * totalKPIMustEvaluate) < kpiGroup.Weight) && (totalStarEvaluated === totalStarMustEvaluate)) {
            score = averageWeight + (kpiGroup.Weight - (averageWeight * totalKPIMustEvaluate));
            kpis[kpis.length - 1].Weight = utilitiesFactory.parseFloat(score);
            kpis[kpis.length - 1].AdjustScore = kpis[kpis.length - 1].Weight;
        }
    };

    var openPopupNotificationComment = function (message) {
        var modalInstance = $modal.open({
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
        var i = 0, j = 0;

        for (i = 0; i < vm.memberEPAs.length; i++) {
            for (j = 0; j < vm.memberEPAs[i].KPIGroup.KPIs.length; j++) {
                var kpi = vm.memberEPAs[i].KPIGroup.KPIs[j];

                if (kpi.KPIId === vm.kpiSelected.KPIId
                    && kpi.HasComment && (!kpi.Comment || kpi.Comment.lenght === 0)
                    && (utilitiesFactory.parseFloat(kpi.AdjustScore) > 0 || utilitiesFactory.parseFloat(kpi.Score) > 0)) {
                    if (kpi.IsRequiredComment) {
                        openPopupNotificationComment(vm.evaluateEPAInvalidMessage);
                        return false;
                    } else if (vm.memberEPAs[i].RequiredCommentOnMaxScore && kpi.IsMaxScore) {
                        openPopupNotificationComment(vm.requiredCommentForMaxScoreRating);
                        return false;
                    }
                }
            }
        }

        return true;
    };

    ratingByKPIModel.updateKPIScore = function (vm, modalInstance) {
        return ratingByKPIFactory.updateKPIScore(vm.memberEPAs)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
                vm.dataSaved = true;

                if (modalInstance) {
                    modalInstance.close(vm);
                }

                angular.forEach(vm.memberEPAs, function (memberEPA) {

                    angular.forEach(memberEPA.KPIGroup.KPIs, function (kpi) {
                        if (kpi.AdjustRate > 0 || kpi.AdjustScore > 0) {
                            kpi.Rate = kpi.AdjustRate;
                            kpi.Score = kpi.AdjustScore;
                        }
                    });

                    if (memberEPA.KPIGroup.HasKPIOptional) {
                        memberEPA.TotalScoreOriginal = memberEPA.TotalScoreOriginal;
                    }
                    else {
                        memberEPA.TotalScoreOriginal = memberEPA.TotalScore;
                    }
                });
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