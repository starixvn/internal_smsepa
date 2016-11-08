namespace SMS.EPA.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;

    public class MonthlyEPAModel
    {
        public MonthlyEPAModel()
        {
            MonthlyEPAs = new List<EPAModel>();
        }

        public IList<EPAModel> MonthlyEPAs { get; set; }

        public float? AverageSelfScore
        {
            get
            {
                return CalculateAverageScore((int)ProcessType.Self);
            }
        }

        public float? AverageEvaluatorScores
        {
            get
            {
                return CalculateAverageScore((int)ProcessType.Evaluator);
            }
        }

        public float? AverageReviewerScores
        {
            get
            {
                return CalculateAverageScore((int)ProcessType.Reviewer);
            }
        }

        public float? AverageSelfScoreConverted { get; set; }

        public float? AverageEvaluatorScoreConverted { get; set; }

        public float? AverageReviewerScoreConverted { get; set; }

        private float? CalculateAverageScore(int processType)
        {
            float average = 0;
            int count = 0;
            float? result = null;

            var finishedMonthlyEPAs = MonthlyEPAs.Where(x => x.Period.IsEndPeriod);
            foreach (EPAModel monthlyEPA in finishedMonthlyEPAs)
            {
                switch (processType)
                {
                    case (int)ProcessType.Self:
                        if (!string.IsNullOrEmpty(monthlyEPA.Process.Self.TotalScore))
                        {
                            count++;
                            average += float.Parse(monthlyEPA.Process.Self.TotalScore);
                        }

                        break;

                    case (int)ProcessType.Evaluator:
                        if (monthlyEPA.Process.AverageScoreByEvaluators.HasValue)
                        {
                            count++;
                            average += monthlyEPA.Process.AverageScoreByEvaluators.Value;
                        }

                        break;

                    case (int)ProcessType.Reviewer:
                        if (monthlyEPA.Process.AverageScoreByReviewers.HasValue)
                        {
                            count++;
                            average += monthlyEPA.Process.AverageScoreByReviewers.Value;
                        }

                        break;
                }
            }

            if (count > 0)
            {
                result = (float)Math.Round(average / count, Constants.NumberOfDecimalDefault);
            }

            return result;
        }
    }
}