// ----------------------------------------------------------------------
// <copyright file="PeriodFlowModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/13/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.Period
{
    using SMS.EPA.Model.FlowManagement;

    public class PeriodFlowModel : FlowModel
    {
        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }
}
