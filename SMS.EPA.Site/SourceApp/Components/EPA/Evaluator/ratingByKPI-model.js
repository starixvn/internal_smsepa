'use strict';
/*global epaModule*/

epaModule.factory('RatingByKPIModel',
['$modal', 'ratingByKPIFactory', 'notificationFactory', 'errorFactory',
function ($modal, ratingByKPIFactory, notificationFactory, errorFactory) {
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

    ratingByKPIModel.getEmployeeEPAByKPI = function (targetTemplateId, kpiId, groupId) {
        return ratingByKPIFactory.getEmployeeEPAByKPI(targetTemplateId, kpiId, groupId)
        .success(function (memberEPAs) {
            ratingByKPIModel.memberEPAs = memberEPAs;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
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

    ratingByKPIModel.calculatePerformanceScore = function (memberEPA) {
        var totalScore = 0;

        angular.forEach(memberEPA.KPIGroup.KPIs, function (kpi) {
            var curentScore = kpi.Score ? parseFloat(kpi.Score) : 0,
                adjustScore = kpi.AdjustScore ? parseFloat(kpi.AdjustScore) : 0;

            totalScore = totalScore - curentScore + adjustScore;
        });

        memberEPA.TotalScore = totalScore.toFixed(2);
    };

    return ratingByKPIModel;
}]);