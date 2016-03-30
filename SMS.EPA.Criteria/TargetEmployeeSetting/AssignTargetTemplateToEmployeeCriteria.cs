// ----------------------------------------------------------------------
// <copyright file="AssignTargetTemplateToEmployeeCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/17/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.TargetEmployeeSetting
{
    using Fanex.Data.Repository;

    public class AssignTargetTemplateToEmployeeCriteria : CriteriaBase
    {
        public int TargetTemplateId { get; set; }

        public string EmployeeIds { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaTargetTemplateEmployee_Assign";
        }
    }
}