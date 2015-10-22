// -----------------------------------------------------------------------
// <copyright file="GetDepartmentSectionHasSettingCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2015.10.22">Created</change>
// </history>

namespace SMS.EPA.Criteria.FlowManagement
{
    using Fanex.Data.Repository;

    public class GetDepartmentSectionHasSettingCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }

        public override bool IsValid()
        {
            return FlowTemplateId > 0;
        }

        public override string GetSettingKey()
        {
            return "FlowTemplate_GetDepartmentHasSetting";
        }
    }
}
