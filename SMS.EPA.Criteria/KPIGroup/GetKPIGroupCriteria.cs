// -----------------------------------------------------------------------
// <copyright file="GetKPIGroupCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.08">Created</change>
// </history>

namespace SMS.EPA.Criteria.KPIGroup
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get KPI Group criteria.
    /// </summary>
    public class GetKPIGroupCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "KPIGroup_Get";
        }
    }
}