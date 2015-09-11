'use strict';
/*global epaModule*/

epaModule.factory('RatingByKPIModel',
['$modal', 'ratingByKPIFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory',
function ($modal, ratingByKPIFactory, utilitiesFactory, notificationFactory, errorFactory) {
    var ratingByKPIModel = this;

    ratingByKPIModel.targetTemplates = [];
    ratingByKPIModel.kpis = [];
    ratingByKPIModel.memberEPAs = [];

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
            notificationFactory.error("TargetTemplateRequire");

            return false;
        }
        else if (!filter.kpiId) {
            notificationFactory.error("KPIRequire");

            return false;
        }

        return true;
    };

    ratingByKPIModel.getEmployeeEPAByKPI = function (targetTemplateId, kpiId, groupId) {
        return ratingByKPIFactory.getEmployeeEPAByKPI(targetTemplateId, kpiId, groupId)
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
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    ratingByKPIModel.checkKPIOptional = function (kpiGroup) {
        var hasKPIOptional = false, totalKPIOptional = 0;

        angular.forEach(kpiGroup.KPIs, function (kpi) {
            if (!kpi.IsRequiredEvaluate) {
                hasKPIOptional = true;
                totalKPIOptional += 1;
            }
        });

        kpiGroup.TotalKPIOptional = totalKPIOptional;

        return hasKPIOptional;
    };

    ratingByKPIModel.validateScoreInput = function (score, maxScore, scoreInputValidMessage) {
        var isValid = true;

        if (parseFloat(score) > parseFloat(maxScore)) {
            isValid = false;

            var message = utilitiesFactory.formatString(scoreInputValidMessage, maxScore);
            notificationFactory.error('', message);
        }

        return isValid;
    };

    ratingByKPIModel.calculatePerformanceScore = function (memberEPA, kpiAdjusted) {
        var curentScore = kpiAdjusted.Score ? parseFloat(kpiAdjusted.Score) : 0,
            adjustScore = kpiAdjusted.AdjustScore ? parseFloat(kpiAdjusted.AdjustScore) : 0;

        memberEPA.TotalScore = (memberEPA.TotalScoreOriginal - curentScore + adjustScore).toFixed(2);
    };

    ratingByKPIModel.updateKPIScore = function (memberEPAs, vm) {
        return ratingByKPIFactory.updateKPIScore(memberEPAs)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
                vm.dataSaved = true;
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