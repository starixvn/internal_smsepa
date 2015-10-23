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
            templateUrl: 'SourceApp/Components/EPA/Evaluator/Templates/modalConfirmSaveChanges.html',
            controller: "ModalConfirmSaveChangesController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
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
            }
            else if (kpi.AdjustRate > 0) {
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
            if (kpi.Rate > 0 && !kpi.AdjustRate) {
                kpi.Score = utilitiesFactory.parseFloat(averageWeight * (kpi.Rate / maxStar));
            }
            else if (kpi.AdjustRate > 0) {
                kpi.AdjustScore = utilitiesFactory.parseFloat(averageWeight * (kpi.AdjustRate / maxStar));
            }
        });

        if (((averageWeight * totalKPIMustEvaluate) > kpiGroup.Weight) && totalStarEvaluated === totalStarMustEvaluate) {
            score = utilitiesFactory.parseFloat(averageWeight - ((averageWeight * totalKPIMustEvaluate) - kpiGroup.Weight));
            kpis[kpis.length - 1].AdjustScore = score;
        } else if ((averageWeight * totalKPIMustEvaluate) < kpiGroup.Weight && totalStarEvaluated === totalStarMustEvaluate) {
            score = utilitiesFactory.parseFloat(averageWeight + (kpiGroup.Weight - (averageWeight * totalKPIMustEvaluate)));
            kpis[kpis.length - 1].AdjustScore = score;
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
        var isValid = true;

        if (!vm.kpiSelected.KPIGroup.HasKPIOptional) {
            angular.forEach(vm.memberEPAs, function (memberEPA) {
                angular.forEach(memberEPA.KPIGroup.KPIs, function (kpi) {
                    if (kpi.HasComment && kpi.IsRequiredComment
                        && (utilitiesFactory.parseFloat(kpi.AdjustScore) > 0 || utilitiesFactory.parseFloat(kpi.Score) > 0)
                        && (!kpi.Comment || kpi.Comment.lenght === 0)) {
                        isValid = false;
                    }
                });
            });

            if (!isValid) {
                openPopupNotificationComment(vm.evaluateEPAInvalidMessage);
            }
        } else {
            angular.forEach(vm.memberEPAs, function (memberEPA) {
                angular.forEach(memberEPA.KPIGroup.KPIs, function (kpi) {
                    if (kpi.HasComment && memberEPA.RequiredCommentOnMaxScore && kpi.IsMaxScore
                        && (utilitiesFactory.parseFloat(kpi.AdjustScore) > 0 || utilitiesFactory.parseFloat(kpi.Score) > 0)
                        && (!kpi.Comment || kpi.Comment.lenght === 0)) {
                        isValid = false;
                    }
                });
            });

            if (!isValid) {
                openPopupNotificationComment(vm.requiredCommentForMaxScoreRating);
            }
        }

        return isValid;
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