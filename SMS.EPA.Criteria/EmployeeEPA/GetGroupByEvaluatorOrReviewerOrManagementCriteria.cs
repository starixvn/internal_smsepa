// -----------------------------------------------------------------------
// <copyright file="GetGroupByEvaluatorOrReviewerOrManagementCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.11.09">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetGroupByEvaluatorOrReviewerOrManagementCriteria : CriteriaBase
    {
        public int ActorUserId { get; set; }

        public int ProcessId { get; set; }

        public override bool IsValid()
        {
            return ActorUserId > 0 && ProcessId > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_GetGroup";
        }
    }
}