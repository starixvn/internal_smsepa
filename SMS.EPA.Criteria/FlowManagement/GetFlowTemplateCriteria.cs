// -----------------------------------------------------------------------
// <copyright file="GetFlowTemplateCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.04">Created</change>
// </history>

namespace SMS.EPA.Criteria.FlowManagement
{
    using Fanex.Data.Repository;

    public class GetFlowTemplateCriteria : CriteriaBase
    {
        public int? TargetTypeId { get; set; }

        public bool? IsActive { get; set; }

        public override string GetSettingKey()
        {
            return "FlowTemplate_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}