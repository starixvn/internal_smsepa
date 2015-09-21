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
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;

    public class EPAModel
    {
        public EPAModel()
        {
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

        public PeriodModel Period { get; set; }

        public StatusModel Status { get; set; }

        public string FinalScoreEncode { get; set; }

        public string FinalScore
        {
            get
            {
                return !string.IsNullOrEmpty(FinalScoreEncode) ? EncodeService.Decode(FinalScoreEncode) : string.Empty;
            }

            set
            {
                FinalScoreEncode = EncodeService.Encode(value);
            }
        }

        public bool HasAttendanceScore { get; set; }

        public int AttendanceScore
        {
            get
            {
                return 20;
            }
        }

        public int Rowspan
        {
            get
            {
                List<int> numbers = new List<int> { Evaluators.Count, Reviewers.Count, Managements.Count };

                return numbers.Max();
            }
        }

        public string AverageScoreByEvaluators
        {
            get
            {
                float score = 0;
                int count = 0;

                foreach (var processByEvaluator in ProcessByEvaluators)
                {
                    if (!string.IsNullOrEmpty(processByEvaluator.TotalScore))
                    {
                        count++;
                        score += float.Parse(processByEvaluator.TotalScore);
                    }
                }

                if (count > 0)
                {
                    return (score / count).ToString();
                }

                return string.Empty;
            }
        }

        public string AverageScoreByReviewers
        {
            get
            {
                float score = 0;
                int count = 0;

                foreach (var processByReviewer in ProcessByReviewers)
                {
                    if (!string.IsNullOrEmpty(processByReviewer.TotalScore))
                    {
                        count++;
                        score += float.Parse(processByReviewer.TotalScore);
                    }
                }

                if (count > 0)
                {
                    return (score / count).ToString();
                }

                return string.Empty;
            }
        }

        public string AverageScoreByManagements
        {
            get
            {
                float score = 0;
                int count = 0;

                foreach (var processByManagement in ProcessByManagements)
                {
                    if (!string.IsNullOrEmpty(processByManagement.TotalScore))
                    {
                        count++;
                        score += float.Parse(processByManagement.TotalScore);
                    }
                }

                if (count > 0)
                {
                    return (score / count).ToString();
                }

                return string.Empty;
            }
        }

        public EPAProcessModel ProcessBySelf { get; set; }

        public IList<EPAProcessModel> ProcessByEvaluators { get; set; }

        public IList<EPAProcessModel> ProcessByReviewers { get; set; }

        public IList<EPAProcessModel> ProcessByManagements { get; set; }

        public IList<EPAKPIGroupModel> KPIGroups { get; set; }

        public IList<EPAProcessModel> Evaluators { get; set; }

        public IList<EPAProcessModel> Reviewers { get; set; }

        public IList<EPAProcessModel> Managements { get; set; }
    }
}