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
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;

    public class EmployeeEPAInfoModel
    {
        public EmployeeEPAInfoModel()
        {
            Evaluators = new List<EmployeeModel>();
            KPIGroups = new List<KPIGroupViewModel>();
        }

        public int EPAId { get; set; }

        public int PeriodId { get; set; }

        public bool IsEndProcess { get; set; }

        public EmployeeModel Employee { get; set; }

        public IList<EmployeeModel> Evaluators { get; set; }

        public EmployeeModel Modifier { get; set; }

        public string Comment { get; set; }

        public StatusModel Status { get; set; }

        public IList<KPIGroupViewModel> KPIGroups { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string TotalScore { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}