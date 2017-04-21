namespace SMS.EPA.Model
{
    using Core.Constants;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EvaluationProcessModel
    {
        public EvaluationProcessModel()
        {
            HumanResources = new List<EPAProcessModel>();
            Evaluators = new List<EPAProcessModel>();
            Reviewers = new List<EPAProcessModel>();
            Managements = new List<EPAProcessModel>();
        }

        public float? AverageScoreByEvaluators => CalculateAverageScore(Evaluators);

        public float? AverageScoreByReviewers => CalculateAverageScore(Reviewers);

        public float? AverageScoreByManagements => CalculateAverageScore(Managements);

        public float MaxPerformanceScore
        {
            get
            {
                float maxPerformanceScore = 0;

                if (Managements.Any())
                {
                    maxPerformanceScore = Managements.First().MaxTotalScore;
                }
                else if (Reviewers.Any())
                {
                    maxPerformanceScore = Reviewers.First().MaxTotalScore;
                }
                else if (Evaluators.Any())
                {
                    maxPerformanceScore = Evaluators.First().MaxTotalScore;
                }

                return maxPerformanceScore;
            }
        }

        public float? PerformanceScore
        {
            get
            {
                float? performanceScore = AverageScoreByEvaluators;

                if (Managements.Any())
                {
                    performanceScore = AverageScoreByManagements;
                }
                else if (Reviewers.Any())
                {
                    performanceScore = AverageScoreByReviewers;
                }

                return performanceScore;
            }
        }

        public EPAProcessModel CurrentProcess { get; set; }

        public IList<EPAProcessModel> HumanResources { get; set; }

        public EPAProcessModel Self { get; set; }

        public IList<EPAProcessModel> Evaluators { get; set; }

        public IList<EPAProcessModel> Reviewers { get; set; }

        public IList<EPAProcessModel> Managements { get; set; }

        public float? CalculateAverageScore(IList<EPAProcessModel> processes)
        {
            float average = 0;
            int count = 0;
            float? result = null;

            foreach (EPAProcessModel process in processes.Where(x => !string.IsNullOrEmpty(x.TotalScore)))
            {
                count++;
                average += float.Parse(process.TotalScore);
            }

            if (count > 0)
            {
                result = (float)Math.Round(average / count, Constants.NumberOfDecimalDefault);
            }

            return result;
        }
    }
}