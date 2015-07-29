// -----------------------------------------------------------------------
// <copyright file="CreateDepartmentCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.29">Created</change>
// </history>

namespace SMS.EPA.Criteria.Department
{
    using Fanex.Data.Repository;

    public class CreateDepartmentCriteria : CriteriaBase
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public int? ManagerId { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrEmpty(Name);
        }

        public override string GetSettingKey()
        {
            return "Department_Insert";
        }
    }
}