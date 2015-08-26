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
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.TargetType;

    public class EPAModel
    {
        public EPAModel()
        {
            Evaluators = new List<EvaluatorModel>();
            EPAInfoBySelf = new List<EPAInfoModel>();
        }

        public int EPAId { get; set; }

        public Employee Employee { get; set; }

        public DepartmentViewModel Department { get; set; }

        public Group Group { get; set; }

        public JobTitle JobTitle { get; set; }

        public TargetTypeModel TargetType { get; set; }

        public bool Status { get; set; }

        public string FinalScore { get; set; }

        public int AttendanceScore
        {
            get
            {
                return 20;
            }
        }

        public EPAProcessModel ProcessBySelf { get; set; }

        public IList<EPAInfoModel> EPAInfoBySelf { get; set; }

        public IList<EvaluatorModel> Evaluators { get; set; }
    }
}