(function(angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').factory('EvaluationModel', evaluationModel);
    evaluationModel.$inject = [
        'evaluationFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory', "CommonModel"
    ];

    function evaluationModel(evaluationFactory, utilitiesFactory, notificationFactory, errorFactory, CommonModel) {
        var epaInfo = {};

        CommonModel.getStandardGrade();

        var standardGrades = CommonModel.standardGrades;

        var model = {
            epaInfo: epaInfo,
            checkScore: checkScore,
            convertScoreToGrade: convertScoreToGrade,
            getEPAInfoById: getEPAInfoById,
            getNotificationInputScore: getNotificationInputScore,
            scoreSpread: scoreSpread,
            validateScoreInput: validateScoreInput,
            calculateScoreAndGradeToDisplay: calculateScoreAndGradeToDisplay,
            getMultiScoreDatas: getMultiScoreDatas,
            getScoreData: getScoreData
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

        function convertScoreToGrade(performanceScore,
            disciplineScore,
            hasDisciplineScore,
            maxPerformanceScore,
            standardGrades) {
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
                angular.forEach(standardGrades,
                    function(standardGrade) {
                        if (standardGrade.Type === gradeType.Discipline &&
                            disciplineScore >= standardGrade.LeftScore &&
                            disciplineScore <= standardGrade.RightScore) {
                            disciplineGrade = standardGrade.Code.toUpperCase();
                        }

                        if (standardGrade.Type === gradeType.Performance &&
                            performanceScore >= standardGrade.LeftScore &&
                            performanceScore <= standardGrade.RightScore) {
                            performanceGrade = standardGrade.Code.toUpperCase();
                        }

                        if (standardGrade.Type === gradeType.Final &&
                            finalScore >= standardGrade.LeftScore &&
                            finalScore <= standardGrade.RightScore) {
                            finalGrade = standardGrade.Code.toUpperCase();
                        }
                    });

                if (disciplineGrade.localeCompare(belowAverage) >= 0 ||
                    performanceGrade.localeCompare(belowAverage) >= 0) {
                    finalGrade = disciplineGrade.localeCompare(performanceGrade) >= 0
                        ? disciplineGrade
                        : performanceGrade;
                }
            } else {
                angular.forEach(standardGrades,
                    function(standardGrade) {
                        if (standardGrade.Type === gradeType.Final &&
                            finalScore >= standardGrade.LeftScore &&
                            finalScore <= standardGrade.RightScore) {
                            finalGrade = standardGrade.Code.toUpperCase();
                        }
                    });
            }

            return finalGrade;
        }

        function getEPAInfoById(epaId) {
            return evaluationFactory.getEPAInfoById(epaId).success(function(epaInfo) {
                model.epaInfo = epaInfo;
            }).error(function(response, status) {
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

                angular.forEach(kpiGroup.KPIs,
                    function(kpi) {
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

        function calculateScoreAndGradeToDisplay(epaInfo) {
            var result = {
                SelfScore: utilitiesFactory.notAvailableString,
                EvaluatorScore: utilitiesFactory.notAvailableString,
                ReviewerScore: utilitiesFactory.notAvailableString,
                ManagementScore: utilitiesFactory.notAvailableString,
                DisciplineScore: utilitiesFactory.notAvailableString,
                FinalScore: utilitiesFactory.notAvailableString,
                Grade: utilitiesFactory.notAvailableString
            };

            if (epaInfo.HasDisciplineScore) {
                result.DisciplineScore = epaInfo.DisciplineScore;
            } else {
                if (epaInfo.DisciplineScore !== 0) {
                    result.DisciplineScore = epaInfo.DisciplineScore;
                } else {
                    result.DisciplineScore = "-";
                }
            }

            if (epaInfo.EPAId > 0) {
                switch (epaInfo.Process.Self.StatusId) {
                case utilitiesFactory.processStatus.InProgress:
                case utilitiesFactory.processStatus.Closed:
                    result.SelfScore = epaInfo.Process.Self.TotalScore.length > 0
                        ? utilitiesFactory.parseFloat(epaInfo.Process.Self.TotalScore)
                        : utilitiesFactory.notAvailableString;
                    break;
                }

                switch (epaInfo.Process.Evaluators[0].StatusId) {
                case utilitiesFactory.processStatus.InProgress:
                case utilitiesFactory.processStatus.Closed:
                    result.EvaluatorScore = epaInfo.Process.AverageScoreByEvaluators !== null
                        ? epaInfo.Process.AverageScoreByEvaluators
                        : utilitiesFactory.notAvailableString;
                    break;
                }

                switch (epaInfo.Process.Reviewers[0].StatusId) {
                case utilitiesFactory.processStatus.InProgress:
                case utilitiesFactory.processStatus.Closed:
                    result.ReviewerScore = epaInfo.Process.AverageScoreByReviewers !== null
                        ? epaInfo.Process.AverageScoreByReviewers
                        : utilitiesFactory.notAvailableString;
                    break;
                }

                if (epaInfo.Process.Managements.length > 0) {
                    switch (epaInfo.Process.Managements[0].StatusId) {
                    case utilitiesFactory.processStatus.InProgress:
                    case utilitiesFactory.processStatus.Closed:
                        result.ManagementScore = epaInfo.Process.AverageScoreByManagements !== null
                            ? epaInfo.Process.AverageScoreByManagements
                            : utilitiesFactory.notAvailableString;
                        break;
                    }
                }

                if (epaInfo.IsSemiHasMonthly) {
                    result.SelfScore = epaInfo.MonthlyEPA.AverageSelfScoreConverted !== null
                        ? epaInfo.MonthlyEPA.AverageSelfScoreConverted
                        : utilitiesFactory.notAvailableString;

                    result.EvaluatorScore = epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted !== null
                        ? epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted
                        : utilitiesFactory.notAvailableString;
                }
            }

            var tempDisciplineScore = 0;
            if (result.DisciplineScore !== "-") {
                tempDisciplineScore = result.DisciplineScore;
            }

            // Get Standard grades if empty
            if (standardGrades.length === 0) {
                CommonModel.getStandardGrade();
                standardGrades = CommonModel.standardGrades;
            }

            result.FinalScore = epaInfo.FinalScoreToExport;

            if (result.FinalScore !== "") {
                result.Grade = convertScoreToGrade(
                    result.FinalScore - tempDisciplineScore,
                    tempDisciplineScore,
                    epaInfo.HasDisciplineScore,
                    100,
                    standardGrades);
            } else {
                // Set default lowest value
                result.FinalScore = utilitiesFactory.notAvailableString;
                result.Grade = utilitiesFactory.notAvailableString;
            }

            return result;
        }

        function getPhaseScore(phaseScore, tempDisciplineScore) {
            if (phaseScore === utilitiesFactory.notAvailableString) {
                return utilitiesFactory.notAvailableString;
            } else {
                return utilitiesFactory.parseFloat(phaseScore + tempDisciplineScore);
            }
        }

        function isShowScoreOfProcess(actors) {
            if (actors.length > 0) {
                if (actors[0].StatusId === utilitiesFactory.processStatus.InProgress ||
                    actors[0].StatusId === utilitiesFactory.processStatus.Closed)
                    if (actors[0].Evaluated) {
                        return true;
                    }
            }
            return false;
        }

        function getMultiScoreDatas($filter, actors, maxScore) {
            var data = [];
            var index;
            for (index = 0; index < actors.length; index++) {
                var actorScore = getScoreData($filter,
                    actors[index].StatusId,
                    actors[index].TotalScore,
                    maxScore,
                    actors[index].Comment);
                data.push(actorScore);
            }

            return data;
        }

        function getScoreData($filter, statusId, score, maxScore, comment) {
            return {
                IsShowed: isScoreShowed(statusId),
                IsCommentShowed: isCommentShowed(statusId, comment),
                Class: getScoreClass(statusId),
                Score: getScoreValue($filter, score, maxScore, statusId)
            };
        }

        function isScoreShowed(statusId) {
            //if (statusId !== utilitiesFactory.processStatus.Open) {
            //    return true;
            //} else {
            //    return false;
            //}
            return true;
        }

        function isCommentShowed(statusId, comment) {
            if (comment !== undefined && comment !== null) {
                if ((statusId === utilitiesFactory.processStatus.Closed ||
                        statusId === utilitiesFactory.processStatus.InProgress) &&
                    comment.length > 0) {
                    return true;
                }
            }

            return false;
        }

        function getScoreClass(statusId) {
            if (statusId === utilitiesFactory.processStatus.InProgress) {
                return "red";
            }
            return "";
        }

        function getScoreValue($filter, score, maxScore, statusId) {
            if (score === "" || score === null || statusId === utilitiesFactory.processStatus.Open) {
                return "N/A";
            }

            return $filter("customScore")(score) + " / " + $filter("customScore")(maxScore);
        }
    }
})(angular);