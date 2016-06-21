namespace SMS.EPA.Model.EmployeeEPA
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Constants;

    public class EvaluationProcessModel
    {
        public EvaluationProcessModel()
        {
            HumanResources = new List<EPAProcessModel>();
            Evaluators = new List<EPAProcessModel>();
            Reviewers = new List<EPAProcessModel>();
            Managements = new List<EPAProcessModel>();
        }

        public float AverageScoreByEvaluators
        {
            get
            {
                return CalculateAverageScore(Evaluators);
            }
        }

        public float AverageScoreByReviewers
        {
            get
            {
                return CalculateAverageScore(Reviewers);
            }
        }

        public float AverageScoreByManagements
        {
            get
            {
                return CalculateAverageScore(Managements);
            }
        }

        public float MaxPerformanceScore
        {
            get
            {
                float maxPerformanceScore = 0;

                if (Managements.Count > 0)
                {
                    maxPerformanceScore = Managements.First().MaxTotalScore;
                }
                else if (Reviewers.Count > 0)
                {
                    maxPerformanceScore = Reviewers.First().MaxTotalScore;
                }
                else if (Evaluators.Count > 0)
                {
                    maxPerformanceScore = Evaluators.First().MaxTotalScore;
                }

                return maxPerformanceScore;
            }
        }

        public float PerformanceScore
        {
            get
            {
                float performanceScore = AverageScoreByEvaluators;

                if (Managements.Count > 0)
                {
                    performanceScore = AverageScoreByManagements;
                }
                else if (Reviewers.Count > 0)
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

        public float CalculateAverageScore(IList<EPAProcessModel> processes)
        {
            float average = 0;
            int count = 0;

            foreach (EPAProcessModel process in processes.Where(x => !string.IsNullOrEmpty(x.TotalScore)))
            {
                count++;
                average += float.Parse(process.TotalScore);
            }

            if (count > 0)
            {
                average = (float)Math.Round(average / count, Constants.NumberOfDecimalDefault);
            }

            return average;
        }
    }
}