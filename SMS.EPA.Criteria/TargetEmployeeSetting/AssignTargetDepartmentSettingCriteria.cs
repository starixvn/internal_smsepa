// ----------------------------------------------------------------------
// <copyright file="AssignTargetDepartmentSettingCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/7/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.TargetEmployeeSetting
{
    using Fanex.Data.Repository;

    public class AssignTargetDepartmentSettingCriteria : CriteriaBase
    {
        public int TargetTemplateId { get; set; }

        public string Departments { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaTargetTemplateDepartment_Assign";
        }
    }
}