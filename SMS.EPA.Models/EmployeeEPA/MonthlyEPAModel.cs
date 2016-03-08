namespace SMS.EPA.Model.EmployeeEPA
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.Core.Enumerations;

    public class MonthlyEPAModel
    {
        public MonthlyEPAModel()
        {
            MonthlyEPAs = new List<EPAModel>();
        }

        public IList<EPAModel> MonthlyEPAs { get; set; }

        public float AverageMonthlySelfScore
        {
            get
            {
                return CalculateAverageScore((int)ProcessType.Self);
            }
        }

        public float AverageMonthlyEvaluatorScores
        {
            get
            {
                return CalculateAverageScore((int)ProcessType.Evaluator);
            }
        }

        public float AverageMonthlyReviewerScores
        {
            get
            {
                return CalculateAverageScore((int)ProcessType.Reviewer);
            }
        }

        public float AverageMonthlySelfScoreConversion { get; set; }

        public float AverageMonthlyEvaluatorScoreConversion { get; set; }

        public float AverageMonthlyReviewerScoreConversion { get; set; }

        private float CalculateAverageScore(int processType)
        {
            float score = 0, average = 0;
            int count = 0;

            foreach (EPAModel monthlyEPA in MonthlyEPAs)
            {
                if (monthlyEPA.Status.StatusId == 1)
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
                return (float)Math.Round(average, 2);
            }

            return score;
        }
    }
}