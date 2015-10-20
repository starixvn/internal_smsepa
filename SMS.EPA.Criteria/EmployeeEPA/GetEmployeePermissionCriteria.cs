// -----------------------------------------------------------------------
// <copyright file="GetEmployeePermissionCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.10.13">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetEmployeePermissionCriteria : CriteriaBase
    {
        public int EPAId { get; set; }

        public int ActorUserId { get; set; }

        public int? ProcessId { get; set; }

        public override bool IsValid()
        {
            return EPAId > 0 && ActorUserId > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_CheckPermission";
        }
    }
}