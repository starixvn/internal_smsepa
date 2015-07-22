// -----------------------------------------------------------------------
// <copyright file="GetDepartmentCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.21">Created</change>
// </history>

namespace SMS.EPA.Criteria.Department
{
    using Fanex.Data.Repository;

    public class GetDepartmentCriteria : CriteriaBase
    {
        public int? ParentId { get; set; }

        public bool? IsActive { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Department_Get";
        }
    }
}