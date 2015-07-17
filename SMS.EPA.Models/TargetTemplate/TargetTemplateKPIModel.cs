// ----------------------------------------------------------------------
// <copyright file="TargetTemplateKPIModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/10/2015 11:29:52 AM">Created and clone from NexEPA.</change>
// </history>

namespace SMS.EPA.Model.TargetTemplate
{
    public class TargetTemplateKPIModel
    {
        public int KPIId { get; set; }

        public string Name { get; set; }

        public string KPIGroupName { get; set; }

        public int KPIGroupId { get; set; }

        public double? Weight { get; set; }

        public int? DisplayOrder { get; set; }
    }
}
