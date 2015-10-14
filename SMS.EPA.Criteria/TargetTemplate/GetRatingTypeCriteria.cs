// ----------------------------------------------------------------------
// <copyright file="GetRatingTypeCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="10/14/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.TargetTemplate
{
    using Fanex.Data.Repository;

    public class GetRatingTypeCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaRatingType_Get";
        }
    }
}
