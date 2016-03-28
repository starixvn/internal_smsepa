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
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Model.Report.EPADetail;
    using SMS.EPA.Model.Report.EPASummary;
    using SMS.EPA.Repositories.Interfaces;

    public class ReportRepository : IReportRepository
    {
        public EPASummaryModel GetEPASummary(ICriteria criteria)
        {
            EPASummaryModel epaSummary = new EPASummaryModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaSummary.PeriodSelecteds = multi.Read<PeriodModel>().ToList();

                    epaSummary.EPAInfos = multi.Read<EPASummaryEmployeeModel, Group, JobTitle, EPASummaryEmployeeModel>(
                        (res, gr, job) =>
                        {
                            res.Group = gr;
                            res.JobTitle = job;

                            return res;
                        },
                        "GroupId, JobTitleId").ToList();

                    IList<EPASummaryPeriodModel> periods = multi.Read<EPASummaryPeriodModel, StatusModel, EPASummaryPeriodModel>(
                        (res, status) =>
                        {
                            res.Status = status;

                            return res;
                        },
                        "StatusId").ToList();

                    var hashParam = db.GetOuputParams();
                    var totalItem = (int)hashParam["totalitem"];

                    epaSummary.TotalItem = totalItem;

                    GroupPeriodByEmployee(epaSummary.EPAInfos, periods);
                    DecodeFinalScoreAndGrade(epaSummary.EPAInfos);
                }
            }

            return epaSummary;
        }

        public EPADetailModel GetEPADetail(ICriteria criteria)
        {
            EPADetailModel epaDetail = new EPADetailModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaDetail.PhaseSelecteds = multi.Read<EvaluationPhaseModel>();

                    epaDetail.MemberEPAs = multi.Read<EPAModel, StatusModel, EPAPeriodModel, EPAModel>(
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

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel>();

                    var hashParam = db.GetOuputParams();
                    var totalItem = (int)hashParam["totalitem"];

                    epaDetail.TotalItem = totalItem;

                    GroupEmployeeByEPA(epaDetail.MemberEPAs, employees);
                    GroupEPAProcessByEPA(epaDetail.MemberEPAs, processes);
                    DecodeAllScore(epaDetail.MemberEPAs);
                }
            }

            return epaDetail;
        }

        private void GroupPeriodByEmployee(IEnumerable<EPASummaryEmployeeModel> epaSummaries, IList<EPASummaryPeriodModel> periods)
        {
            foreach (EPASummaryEmployeeModel epaSummary in epaSummaries)
            {
                foreach (EPASummaryPeriodModel period in periods)
                {
                    if (epaSummary.EmployeeId.Equals(period.EmployeeId))
                    {
                        epaSummary.Periods.Add(period);
                    }
                }
            }
        }

        private void GroupEmployeeByEPA(IEnumerable<EPAModel> epaInfos, IEnumerable<EPAEmployeeModel> employees)
        {
            foreach (EPAModel epaInfo in epaInfos)
            {
                foreach (EPAEmployeeModel employee in employees)
                {
                    if (employee.EPAId.Equals(epaInfo.EPAId))
                    {
                        epaInfo.Employee = employee;
                        break;
                    }
                }
            }
        }

        private void GroupEPAProcessByEPA(IEnumerable<EPAModel> epaInfos, IEnumerable<EPAProcessModel> processes)
        {
            foreach (EPAModel epaInfo in epaInfos)
            {
                foreach (EPAProcessModel process in processes)
                {
                    if (process.EPAId == epaInfo.EPAId)
                    {
                        switch (process.ProcessId)
                        {
                            case (int)ProcessType.Self:
                                epaInfo.ProcessBySelf = process;
                                break;

                            case (int)ProcessType.Evaluator:
                                epaInfo.ProcessByEvaluators.Add(process);
                                break;

                            case (int)ProcessType.Reviewer:
                                epaInfo.ProcessByReviewers.Add(process);
                                break;

                            case (int)ProcessType.Management:
                                epaInfo.ProcessByManagements.Add(process);
                                break;
                        }
                    }
                }
            }
        }

        private void DecodeFinalScoreAndGrade(IEnumerable<EPASummaryEmployeeModel> epaSummaries)
        {
            foreach (EPASummaryEmployeeModel epaSummary in epaSummaries)
            {
                foreach (EPASummaryPeriodModel period in epaSummary.Periods)
                {
                    period.FinalScore = EncodeService.Decode(period.FinalScoreEncode);
                    period.Grade = EncodeService.Decode(period.GradeEncode);
                }
            }
        }

        private void DecodeAllScore(IEnumerable<EPAModel> epaInfos)
        {
            foreach (EPAModel epaInfo in epaInfos)
            {
                epaInfo.FinalScore = EncodeService.Decode(epaInfo.FinalScoreEncode);
                epaInfo.Grade = EncodeService.Decode(epaInfo.GradeEncode);
                DecodeTotalScore(epaInfo.ProcessBySelf);
                DecodeTotalScore(epaInfo.ProcessByEvaluators);
                DecodeTotalScore(epaInfo.ProcessByReviewers);
                DecodeTotalScore(epaInfo.ProcessByManagements);
            }
        }

        private void DecodeTotalScore(EPAProcessModel process)
        {
            if (process != null)
            {
                process.TotalScore = EncodeService.Decode(process.TotalScoreEncode);
                process.AdjustScore = EncodeService.Decode(process.AdjustScoreEncode);
            }
        }

        private void DecodeTotalScore(IList<EPAProcessModel> processes)
        {
            foreach (EPAProcessModel process in processes)
            {
                DecodeTotalScore(process);
            }
        }
    }
}