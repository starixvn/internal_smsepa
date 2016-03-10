// -----------------------------------------------------------------------
// <copyright file="EPADashboardModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.04">Created</change>
// </history>

namespace SMS.EPA.Model.Report.Dashboard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Enitites;

    public class EPADashboardModel
    {
        public EPADashboardModel()
        {
            EvaluationProcess = new EvaluationProcessModel();
            ProcessByEvaluators = new List<EPAProcessModel>();
            ProcessByReviewers = new List<EPAProcessModel>();
            ProcessByManagements = new List<EPAProcessModel>();
        }

        public int EPAId { get; set; }

        public StatusModel Status { get; set; }

        public EPAPeriodModel Period { get; set; }

        public string FinalScoreEncode { get; set; }

        public string FinalScore { get; set; }

        public int MaxFinalScore
        {
            get
            {
                return Constants.MaxFinalScore;
            }
        }

        public bool IsEndProcessBySelf { get; set; }

        public bool HasDisciplineScore { get; set; }

        public float DisciplineScore { get; set; }

        public float MaxDisciplineScore { get; set; }

        public bool HasOtherViolation { get; set; }

        public bool HasMonthlyEPA { get; set; }

        public EvaluationProcessModel EvaluationProcess { get; set; }

        public EPAProcessModel ProcessBySelf { get; set; }

        public IList<EPAProcessModel> ProcessByEvaluators { get; set; }

        public IList<EPAProcessModel> ProcessByReviewers { get; set; }

        public IList<EPAProcessModel> ProcessByManagements { get; set; }

        public float AverageScoreByEvaluators
        {
            get
            {
                return CalculateAverageScore(ProcessByEvaluators);
            }
        }

        public float AverageScoreByReviewers
        {
            get
            {
                return CalculateAverageScore(ProcessByReviewers);
            }
        }

        public float AverageScoreByManagements
        {
            get
            {
                return CalculateAverageScore(ProcessByManagements);
            }
        }

        public float MaxPerformanceScore
        {
            get
            {
                float maxPerformanceScore = 0;

                if (ProcessByManagements.Count > 0)
                {
                    maxPerformanceScore = ProcessByManagements.First().MaxTotalScore;
                }
                else if (ProcessByReviewers.Count > 0)
                {
                    maxPerformanceScore = ProcessByReviewers.First().MaxTotalScore;
                }
                else if (ProcessByEvaluators.Count > 0)
                {
                    maxPerformanceScore = ProcessByEvaluators.First().MaxTotalScore;
                }

                return maxPerformanceScore;
            }
        }

        public float PerformanceScore
        {
            get
            {
                float performanceScore = AverageScoreByEvaluators;

                if (ProcessByManagements.Count > 0)
                {
                    performanceScore = AverageScoreByManagements;
                }
                else if (ProcessByReviewers.Count > 0)
                {
                    performanceScore = AverageScoreByReviewers;
                }

                return performanceScore;
            }
        }

        private float CalculateAverageScore(IList<EPAProcessModel> processes)
        {
            float avegareScore = 0, score = 0;
            int count = 0;

            foreach (EPAProcessModel process in processes)
            {
                if (!string.IsNullOrEmpty(process.TotalScore))
                {
                    count++;
                    score += float.Parse(process.TotalScore);
                }
            }

            if (count > 0)
            {
                avegareScore = score / count;
                avegareScore = (float)Math.Round(avegareScore, 2);
            }

            return avegareScore;
        }
    }
}