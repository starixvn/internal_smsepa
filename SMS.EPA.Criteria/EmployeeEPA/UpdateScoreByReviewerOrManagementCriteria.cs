// -----------------------------------------------------------------------
// <copyright file="UpdateScoreByReviewerOrManagementCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.13">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class UpdateScoreByReviewerOrManagementCriteria : CriteriaBase
    {
        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public int EmployeeId { get; set; }

        public int EvaluatorId { get; set; }

        public string Comment { get; set; }

        public string Score { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EPA_UpdateScoreByReviewerOrManagement";
        }
    }
}