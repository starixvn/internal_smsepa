// -----------------------------------------------------------------------
// <copyright file="GetTargetTypeCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.09">Created</change>
// </history>

namespace SMS.EPA.Criteria.TargetType
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get target type criteria.
    /// </summary>
    public class GetTargetTypeCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override string GetSettingKey()
        {
            return "TargetType_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}