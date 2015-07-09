// -----------------------------------------------------------------------
// <copyright file="UpdateStatusKPICriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.08">Created</change>
// </history>

namespace SMS.EPA.Criteria.KPI
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Update Status KPI criteria.
    /// </summary>
    public class UpdateStatusKPICriteria : CriteriaBase
    {
        public int KPIId { get; set; }

        public bool IsActive { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "KPI_UpdateStatus";
        }

        public override bool IsValid()
        {
            return KPIId > 0;
        }
    }
}