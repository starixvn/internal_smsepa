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
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Model.TargetType;

    public class EPAModel
    {
        public EPAModel()
        {
            Evaluators = new List<EvaluatorModel>();
            ProcessByEvaluators = new List<EPAProcessModel>();
            ProcessByReviewers = new List<EPAProcessModel>();
            ProcessByManagements = new List<EPAProcessModel>();
            KPIGroups = new List<KPIGroupViewModel>();
        }

        public int EPAId { get; set; }

        public Employee Employee { get; set; }

        public Group Group { get; set; }

        public DepartmentModel Department { get; set; }

        public JobTitle JobTitle { get; set; }

        public PeriodModel Period { get; set; }

        public StatusModel Status { get; set; }

        public string FinalScore { get; set; }

        public int AttendanceScore
        {
            get
            {
                return 20;
            }
        }

        public int NumberEvaluators
        {
            get
            {
                int numberEvaluators = Evaluators.Count;
                return numberEvaluators != 0 ? numberEvaluators : 1;
            }
        }

        public string AverageScoreByEvaluators { get; set; }

        public string AverageScoreByReviewers { get; set; }

        public string AverageScoreByManagements { get; set; }

        public EPAProcessModel ProcessBySelf { get; set; }

        public IList<EPAProcessModel> ProcessByEvaluators { get; set; }

        public IList<EPAProcessModel> ProcessByReviewers { get; set; }

        public IList<EPAProcessModel> ProcessByManagements { get; set; }

        public IList<KPIGroupViewModel> KPIGroups { get; set; }

        public IList<EvaluatorModel> Evaluators { get; set; }

        public IList<EvaluatorModel> Reviewers { get; set; }

        public IList<EvaluatorModel> Managements { get; set; }
    }
}