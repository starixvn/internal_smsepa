// <copyright file="GetEPAByManagementCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.28">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetEPAByManagementCriteria : CriteriaBase
    {
        public int ManagementId { get; set; }

        public string EmployeeIds { get; set; }

        public int? GroupId { get; set; }

        public bool? IsLocal { get; set; }

        public bool IsMonthly { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_GetByManagement";
        }

        public override bool IsValid()
        {
            return ManagementId > 0;
        }
    }
}