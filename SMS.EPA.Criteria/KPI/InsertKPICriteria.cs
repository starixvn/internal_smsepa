// -----------------------------------------------------------------------
// <copyright file="InsertKPICriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.08">Created</change>
// </history>

namespace SMS.EPA.Criteria.KPI
{
    using System;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Attribute;

    public class InsertKPICriteria : CriteriaBase
    {
        [Output]
        public int KPIId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string KPIGroupIds { get; set; }

        public bool HasComment { get; set; }

        public bool IsRequiredComment { get; set; }

        public bool IsRequiredEvaluate { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "KPI_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(KPIGroupIds);
        }
    }
}