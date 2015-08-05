// -----------------------------------------------------------------------
// <copyright file="CreateFlowTemplateCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.04">Created</change>
// </history>

namespace SMS.EPA.Criteria.FlowManagement
{
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Attribute;

    public class CreateFlowTemplateCriteria : CriteriaBase
    {
        [Output]
        public int FlowTemplateId { get; set; }

        public string Name { get; set; }

        public int TargetTypeId { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "FlowTemplate_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) && TargetTypeId > 0;
        }
    }
}