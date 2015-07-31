// -----------------------------------------------------------------------
// <copyright file="GetMenuCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.31">Created</change>
// </history>

namespace SMS.EPA.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetMenuCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "Menu_Get";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}