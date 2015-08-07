// -----------------------------------------------------------------------
// <copyright file="DeleteFlowTemplateCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.07">Created</change>
// </history>

namespace SMS.EPA.Criteria.FlowManagement
{
    using Fanex.Data.Repository;

    public class DeleteFlowTemplateCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "FlowTemplate_Delete";
        }

        public override bool IsValid()
        {
            return FlowTemplateId > 0;
        }
    }
}