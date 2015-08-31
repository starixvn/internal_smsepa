// ----------------------------------------------------------------------
// <copyright file="GetEmployeeSettingByPeriodIdCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/31/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Criteria
{
    using Fanex.Data.Repository;

    public class GetEmployeeSettingByPeriodIdCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public int? ProcessId { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_GetEmployeeSettingToSendMail";
        }
    }
}
