// -----------------------------------------------------------------------
// <copyright file="UpdateKPIGroupCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.09">Created</change>
// </history>

namespace SMS.EPA.Criteria.KPIGroup
{
    using Fanex.Data.Repository;

    public class UpdateKPIGroupCriteria : CriteriaBase
    {
        public int KPIGroupId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string Description { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "KPIGroup_Update";
        }

        public override bool IsValid()
        {
            return KPIGroupId > 0 && !string.IsNullOrWhiteSpace(Name);
        }
    }
}