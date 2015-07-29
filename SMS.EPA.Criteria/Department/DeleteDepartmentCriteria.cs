// -----------------------------------------------------------------------
// <copyright file="DeleteDepartmentCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.29">Created</change>
// </history>

namespace SMS.EPA.Criteria.Department
{
    using Fanex.Data.Repository;

    public class DeleteDepartmentCriteria : CriteriaBase
    {
        public int DepartmentId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return DepartmentId != 0;
        }

        public override string GetSettingKey()
        {
            return "Department_Delete";
        }
    }
}