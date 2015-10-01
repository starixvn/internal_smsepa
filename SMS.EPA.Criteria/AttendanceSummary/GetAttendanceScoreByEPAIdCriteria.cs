// ----------------------------------------------------------------------
// <copyright file="GetAttendanceScoreByEPAIdCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="10/01/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.AttendanceSummary
{
    using Fanex.Data.Repository;

    public class GetAttendanceScoreByEPAIdCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EPAIds")]
        public string EvaluationIds { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EvaluationIds);
        }

        public override string GetSettingKey()
        {
            return "EpaAttendanceSummary_GetByEPA";
        }
    }
}
