// -----------------------------------------------------------------------
// <copyright file="EPASummaryModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.25">Created</change>
// </history>

namespace SMS.EPA.Model.Report.EPASummary
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Period;

    public class EPASummaryModel
    {
        public IEnumerable<PeriodModel> PeriodSelecteds { get; set; }

        public IEnumerable<EPASummaryEmployeeModel> EPAEmployees { get; set; }
    }
}