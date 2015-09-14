// -----------------------------------------------------------------------
// <copyright file="GetEmployeeEPAByKPICriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.09">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA.RatingByKPI
{
    using Fanex.Data.Repository;

    public class GetEmployeeEPAByKPICriteria : CriteriaBase
    {
        public int EvaluatorId { get; set; }

        public int TargetTemplateId { get; set; }

        public int KPIId { get; set; }

        public int? GroupId { get; set; }

        public string EmployeeIds { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_RatingByKPI_Get";
        }

        public override bool IsValid()
        {
            return EvaluatorId > 0 && TargetTemplateId > 0 && KPIId > 0;
        }
    }
}