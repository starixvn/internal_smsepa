// ----------------------------------------------------------------------
// <copyright file="GetTargetDepartmentSettingCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/7/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.TargetEmployeeSetting
{
    using Fanex.Data.Repository;

    public class GetTargetDepartmentSettingCriteria : CriteriaBase
    {
        public int? DepartmentId { get; set; }

        public bool? IsGroup { get; set; }

        public int? TargetTypeId { get; set; }

        public bool? ShowAll { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaTargetTemplateDepartment_Get";
        }
    }
}
