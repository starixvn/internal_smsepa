// ----------------------------------------------------------------------
// <copyright file="GetEPAHistoryCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="10/19/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.EPAHistory
{
    using Fanex.Data.Repository;

    public class GetEPAHistoryCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public int EvaluatorId { get; set; }

        public int? EmployeeId { get; set; }

        public override bool IsValid()
        {
            return PeriodId != 0 && EvaluatorId > 0;
        }

        public override string GetSettingKey()
        {
            return "Epa_GetEPAHistory";
        }
    }
}
