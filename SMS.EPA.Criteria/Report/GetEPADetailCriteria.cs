// -----------------------------------------------------------------------
// <copyright file="GetEPADetailCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.10.01">Created</change>
// </history>

namespace SMS.EPA.Criteria.Report
{
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Attribute;

    public class GetEPADetailCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int? PeriodId { get; set; }

        public int? DepartmentId { get; set; }

        public int? PageIndex { get; set; }

        public int? ItemsPerPage { get; set; }

        public bool IsViewAll { get; set; }

        [Output]
        public int? TotalItem { get; set; }

        public override string GetSettingKey()
        {
            return "Report_EPADetail";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}