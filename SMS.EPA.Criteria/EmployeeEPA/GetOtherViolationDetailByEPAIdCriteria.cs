// -----------------------------------------------------------------------
// <copyright file="GetOtherViolationDetailByEPAIdCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.11.13">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetOtherViolationDetailByEPAIdCriteria : CriteriaBase
    {
        public int EPAId { get; set; }

        public override bool IsValid()
        {
            return EPAId > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_GetOtherViolations";
        }
    }
}