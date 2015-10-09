// -----------------------------------------------------------------------
// <copyright file="EPAModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.26">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.Enitites;

    public class EPAModel
    {
        public EPAModel()
        {
            HumanResources = new List<EPAProcessModel>();
            Evaluators = new List<EPAProcessModel>();
            Reviewers = new List<EPAProcessModel>();
            Managements = new List<EPAProcessModel>();
            ProcessByEvaluators = new List<EPAProcessModel>();
            ProcessByReviewers = new List<EPAProcessModel>();
            ProcessByManagements = new List<EPAProcessModel>();
            KPIGroups = new List<EPAKPIGroupModel>();
        }

        public int EPAId { get; set; }

        public EPAEmployeeModel Employee { get; set; }

        public EPAPeriodModel Period { get; set; }

        public StatusModel Status { get; set; }

        public string LastFinalScoreEncode { get; set; }

        public string LastFinalScore
        {
            get
            {
                return !string.IsNullOrEmpty(LastFinalScoreEncode) ? EncodeService.Decode(LastFinalScoreEncode) : string.Empty;
            }
        }

        public string FinalScoreEncode { get; set; }

        public string FinalScore { get; set; }

        public int MaxFinalScore
        {
            get
            {
                return Constants.MaxFinalScore;
            }
        }

        public string GradeEncode { get; set; }

        public string Grade { get; set; }

        public string LastGradeEncode { get; set; }

        public string LastGrade
        {
            get
            {
                return !string.IsNullOrEmpty(LastGradeEncode) ? EncodeService.Decode(LastGradeEncode) : string.Empty;
            }
        }

        public bool HasDisciplineScore { get; set; }

        public float? LastDisciplineScore { get; set; }

        public float DisciplineScore { get; set; }

        public float MaxDisciplineScore { get; set; }

        public int Rowspan
        {
            get
            {
                List<int> numbers = new List<int> { Evaluators.Count, Reviewers.Count, Managements.Count };

                return numbers.Max();
            }
        }

        public bool HasMaxScoreOnKPI { get; set; }

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

        public EPAProcessModel CurrentProcess { get; set; }

        public EPAProcessModel ProcessBySelf { get; set; }

        public IList<EPAProcessModel> ProcessByEvaluators { get; set; }

        public IList<EPAProcessModel> ProcessByReviewers { get; set; }

        public IList<EPAProcessModel> ProcessByManagements { get; set; }

        public IList<EPAKPIGroupModel> KPIGroups { get; set; }

        public IList<EPAProcessModel> HumanResources { get; set; }

        public IList<EPAProcessModel> Evaluators { get; set; }

        public IList<EPAProcessModel> Reviewers { get; set; }

        public IList<EPAProcessModel> Managements { get; set; }

        private float CalculateAverageScore(IList<EPAProcessModel> processes)
        {
            float score = 0;
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
                return score / count;
            }

            return score;
        }
    }
}