// -----------------------------------------------------------------------
// <copyright file="DeleteKPIGroupCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.10">Created</change>
// </history>

namespace SMS.EPA.Criteria.KPIGroup
{
    using Fanex.Data.Repository;

    public class DeleteKPIGroupCriteria : CriteriaBase
    {
        public int KPIGroupId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "KPIGroup_Delete";
        }

        public override bool IsValid()
        {
            return KPIGroupId > 0;
        }
    }
}