'use strict';
(function (angular) {
    function EPAByKPIModel(epaByKPIFactory, utilitiesFactory, notificationFactory, errorFactory) {
        var epaByKPIModel = {
            targetTemplates: [],
            kpis: [],
            evaluators: [],
            memberEPAs: []
        };

        function getTargetTemplateToView() {
            return epaByKPIFactory.getTargetTemplateToView()
            .success(function (targetTemplates) {
                epaByKPIModel.targetTemplates = targetTemplates;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getKPIToView(targetTemplateId, evaluatorId) {
            return epaByKPIFactory.getKPIToView(targetTemplateId, evaluatorId)
            .success(function (kpis) {
                epaByKPIModel.kpis = kpis;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getEmployeeEPAToView(targetTemplateId, kpiId, evaluatorId) {
            return epaByKPIFactory.getEmployeeEPAToView(targetTemplateId, kpiId, evaluatorId)
            .success(function (memberEPAs) {
                epaByKPIModel.memberEPAs = memberEPAs;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getEvaluatorsByTargetTemplate(targetTemplateId) {
            return epaByKPIFactory.getEvaluatorsByTargetTemplate(targetTemplateId)
            .success(function (evaluators) {
                epaByKPIModel.evaluators = evaluators;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function scoreSpreadForKPIGroupHasKPIOptional(kpiGroup, maxStar) {
            var averageWeight, totalKPIMustEvaluate, totalStarMustEvaluate, totalStarEvaluated = 0,
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
                    var score = averageWeight * (kpi.Rate / maxStar);
                    kpi.Score = utilitiesFactory.splitNumber(score, 4);
                } else if (kpi.AdjustRate > 0) {
                    if (kpi.AdjustRate === maxStar) {
                        kpi.IsMaxScore = true;
                    } else {
                        kpi.IsMaxScore = false;
                    }
                    var adjustScore = averageWeight * (kpi.AdjustRate / maxStar);
                    kpi.AdjustScore = utilitiesFactory.splitNumber(adjustScore, 4);
                }
            });
        }

        epaByKPIModel.getTargetTemplateToView = getTargetTemplateToView;
        epaByKPIModel.getKPIToView = getKPIToView;
        epaByKPIModel.getEmployeeEPAToView = getEmployeeEPAToView;
        epaByKPIModel.getEvaluatorsByTargetTemplate = getEvaluatorsByTargetTemplate;
        epaByKPIModel.scoreSpreadForKPIGroupHasKPIOptional = scoreSpreadForKPIGroupHasKPIOptional;

        return epaByKPIModel;
    }

    EPAByKPIModel.$inject = ['epaByKPIFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory'];

    angular
    .module('epaModule')
    .factory('EPAByKPIModel', EPAByKPIModel);

})(angular);
