// -----------------------------------------------------------------------
// <copyright file="GetDashboardCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.04">Created</change>
// </history>

namespace SMS.EPA.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetDashboardCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "Epa_Dashboard";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}