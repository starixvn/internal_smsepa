namespace SMS.EPA.Model
{
    using Core.Constants;
    using Core.Enumerations;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MonthlyEPAModel
    {
        public MonthlyEPAModel()
        {
            MonthlyEPAs = new List<EPAModel>();
        }

        public IList<EPAModel> MonthlyEPAs { get; set; }

        public float? AverageSelfScore => CalculateAverageScore((int)ProcessType.Self);

        public float? AverageEvaluatorScores => CalculateAverageScore((int)ProcessType.Evaluator);

        public float? AverageReviewerScores => CalculateAverageScore((int)ProcessType.Reviewer);

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
                    default:
                        throw new ArgumentOutOfRangeException();
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