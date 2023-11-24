namespace SMS.EPA.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Constants;
    using Core.Enumerations;

    public class MonthlyEPAModel
    {
        public IList<EPAModel> MonthlyEPAs { get; set; } = new List<EPAModel>();

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

            var processes = MonthlyEPAs.Where(x => x.IsClosed).Select(x => x.Process).ToList();
            foreach (var process in processes)
            {
                switch (processType)
                {
                    case (byte)ProcessType.Self:
                        if (!string.IsNullOrEmpty(process.Self.TotalScore))
                        {
                            count++;
                            average += float.Parse(process.Self.TotalScore);
                        }

                        break;

                    case (byte)ProcessType.Evaluator:
                        if (process.AverageScoreByEvaluators.HasValue)
                        {
                            count++;
                            average += process.AverageScoreByEvaluators.Value;
                        }

                        break;

                    case (byte)ProcessType.Reviewer:
                        if (process.AverageScoreByReviewers.HasValue)
                        {
                            count++;
                            average += process.AverageScoreByReviewers.Value;
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