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
        public EPASummaryModel()
        {
            PeriodSelecteds = new List<PeriodModel>();
            EPAEmployees = new List<EPASummaryEmployeeModel>();
        }

        public IList<PeriodModel> PeriodSelecteds { get; set; }

        public IList<EPASummaryEmployeeModel> EPAEmployees { get; set; }

        public int TotalItem { get; set; }
    }
}