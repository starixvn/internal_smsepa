// -----------------------------------------------------------------------
// <copyright file="GetMemberByEvaluatorOrReviewerOrManagementCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.18">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetMemberByEvaluatorOrReviewerOrManagementCriteria : CriteriaBase
    {
        public int ProcessId { get; set; }

        public int ActorUserId { get; set; }

        public int? GroupId { get; set; }

        public bool? IsLocal { get; set; }

        public override bool IsValid()
        {
            return ActorUserId > 0 && ProcessId > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_GetMember";
        }
    }
}