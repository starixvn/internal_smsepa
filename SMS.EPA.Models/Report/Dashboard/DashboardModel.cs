// -----------------------------------------------------------------------
// <copyright file="DashboardModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.04">Created</change>
// </history>

namespace SMS.EPA.Model.Report.Dashboard
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.EmployeeEPA;

    public class DashboardModel
    {
        public EmployeeModel Employee { get; set; }

        public IEnumerable<EPAModel> MyEPAs { get; set; }

        public IEnumerable<EPAModel> MonthlyEPAs { get; set; }
    }
}