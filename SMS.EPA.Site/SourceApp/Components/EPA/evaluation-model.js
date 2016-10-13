'use strict';
/*global epaModule*/

epaModule.factory('EvaluationModel',
['evaluationFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory',
function (evaluationFactory, utilitiesFactory, notificationFactory, errorFactory) {
    var model = this;

    model.epaInfo = {};

    model.getEPAInfoById = function (epaId) {
        return evaluationFactory.getEPAInfoById(epaId).success(function (epaInfo) {
            model.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    model.getNotificationInputScore = function (minScore, maxScore, message) {
        return utilitiesFactory.formatString(message, minScore, maxScore);
    };

    model.validateScoreInput = function (weight, minScore, maxScore, message) {
        var isValid = true;

        if (weight > maxScore || weight < minScore) {
            isValid = false;

            var msg = utilitiesFactory.formatString(message, minScore, maxScore);
            notificationFactory.error('', msg);
        }

        return isValid;
    };

    model.convertScoreToGrade = function (finalScore, standardGrades) {
        var score = utilitiesFactory.parseFloat(finalScore),
            grade;

        angular.forEach(standardGrades, function (standardGrade) {
            if (score >= standardGrade.LeftScore && score <= standardGrade.RightScore) {
                grade = standardGrade.Code;
            }
        });

        return grade;
    };

    model.scoreSpread = function (kpiGroup) {
        var averageWeight;

        if (!kpiGroup.HasKPIOptional && kpiGroup.IsSpreadKPI) {
            averageWeight = kpiGroup.Weight / kpiGroup.KPIs.length;
            averageWeight = utilitiesFactory.splitNumber(averageWeight, 4);

            angular.forEach(kpiGroup.KPIs, function (kpi) {
                kpi.Weight = averageWeight;
            });
        }
    };

    model.checkScore = function (score, minScore, maxScore) {
        var result = score;

        if (score > maxScore) {
            result = maxScore;
        } else if (score < minScore) {
            result = minScore;
        }

        return result;
    };

    return model;
}]);