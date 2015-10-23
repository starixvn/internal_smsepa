// ----------------------------------------------------------------------
// <copyright file="CheckTotalScoreOfProcessIsNullCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/08/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Criteria
{
    using Fanex.Data.Repository;

    public class CheckTotalScoreOfProcessIsNullCriteria : CriteriaBase
    {
        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public override bool IsValid()
        {
            return EPAId > 0 && ProcessId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_CheckTotalScoreIsNull";
        }
    }
}
