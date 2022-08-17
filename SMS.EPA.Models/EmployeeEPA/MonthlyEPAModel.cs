namespace SMS.EPA.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Constants;
    using Core.Enumerations;

    public class MonthlyEPAModel
    {
        public MonthlyEPAModel()
        {
            MonthlyEPAs = new List<EPAModel>();
        }

        public IList<EPAModel> MonthlyEPAs { get; set; }

        public float? AverageSelfScore => CalculateAverageScore((byte)ProcessType.Self);

        public float? AverageEvaluatorScores => CalculateAverageScore((byte)ProcessType.Evaluator);

        public float? AverageReviewerScores => CalculateAverageScore((byte)ProcessType.Reviewer);

        public float? AverageSelfScoreConverted { get; set; }

        public float? AverageEvaluatorScoreConverted { get; set; }

        public float? AverageReviewerScoreConverted { get; set; }

        private float? CalculateAverageScore(int processType)
        {
            float average = 0;
            int count = 0;
            float? result = null;

            var finishedMonthlyEPAs = MonthlyEPAs.Where(x => x.IsClosed).ToList();
            foreach (EPAModel monthlyEPA in finishedMonthlyEPAs)
            {
                switch (processType)
                {
                    case (byte)ProcessType.Self:
                        if (!string.IsNullOrEmpty(monthlyEPA.Process.Self.TotalScore))
                        {
                            count++;
                            average += float.Parse(monthlyEPA.Process.Self.TotalScore);
                        }

                        break;

                    case (byte)ProcessType.Evaluator:
                        if (monthlyEPA.Process.AverageScoreByEvaluators.HasValue)
                        {
                            count++;
                            average += monthlyEPA.Process.AverageScoreByEvaluators.Value;
                        }

                        break;

                    case (byte)ProcessType.Reviewer:
                        if (monthlyEPA.Process.AverageScoreByReviewers.HasValue)
                        {
                            count++;
                            average += monthlyEPA.Process.AverageScoreByReviewers.Value;
                        }

                        break;

                    default:
                        continue;
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