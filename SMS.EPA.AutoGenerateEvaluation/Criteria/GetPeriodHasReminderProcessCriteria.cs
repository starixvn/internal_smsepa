// ----------------------------------------------------------------------
// <copyright file="GetPeriodHasReminderProcessCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/03/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Criteria
{
    using Fanex.Data.Repository;

    public class GetPeriodHasReminderProcessCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_GetPeriodHasReminderProcess";
        }
    }
}
