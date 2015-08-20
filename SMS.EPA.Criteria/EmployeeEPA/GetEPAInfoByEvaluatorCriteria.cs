// -----------------------------------------------------------------------
// <copyright file="GetEPAInfoByEvaluatorCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.20">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetEPAInfoByEvaluatorCriteria : CriteriaBase
    {
        public int EPAId { get; set; }

        public int PeriodId { get; set; }

        public int EvaluatorId { get; set; }

        public override bool IsValid()
        {
            return EvaluatorId > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_GetInfoByEvaluator";
        }
    }
}