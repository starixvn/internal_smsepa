// ----------------------------------------------------------------------
// <copyright file="UpdateStatusEndEvaluationPeriodCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/08/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateStatusEndEvaluationPeriodCriteria : CriteriaBase
    {
        public string EPAIds { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EPAIds);
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_UpdateStatusEndEvaluationPeriod";
        }
    }
}
