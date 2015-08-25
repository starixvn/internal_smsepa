// -----------------------------------------------------------------------
// <copyright file="MemberEPAModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.18">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.TargetType;

    public class MemberEPAModel
    {
        public MemberEPAModel()
        {
            Evaluators = new List<EvaluatorModel>();
        }

        public int EPAId { get; set; }

        public int PeriodId { get; set; }

        public bool IsEndProcess { get; set; }

        public Group Group { get; set; }

        public Employee Employee { get; set; }

        public TargetTypeModel TargetType { get; set; }

        public StatusModel Status { get; set; }

        public string SelfEvaluationScore { get; set; }

        public string EvaluationScore { get; set; }

        public IList<EvaluatorModel> Evaluators { get; set; }

        public string ReviewerScore { get; set; }

        public DateTime? EvaluateDate { get; set; }
    }
}