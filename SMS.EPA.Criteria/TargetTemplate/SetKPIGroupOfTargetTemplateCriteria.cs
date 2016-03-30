// ----------------------------------------------------------------------
// <copyright file="SetKPIGroupOfTargetTemplateCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/10/2015 11:17:34 AM">Created.</change>
// </history>

namespace SMS.EPA.Criteria.TargetTemplate
{
    using Fanex.Data.Repository;

    public class SetKPIGroupOfTargetTemplateCriteria : CriteriaBase
    {
        public int TargetTemplateId { get; set; }

        public string KPIGroups { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return TargetTemplateId > 0;
        }

        public override string GetSettingKey()
        {
            return "TargetTemplate_SetKPIGroup";
        }
    }
}