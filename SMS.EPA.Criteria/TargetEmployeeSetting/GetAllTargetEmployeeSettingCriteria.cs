// ----------------------------------------------------------------------
// <copyright file="GetAllTargetEmployeeSettingCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/17/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.TargetEmployeeSetting
{
    using Fanex.Data.Repository;

    public class GetAllTargetEmployeeSettingCriteria : CriteriaBase
    {
        public int? GroupId { get; set; }

        public int? EmployeeId { get; set; }

        public int? TargetTypeId { get; set; }

        public bool? ShowAll { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaTargetTemplateEmployee_Get";
        }
    }
}