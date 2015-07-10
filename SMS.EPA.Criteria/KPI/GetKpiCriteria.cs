// -----------------------------------------------------------------------
// <copyright file="GetKPICriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.07">Created</change>
// </history>

namespace SMS.EPA.Criteria.KPI
{
    using Fanex.Data.Repository;

    public class GetKPICriteria : CriteriaBase
    {
        public int? TargetTypeId { get; set; }

        public int? KPIGroupId { get; set; }

        public bool? IsActive { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "KPI_Get";
        }
    }
}