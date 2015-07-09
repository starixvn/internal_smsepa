// -----------------------------------------------------------------------
// <copyright file="UpdateRequiredEvaluateKPICriteria.cs" company="Nexcel Solutions Viet Nam">
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
    /// Update required evaluate criteria.
    /// </summary>
    public class UpdateRequiredEvaluateKPICriteria : CriteriaBase
    {
        public int KPIId { get; set; }

        public bool IsRequiredEvaluate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "KPI_UpdateRequiredEvaluate";
        }

        public override bool IsValid()
        {
            return KPIId > 0;
        }
    }
}