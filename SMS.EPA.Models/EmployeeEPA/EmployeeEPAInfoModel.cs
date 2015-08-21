// -----------------------------------------------------------------------
// <copyright file="EmployeeEPAInfoModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.11">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.Model.Enitites;

    public class EmployeeEPAInfoModel
    {
        public EmployeeEPAInfoModel()
        {
            Evaluators = new List<Employee>();
            KPIGroups = new List<KPIGroupViewModel>();
        }

        public int EPAId { get; set; }

        public int PeriodId { get; set; }

        public int ProcessId { get; set; }

        public bool IsEndProcess { get; set; }

        public Employee Employee { get; set; }

        public IList<Employee> Evaluators { get; set; }

        public Employee Modifier { get; set; }

        public string CommentBySelf { get; set; }

        public string CommentByEvaluator { get; set; }

        public StatusModel Status { get; set; }

        public IList<KPIGroupViewModel> KPIGroups { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string TotalScoreBySelf { get; set; }

        public string TotalScoreByEvaluator { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}