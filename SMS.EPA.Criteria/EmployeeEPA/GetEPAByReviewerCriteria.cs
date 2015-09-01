// -----------------------------------------------------------------------
// <copyright file="GetEPAByReviewerCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.24">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetEPAByReviewerCriteria : CriteriaBase
    {
        public int ReviewerId { get; set; }

        public string EmployeeIds { get; set; }

        public int? GroupId { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_GetByReviewer";
        }

        public override bool IsValid()
        {
            return ReviewerId > 0;
        }
    }
}