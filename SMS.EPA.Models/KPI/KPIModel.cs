// -----------------------------------------------------------------------
// <copyright file="KPIModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.07">Created</change>
// </history>

namespace SMS.EPA.Model.KPI
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.Model.KPIGroup;
    using SMS.EPA.Model.TargetType;

    public class KPIModel
    {
        public KPIModel()
        {
            KPIGroups = new List<KPIGroupModel>();
        }

        public int KPIId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public bool HasComment { get; set; }

        public bool IsRequiredComment { get; set; }

        public bool IsRequiredEvaluate { get; set; }

        public bool Selected { get; set; }

        public int Weight { get; set; }

        public IList<KPIGroupModel> KPIGroups { get; set; }

        public TargetTypeModel TargetType { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}