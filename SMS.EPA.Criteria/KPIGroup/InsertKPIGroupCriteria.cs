// -----------------------------------------------------------------------
// <copyright file="InsertKPIGroupCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.09">Created</change>
// </history>

namespace SMS.EPA.Criteria.KPIGroup
{
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Attribute;

    public class InsertKPIGroupCriteria : CriteriaBase
    {
        [Output]
        public int KPIGroupId { get; set; }

        public string Name { get; set; }

        public int TargetTypeId { get; set; }

        public bool IsActive { get; set; }

        public string Description { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "KPIGroup_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) && TargetTypeId > 0;
        }
    }
}