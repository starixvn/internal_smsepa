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
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Model.Report.Dashboard;
    using SMS.EPA.Model.Report.EPADetail;
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
                    epaSummary.PeriodSelecteds = multi.Read<PeriodModel>().ToList();

                    epaSummary.EPAEmployees = multi.Read<EPASummaryEmployeeModel, Group, JobTitle, EPASummaryEmployeeModel>(
                        (res, gr, job) =>
                        {
                            res.Group = gr;
                            res.JobTitle = job;

                            return res;
                        },
                        "GroupId, JobTitleId").ToList();

                    IEnumerable<EPASummaryPeriodModel> periods = multi.Read<EPASummaryPeriodModel>();

                    var hashParam = db.GetOuputParams();
                    var totalItem = (int)hashParam["totalitem"];

                    epaSummary.TotalItem = totalItem;

                    GroupPeriodByEmployee(epaSummary.EPAEmployees, periods);
                }
            }

            return epaSummary;
        }

        public EPADetailModel GetEPADetail(ICriteria criteria)
        {
            EPADetailModel epaDetail = new EPADetailModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaDetail.MemberEPAs = multi.Read<EPAModel, StatusModel, PeriodModel, EPAModel>(
                    (info, status, period) =>
                    {
                        info.Status = status;
                        info.Period = period;

                        return info;
                    },
                    "StatusId, PeriodId");

                    IEnumerable<EPAEmployeeModel> employees = multi.Read<EPAEmployeeModel, DepartmentModel, Group, JobTitle, EPAEmployeeModel>(
                        (epa, dept, gr, job) =>
                        {
                            epa.Department = dept;
                            epa.Group = gr;
                            epa.JobTitle = job;

                            return epa;
                        },
                        "DepartmentId, GroupId, JobTitleId");

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel, StatusModel, EPAProcessModel>(
                        (process, status) =>
                        {
                            process.Status = status;

                            return process;
                        },
                        "StatusId");

                    var hashParam = db.GetOuputParams();
                    var totalItem = (int)hashParam["totalitem"];

                    epaDetail.TotalItem = totalItem;

                    GroupEmployeeByEPA(epaDetail.MemberEPAs, employees);
                    GroupEPAProcessByEPA(epaDetail.MemberEPAs, processes);
                }
            }

            return epaDetail;
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

        private void GroupEmployeeByEPA(IEnumerable<EPAModel> epas, IEnumerable<EPAEmployeeModel> employees)
        {
            foreach (var epa in epas)
            {
                foreach (var employee in employees)
                {
                    if (employee.EPAId == epa.EPAId)
                    {
                        epa.Employee = employee;
                    }
                }
            }
        }

        private void GroupEPAProcessByEPA(IEnumerable<EPAModel> epas, IEnumerable<EPAProcessModel> processes)
        {
            foreach (var epa in epas)
            {
                foreach (var process in processes)
                {
                    if (process.EPAId == epa.EPAId
                        && process.ProcessId == (int)ProcessType.Self)
                    {
                        epa.ProcessBySelf = process;
                    }
                    else if (process.EPAId == epa.EPAId
                        && process.ProcessId == (int)ProcessType.Evaluator)
                    {
                        epa.ProcessByEvaluators.Add(process);
                    }
                    else if (process.EPAId == epa.EPAId
                        && process.ProcessId == (int)ProcessType.Reviewer)
                    {
                        epa.ProcessByReviewers.Add(process);
                    }
                    else if (process.EPAId == epa.EPAId
                        && process.ProcessId == (int)ProcessType.Management)
                    {
                        epa.ProcessByManagements.Add(process);
                    }
                }
            }
        }
    }
}