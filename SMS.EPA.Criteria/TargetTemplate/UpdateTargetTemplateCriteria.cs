// ----------------------------------------------------------------------
// <copyright file="UpdateTargetTemplateCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/10/2015 11:19:54 AM">Created.</change>
// </history>

namespace SMS.EPA.Criteria.TargetTemplate
{
    using Fanex.Data.Repository;

    public class UpdateTargetTemplateCriteria : CriteriaBase
    {
        public int TargetTemplateId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int ModifiedBy { get; set; }

        public int? TargetTypeId { get; set; }

        public int ScoreRatingTypeId { get; set; }

        public string KPIs { get; set; }

        public override bool IsValid()
        {
            return TargetTemplateId > 0;
        }

        public override string GetSettingKey()
        {
            return "TargetTemplate_Update";
        }
    }
}
