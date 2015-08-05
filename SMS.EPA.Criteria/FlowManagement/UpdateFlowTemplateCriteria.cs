// -----------------------------------------------------------------------
// <copyright file="UpdateFlowTemplateCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.05">Created</change>
// </history>

namespace SMS.EPA.Criteria.FlowManagement
{
    using Fanex.Data.Repository;

    public class UpdateFlowTemplateCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }

        public string Name { get; set; }

        public int TargetTypeId { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "FlowTemplate_Update";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) && FlowTemplateId > 0 && TargetTypeId > 0;
        }
    }
}