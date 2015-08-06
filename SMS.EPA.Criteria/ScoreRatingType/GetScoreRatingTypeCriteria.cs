// ----------------------------------------------------------------------
// <copyright file="GetScoreRatingTypeCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/6/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.ScoreRatingType
{
    using Fanex.Data.Repository;

    public class GetScoreRatingTypeCriteria : CriteriaBase
    {
        public int? ScoreRatingTypeId { get; set; }

        public bool IsActive { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaScoreRatingType_Get";
        }
    }
}
