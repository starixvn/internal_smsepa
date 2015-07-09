// -----------------------------------------------------------------------
// <copyright file="DeleteKPICriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.09">Created</change>
// </history>

namespace SMS.EPA.Criteria.KPI
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Delete KPI criteria.
    /// </summary>
    public class DeleteKPICriteria : CriteriaBase
    {
        public int KPIId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return KPIId > 0;
        }

        public override string GetSettingKey()
        {
            return "KPI_Delete";
        }
    }
}