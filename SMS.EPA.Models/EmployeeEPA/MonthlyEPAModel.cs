namespace SMS.EPA.Model.EmployeeEPA
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;

    public class MonthlyEPAModel
    {
        public MonthlyEPAModel()
        {
            MonthlyEPAs = new List<EPAModel>();
        }

        public IList<EPAModel> MonthlyEPAs { get; set; }

        public float AverageSelfScore
        {
            get
            {
                return CalculateAverageScore((int)ProcessType.Self);
            }
        }

        public float AverageEvaluatorScores
        {
            get
            {
                return CalculateAverageScore((int)ProcessType.Evaluator);
            }
        }

        public float AverageReviewerScores
        {
            get
            {
                return CalculateAverageScore((int)ProcessType.Reviewer);
            }
        }

        public float AverageSelfScoreConverted { get; set; }

        public float AverageEvaluatorScoreConverted { get; set; }

        public float AverageReviewerScoreConverted { get; set; }

        private float CalculateAverageScore(int processType)
        {
            float score = 0, average = 0;
            int count = 0;

            foreach (EPAModel monthlyEPA in MonthlyEPAs)
            {
                if (monthlyEPA.Status.StatusId == (int)Status.Closed)
                {
                    switch (processType)
                    {
                        case (int)ProcessType.Self:
                            if (!string.IsNullOrEmpty(monthlyEPA.ProcessBySelf.TotalScore))
                            {
                                count++;
                                score += float.Parse(monthlyEPA.ProcessBySelf.TotalScore);
                            }

                            break;

                        case (int)ProcessType.Evaluator:
                            if (monthlyEPA.AverageScoreByEvaluators > 0)
                            {
                                count++;
                                score += monthlyEPA.AverageScoreByEvaluators;
                            }

                            break;

                        case (int)ProcessType.Reviewer:
                            if (monthlyEPA.AverageScoreByReviewers > 0)
                            {
                                count++;
                                score += monthlyEPA.AverageScoreByReviewers;
                            }

                            break;
                    }
                }
            }

            if (count > 0)
            {
                average = score / count;
                return (float)Math.Round(average, Constants.NumberOfDecimalDefault);
            }

            return score;
        }
    }
}