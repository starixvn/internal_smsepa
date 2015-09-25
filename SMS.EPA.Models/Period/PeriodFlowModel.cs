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
    using System;
    using SMS.EPA.Model.FlowManagement;

    public class PeriodFlowModel : FlowModel
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool HasEvaluationGenerated { get; set; }

        public int ProcessStatus { get; set; }
    }
}
