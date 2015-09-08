// ----------------------------------------------------------------------
// <copyright file="UpdateIsEndProcessCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/08/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateIsEndProcessCriteria : CriteriaBase
    {
        public string EPAProcess { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EPAProcess);
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_UpdateIsEndProcess";
        }
    }
}
