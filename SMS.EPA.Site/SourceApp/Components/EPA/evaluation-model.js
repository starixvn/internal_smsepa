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

        function convertScoreToGrade(performanceScore, disciplineScore, hasDisciplineScore, maxPerformanceScore, standardGrades) {
            if (isNaN(performanceScore) || performanceScore === "N/A") {
                return "N/A";
            }

            performanceScore = utilitiesFactory.parseFloat(performanceScore);
            disciplineScore = utilitiesFactory.parseFloat(disciplineScore);
            var finalScore = checkScore(performanceScore + disciplineScore, 0, maxPerformanceScore);
            var performanceGrade;
            var disciplineGrade;
            var finalGrade;

            var gradeType = {
                Discipline: 1,
                Performance: 2,
                Final: 3
            };

            var belowAverage = 'D';

            if (hasDisciplineScore) {
                angular.forEach(standardGrades, function (standardGrade) {
                    if (standardGrade.Type === gradeType.Discipline && disciplineScore >= standardGrade.LeftScore && disciplineScore <= standardGrade.RightScore) {
                        disciplineGrade = standardGrade.Code.toUpperCase();
                    }

                    if (standardGrade.Type === gradeType.Performance && performanceScore >= standardGrade.LeftScore && performanceScore <= standardGrade.RightScore) {
                        performanceGrade = standardGrade.Code.toUpperCase();
                    }

                    if (standardGrade.Type === gradeType.Final && finalScore >= standardGrade.LeftScore && finalScore <= standardGrade.RightScore) {
                        finalGrade = standardGrade.Code.toUpperCase();
                    }
                });

                if (disciplineGrade.localeCompare(belowAverage) >= 0 || performanceGrade.localeCompare(belowAverage) >= 0) {
                    finalGrade = disciplineGrade.localeCompare(performanceGrade) >= 0 ? disciplineGrade : performanceGrade;
                }
            } else {
                angular.forEach(standardGrades, function (standardGrade) {
                    if (standardGrade.Type === gradeType.Final && finalScore >= standardGrade.LeftScore && finalScore <= standardGrade.RightScore) {
                        finalGrade = standardGrade.Code.toUpperCase();
                    }
                });
            }

            return finalGrade;
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