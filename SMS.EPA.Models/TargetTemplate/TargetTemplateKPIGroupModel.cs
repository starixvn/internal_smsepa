// ----------------------------------------------------------------------
// <copyright file="TargetTemplateKPIGroupModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/13/2015 3:39:41 PM">Created.</change>
// </history>

namespace SMS.EPA.Model.TargetTemplate
{
    using System.Collections.Generic;
    using SMS.EPA.Model.KPI;
    using SMS.EPA.Model.KPIGroup;

    public class TargetTemplateKPIGroupModel : KPIGroupModel
    {
        public IEnumerable<KPIModel> KPIs { get; set; }
    }
}
