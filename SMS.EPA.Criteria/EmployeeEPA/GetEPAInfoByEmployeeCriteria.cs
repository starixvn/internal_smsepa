// -----------------------------------------------------------------------
// <copyright file="GetEPAInfoByEmployeeCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.11">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetEPAInfoByEmployeeCriteria : CriteriaBase
    {
        public int EPAId { get; set; }

        public int EmployeeId { get; set; }

        public override bool IsValid()
        {
            return EPAId > 0 && EmployeeId > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_GetInfoByEmployee";
        }
    }
}