// -----------------------------------------------------------------------
// <copyright file="GetStandardGradeCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.10.02">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetStandardGradeCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EPA_GetStandardGrade";
        }
    }
}