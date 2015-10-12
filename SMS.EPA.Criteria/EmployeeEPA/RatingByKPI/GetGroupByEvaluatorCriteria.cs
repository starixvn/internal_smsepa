// -----------------------------------------------------------------------
// <copyright file="GetGroupByEvaluatorCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.10.12">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA.RatingByKPI
{
    using Fanex.Data.Repository;

    public class GetGroupByEvaluatorCriteria : CriteriaBase
    {
        public int EvaluatorId { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_RatingByKPI_GetGroup";
        }

        public override bool IsValid()
        {
            return EvaluatorId > 0;
        }
    }
}