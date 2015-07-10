// -----------------------------------------------------------------------
// <copyright file="GetKPIGroupByIdCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.08">Created</change>
// </history>

namespace SMS.EPA.Criteria.KPIGroup
{
    using Fanex.Data.Repository;

    public class GetKPIGroupByIdCriteria : CriteriaBase
    {
        public int KPIGroupId { get; set; }

        public override bool IsValid()
        {
            return KPIGroupId > 0;
        }

        public override string GetSettingKey()
        {
            return "KPIGroup_GetById";
        }
    }
}