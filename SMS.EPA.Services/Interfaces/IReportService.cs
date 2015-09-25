// -----------------------------------------------------------------------
// <copyright file="IReportService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.04">Created</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model.Report.Dashboard;
    using SMS.EPA.Model.Report.EPASummary;

    public interface IReportService
    {
        DashboardModel GetDashboard(GetDashboardCriteria criteria);

        IEnumerable<EPASummaryModel> GetEPASummary(GetEPASummaryCriteria criteria);
    }
}