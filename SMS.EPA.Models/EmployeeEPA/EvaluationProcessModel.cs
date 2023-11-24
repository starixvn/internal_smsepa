namespace SMS.EPA.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Constants;

    public class EvaluationProcessModel
    {
        public float? AverageScoreByEvaluators => CalculateAverageScore(Evaluators);

        public float? AverageScoreByReviewers => CalculateAverageScore(Reviewers);

        public float? AverageScoreByManagements => CalculateAverageScore(Managements);

        public float MaxPerformanceScore
        {
            get
            {
                if (Managements.Any())
                {
                    return Managements[0].MaxTotalScore;
                }

                if (Reviewers.Any())
                {
                    return Reviewers[0].MaxTotalScore;
                }

                if (Evaluators.Any())
                {
                    return Evaluators[0].MaxTotalScore;
                }

                return 0;
            }
        }

        public float? PerformanceScore
        {
            get
            {
                if (Managements.Any() && AverageScoreByManagements != null)
                {
                    return AverageScoreByManagements;
                }

                if (Reviewers.Any() && AverageScoreByReviewers != null)
                {
                    return AverageScoreByReviewers;
                }

                return AverageScoreByEvaluators;
            }
        }

        public EPAProcessModel CurrentProcess { get; set; }

        public IList<EPAProcessModel> HumanResources { get; set; } = new List<EPAProcessModel>();

        public EPAProcessModel Self { get; set; }

        public IList<EPAProcessModel> Evaluators { get; set; } = new List<EPAProcessModel>();

        public IList<EPAProcessModel> Reviewers { get; set; } = new List<EPAProcessModel>();

        public IList<EPAProcessModel> Managements { get; set; } = new List<EPAProcessModel>();

        public static float? CalculateAverageScore(IList<EPAProcessModel> processes)
        {
            float average = 0;
            float? result = null;

            var validProcesses = processes.Where(x => !string.IsNullOrEmpty(x.TotalScore)).ToList();

            if (validProcesses.Any())
            {
                average += validProcesses.Sum(process => float.Parse(process.TotalScore));

                result = (float)Math.Round(average / validProcesses.Count, Constants.NumberOfDecimalDefault);
            }

            return result;
        }
    }
}