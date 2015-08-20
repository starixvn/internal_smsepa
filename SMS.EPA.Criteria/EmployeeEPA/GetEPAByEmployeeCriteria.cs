// -----------------------------------------------------------------------
// <copyright file="GetEPAByEmployeeCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.04">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetEPAByEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int? TargetTypeId { get; set; }

        public int? Status { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_GetByEmployee";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}