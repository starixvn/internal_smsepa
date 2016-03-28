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
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Report.EPADetail;
    using SMS.EPA.Model.Report.EPASummary;

    public interface IReportRepository
    {
        EPASummaryModel GetEPASummary(ICriteria criteria);

        EPADetailModel GetEPADetail(ICriteria criteria);
    }
}