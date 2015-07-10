// ----------------------------------------------------------------------
// <copyright file="GetKPIOfTargetTemplateCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/10/2015 11:04:14 AM">Created.</change>
// </history>

namespace SMS.EPA.Criteria.TargetTemplate
{
    using Fanex.Data.Repository;

    public class GetKPIOfTargetTemplateCriteria : CriteriaBase
    {
        public int TargetTemplateId { get; set; }

        public override bool IsValid()
        {
            return TargetTemplateId > 0;
        }

        public override string GetSettingKey()
        {
            return "TargetTemplate_GetAllKPIByKPIGroups";
        }
    }
}
