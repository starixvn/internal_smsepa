// -----------------------------------------------------------------------
// <copyright file="IReportRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.04">Created</change>
// </history>

namespace SMS.EPA.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Report.Dashboard;
    using SMS.EPA.Model.Report.EPASummary;

    public interface IReportRepository
    {
        DashboardModel GetDashboard(ICriteria criteria);

        IEnumerable<EPASummaryModel> GetEPASummary(ICriteria criteria);
    }
}