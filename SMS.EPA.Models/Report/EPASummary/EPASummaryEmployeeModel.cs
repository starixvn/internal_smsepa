// -----------------------------------------------------------------------
// <copyright file="EPASummaryEmployeeModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.25">Created</change>
// </history>

namespace SMS.EPA.Model.Report.EPASummary
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Enitites;

    public class EPASummaryEmployeeModel
    {
        public EPASummaryEmployeeModel()
        {
            Periods = new List<EPASummaryPeriodModel>();
        }

        public int EmployeeId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public Group Group { get; set; }

        public JobTitle JobTitle { get; set; }

        public IList<EPASummaryPeriodModel> Periods { get; set; }
    }
}