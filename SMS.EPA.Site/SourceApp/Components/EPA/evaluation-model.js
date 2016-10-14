(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').factory('EvaluationModel', evaluationModel);
    evaluationModel.$inject = ['evaluationFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory'];

    function evaluationModel(evaluationFactory, utilitiesFactory, notificationFactory, errorFactory) {
        var epaInfo = {};

        var model = {
            epaInfo: epaInfo,
            checkScore: checkScore,
            convertScoreToGrade: convertScoreToGrade,
            getEPAInfoById: getEPAInfoById,
            getNotificationInputScore: getNotificationInputScore,
            scoreSpread: scoreSpread,
            validateScoreInput: validateScoreInput
        };

        return model;

        function checkScore(score, minScore, maxScore) {
            var result = score;

            if (score > maxScore) {
                result = maxScore;
            } else if (score < minScore) {
                result = minScore;
            }

            return result;
        }

        function convertScoreToGrade(finalScore, standardGrades) {
            var score = utilitiesFactory.parseFloat(finalScore),
                grade;

            angular.forEach(standardGrades, function (standardGrade) {
                if (score >= standardGrade.LeftScore && score <= standardGrade.RightScore) {
                    grade = standardGrade.Code;
                }
            });

            return grade;
        }

        function getEPAInfoById(epaId) {
            return evaluationFactory.getEPAInfoById(epaId).success(function (epaInfo) {
                model.epaInfo = epaInfo;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getNotificationInputScore(minScore, maxScore, message) {
            return utilitiesFactory.formatString(message, minScore, maxScore);
        }

        function scoreSpread(kpiGroup) {
            var averageWeight;

            if (!kpiGroup.HasKPIOptional && kpiGroup.IsSpreadKPI) {
                averageWeight = kpiGroup.Weight / kpiGroup.KPIs.length;
                averageWeight = utilitiesFactory.splitNumber(averageWeight, 4);

                angular.forEach(kpiGroup.KPIs, function (kpi) {
                    kpi.Weight = averageWeight;
                });
            }
        }

        function validateScoreInput(weight, minScore, maxScore, message) {
            var isValid = true;

            if (weight > maxScore || weight < minScore) {
                isValid = false;

                var msg = utilitiesFactory.formatString(message, minScore, maxScore);
                notificationFactory.error('', msg);
            }

            return isValid;
        }
    }
})(angular);