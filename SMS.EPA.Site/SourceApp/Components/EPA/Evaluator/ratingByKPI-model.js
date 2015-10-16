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

    ratingByKPIModel.validateScoreInput = function (score, MaxTotalScore, scoreInputValidMessage) {
        var isValid = true;

        if (parseFloat(score) > parseFloat(MaxTotalScore) || parseFloat(score) < 0) {
            isValid = false;

            var message = utilitiesFactory.formatString(scoreInputValidMessage, MaxTotalScore);
            notificationFactory.error('', message);
        }

        return isValid;
    };

    ratingByKPIModel.calculatePerformanceScore = function (memberEPA, kpiAdjusted) {
        var totalScore = memberEPA.TotalScoreOriginal ? parseFloat(memberEPA.TotalScoreOriginal) : 0;
        var curentScore = kpiAdjusted.Score ? parseFloat(kpiAdjusted.Score) : 0,
            adjustScore = kpiAdjusted.AdjustScore ? parseFloat(kpiAdjusted.AdjustScore) : 0;

        memberEPA.TotalScore = (totalScore - curentScore + adjustScore).toFixed(2);
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
                        && (kpi.AdjustRate > 0 || kpi.AdjustScore > 0)
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
                    memberEPA.TotalScoreOriginal = memberEPA.TotalScore;

                    angular.forEach(memberEPA.KPIGroup.KPIs, function (kpi) {
                        if (kpi.AdjustRate > 0 || kpi.AdjustScore > 0) {
                            kpi.Rate = kpi.AdjustRate;
                            kpi.Score = kpi.AdjustScore;
                        }
                    });
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