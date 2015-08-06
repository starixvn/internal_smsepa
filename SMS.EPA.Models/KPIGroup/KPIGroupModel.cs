// -----------------------------------------------------------------------
// <copyright file="KPIGroupModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.07">Created.</change>
// </history>

namespace SMS.EPA.Model.KPIGroup
{
    using System;

    public class KPIGroupModel
    {
        public int KPIGroupId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int NumberKPI { get; set; }

        public bool IsActive { get; set; }

        public double? Weight { get; set; }

        public bool IsSpreadKPI { get; set; }

        public int DisplayOrder { get; set; }

        public bool? Selected { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}