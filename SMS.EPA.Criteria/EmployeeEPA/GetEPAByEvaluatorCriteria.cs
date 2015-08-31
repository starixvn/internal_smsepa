// -----------------------------------------------------------------------
// <copyright file="GetEPAByEvaluatorCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.18">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetEPAByEvaluatorCriteria : CriteriaBase
    {
        public int EvaluatorId { get; set; }

        public string EmployeeIds { get; set; }

        public int? GroupId { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_GetByEvaluator";
        }

        public override bool IsValid()
        {
            return EvaluatorId > 0;
        }
    }
}