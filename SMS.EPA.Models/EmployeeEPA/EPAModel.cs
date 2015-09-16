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
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;

    public class EPAModel
    {
        public EPAModel()
        {
            Evaluators = new List<EvaluatorModel>();
            Reviewers = new List<EvaluatorModel>();
            Managements = new List<EvaluatorModel>();
            ProcessByEvaluators = new List<EPAProcessModel>();
            ProcessByReviewers = new List<EPAProcessModel>();
            ProcessByManagements = new List<EPAProcessModel>();
            KPIGroups = new List<EPAKPIGroupModel>();
        }

        public int EPAId { get; set; }

        public EPAEmployeeModel Employee { get; set; }

        public PeriodModel Period { get; set; }

        public StatusModel Status { get; set; }

        public string FinalScore { get; set; }

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

        public string AverageScoreByEvaluators { get; set; }

        public string AverageScoreByReviewers { get; set; }

        public string AverageScoreByManagements { get; set; }

        public EPAProcessModel ProcessBySelf { get; set; }

        public IList<EPAProcessModel> ProcessByEvaluators { get; set; }

        public IList<EPAProcessModel> ProcessByReviewers { get; set; }

        public IList<EPAProcessModel> ProcessByManagements { get; set; }

        public IList<EPAKPIGroupModel> KPIGroups { get; set; }

        public IList<EvaluatorModel> Evaluators { get; set; }

        public IList<EvaluatorModel> Reviewers { get; set; }

        public IList<EvaluatorModel> Managements { get; set; }
    }
}