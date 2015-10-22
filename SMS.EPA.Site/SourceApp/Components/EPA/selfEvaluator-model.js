'use strict';
/*global epaModule*/

epaModule.factory('SelfEvalutorModel',
['$modal', 'EmployeeEPAModel', 'utilitiesFactory',
function ($modal, EmployeeEPAModel, utilitiesFactory) {
    var model = this;

    model.calculateScore = function (vm) {
        var totalScore = 0;

        angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIs, function (kpi) {
                var rate = utilitiesFactory.parseFloat(kpi.CurrentEvaluation.Rate);
                var score = utilitiesFactory.parseFloat(kpi.CurrentEvaluation.Score);
                totalScore += score;

                if (score === kpi.Weight || rate === 5) {
                    kpi.CurrentEvaluation.IsMaxScore = true;
                }
                else {
                    kpi.CurrentEvaluation.IsMaxScore = false;
                }
            });
        });

        vm.epaInfo.CurrentProcess.TotalScore = totalScore;
        vm.finalScore = totalScore;

        if (vm.epaInfo.HasDisciplineScore) {
            vm.finalScore += vm.epaInfo.DisciplineScore;
        }

        if (vm.finalScore > vm.epaInfo.MaxFinalScore) {
            vm.finalScore = vm.epaInfo.MaxFinalScore;
        }
        else if (vm.finalScore < 0) {
            vm.finalScore = 0;
        }

        vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
    };

    model.scoreSpreadForKPIGroupHasKPIOptional = function (kpiGroup, maxStar) {
        var score, averageWeight, totalKPIMustEvaluate, totalStarMustEvaluate, totalStarEvaluated = 0,
            numberKPIOptional = kpiGroup.NumberKPIOptional;
        var kpis = [];

        angular.forEach(kpiGroup.KPIs, function (kpi) {
            if (kpi.CurrentEvaluation.Rate > 0) {
                kpis.push(kpi);
                totalStarEvaluated += kpi.CurrentEvaluation.Rate;
            }

            if (!kpi.IsRequiredEvaluate && kpi.CurrentEvaluation.Rate > 0) {
                numberKPIOptional -= 1;
            }
        });

        totalKPIMustEvaluate = kpiGroup.KPIs.length - numberKPIOptional;
        totalStarMustEvaluate = totalKPIMustEvaluate * maxStar;
        averageWeight = utilitiesFactory.parseFloat(kpiGroup.Weight / totalKPIMustEvaluate);

        angular.forEach(kpis, function (kpi) {
            kpi.CurrentEvaluation.Score = utilitiesFactory.parseFloat(averageWeight * (kpi.CurrentEvaluation.Rate / maxStar));
        });

        if (((averageWeight * totalKPIMustEvaluate) > kpiGroup.Weight) && totalStarEvaluated === totalStarMustEvaluate) {
            score = utilitiesFactory.parseFloat(averageWeight - ((averageWeight * totalKPIMustEvaluate) - kpiGroup.Weight));
            kpis[kpis.length - 1].CurrentEvaluation.Score = score;
        } else if ((averageWeight * totalKPIMustEvaluate) < kpiGroup.Weight && totalStarEvaluated === totalStarMustEvaluate) {
            score = utilitiesFactory.parseFloat(averageWeight + (kpiGroup.Weight - (averageWeight * totalKPIMustEvaluate)));
            kpis[kpis.length - 1].CurrentEvaluation.Score = score;
        }
    };

    model.validateEPA = function (epaInfo) {
        var isValid = true;

        angular.forEach(epaInfo.KPIGroups, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIs, function (kpi) {
                if (kpi.IsRequiredEvaluate && (!kpi.CurrentEvaluation.Score || utilitiesFactory.parseFloat(kpi.CurrentEvaluation.Score) <= 0)) {
                    isValid = false;
                    kpi.CurrentEvaluation.EvaluateInvalid = true;
                } else {
                    kpi.CurrentEvaluation.EvaluateInvalid = false;
                }

                if (kpi.HasComment && (!kpi.CurrentEvaluation.Comment || kpi.CurrentEvaluation.Comment.length === 0)
                    && (kpi.IsRequiredComment || (epaInfo.RequiredCommentOnMaxScore && !kpi.IsRequiredComment && kpi.CurrentEvaluation.IsMaxScore))) {
                    isValid = false;
                    kpi.CurrentEvaluation.CommentInvalid = true;
                    kpi.ExpandedComment = true;
                } else {
                    kpi.CurrentEvaluation.CommentInvalid = false;
                }
            });
        });

        return isValid;
    };

    return model;
}]);