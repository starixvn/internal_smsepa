// -----------------------------------------------------------------------
// <copyright file="UpdateKPIScoreCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.10">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA.RatingByKPI
{
    using Fanex.Data.Repository;

    public class UpdateKPIScoreCriteria : CriteriaBase
    {
        public int EvaluatorId { get; set; }

        public string KPIDetails { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_RatingByKPI_UpdateKPIScore";
        }

        public override bool IsValid()
        {
            return EvaluatorId > 0;
        }
    }
}