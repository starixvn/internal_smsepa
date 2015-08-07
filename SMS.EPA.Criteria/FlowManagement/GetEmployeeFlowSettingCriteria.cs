// -----------------------------------------------------------------------
// <copyright file="GetEmployeeFlowSettingCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.07">Created</change>
// </history>

namespace SMS.EPA.Criteria.FlowManagement
{
    using Fanex.Data.Repository;

    public class GetEmployeeFlowSettingCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }

        public int? DepartmentId { get; set; }

        public int? GroupId { get; set; }

        public override bool IsValid()
        {
            return FlowTemplateId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaEmployeeFlowSetting_Get";
        }
    }
}