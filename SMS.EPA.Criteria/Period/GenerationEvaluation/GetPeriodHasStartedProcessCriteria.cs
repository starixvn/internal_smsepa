// ----------------------------------------------------------------------
// <copyright file="GetPeriodHasStartedProcessCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/13/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.Period.GenerationEvaluation
{
    using Fanex.Data.Repository;

    public class GetPeriodHasStartedProcessCriteria : CriteriaBase
    {
        public int? PeriodId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_GetPeriodHasStartedProcess";
        }
    }
}
