// ----------------------------------------------------------------------
// <copyright file="DeleteTargetDepartmentSettingCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/7/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.TargetEmployeeSetting
{
    using Fanex.Data.Repository;

    public class DeleteTargetDepartmentSettingCriteria : CriteriaBase
    {
        public int TargetTemplateId { get; set; }

        public int DepartmentId { get; set; }

        public bool IsGroup { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return TargetTemplateId > 0 && DepartmentId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaTargetTemplateDepartment_Delete";
        }
    }
}