// ----------------------------------------------------------------------
// <copyright file="CheckPeriodStartedSendMailAllCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/03/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Criteria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
