// ----------------------------------------------------------------------
// <copyright file="DeleteTargetEmployeeSettingCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/7/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.TargetEmployeeSetting
{
    using Fanex.Data.Repository;

    public class DeleteTargetEmployeeSettingCriteria : CriteriaBase
    {
        public int TargetTemplateId { get; set; }

        public int EmployeeId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return TargetTemplateId > 0 && EmployeeId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaTargetTemplateEmployee_Delete";
        }
    }
}