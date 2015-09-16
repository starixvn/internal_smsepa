// -----------------------------------------------------------------------
// <copyright file="ReportService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.04">Created</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using Fanex.Common.Helper;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Core.Services.Interfaces;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model.Report.Dashboard;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IEncodeService _encodeService;

        public ReportService()
            : this(null, null)
        {
        }

        public ReportService(
            IReportRepository reportRepository,
            IEncodeService encodeService)
        {
            _reportRepository = reportRepository ?? new ReportRepository();
            _encodeService = encodeService ?? new EncodeService();
        }

        public DashboardModel GetDashboard(GetDashboardCriteria criteria)
        {
            DashboardModel result = new DashboardModel();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                result = _reportRepository.GetDashboard(criteria);

                foreach (var epa in result.MyEPAs)
                {
                    epa.FinalScore = !string.IsNullOrEmpty(epa.FinalScore) ? _encodeService.Decode(epa.FinalScore) : string.Empty;
                }
            }

            return result;
        }
    }
}