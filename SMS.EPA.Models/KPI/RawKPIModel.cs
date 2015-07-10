// -----------------------------------------------------------------------
// <copyright file="RawKPIModel.cs"company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.07">Created</change>
// </history>

namespace SMS.EPA.Model.KPI
{
    using SMS.EPA.Model.KPIGroup;

    public class RawKPIModel
    {
        public int KPIId { get; set; }

        public int KPIGroupId { get; set; }

        public string Name { get; set; }
    }
}