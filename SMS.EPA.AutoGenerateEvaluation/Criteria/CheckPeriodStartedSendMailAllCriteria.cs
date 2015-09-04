// ----------------------------------------------------------------------
// <copyright file="CheckPeriodStartedSendMailAllCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/28/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Criteria
{
    using Fanex.Data.Repository;

    public class CheckPeriodStartedSendMailAllCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_CheckSendMailAllWhenPeriodStarted";
        }
    }
}
