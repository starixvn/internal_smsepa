// -----------------------------------------------------------------------
// <copyright file="ReportRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.04">Created</change>
// </history>

namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Model.Report.Dashboard;
    using SMS.EPA.Model.Report.EPASummary;
    using SMS.EPA.Repositories.Interfaces;

    public class ReportRepository : IReportRepository
    {
        public DashboardModel GetDashboard(ICriteria criteria)
        {
            DashboardModel result = new DashboardModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                result.MyEPAs = db.Query<EPADashboardModel, StatusModel, PeriodModel, EPADashboardModel>(
                    (epa, status, period) =>
                    {
                        epa.Status = status;
                        epa.Period = period;

                        return epa;
                    },
                    "StatusId, PeriodId",
                    criteria.GetSpParams());
            }

            return result;
        }

        public EPASummaryModel GetEPASummary(ICriteria criteria)
        {
            EPASummaryModel epaSummary = new EPASummaryModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaSummary.PeriodSelecteds = multi.Read<PeriodModel>();

                    epaSummary.EPAEmployees = multi.Read<EPASummaryEmployeeModel, Group, JobTitle, EPASummaryEmployeeModel>(
                        (res, gr, job) =>
                        {
                            res.Group = gr;
                            res.JobTitle = job;

                            return res;
                        },
                        "GroupId, JobTitleId");

                    IEnumerable<EPASummaryPeriodModel> periods = multi.Read<EPASummaryPeriodModel>();

                    GroupPeriodByEmployee(epaSummary.EPAEmployees, periods);
                }
            }

            return epaSummary;
        }

        private void GroupPeriodByEmployee(IEnumerable<EPASummaryEmployeeModel> epaSummaries, IEnumerable<EPASummaryPeriodModel> periods)
        {
            foreach (var epaSummary in epaSummaries)
            {
                foreach (var period in periods)
                {
                    if (epaSummary.EmployeeId == period.EmployeeId)
                    {
                        epaSummary.Periods.Add(period);
                    }
                }
            }
        }
    }
}