// -----------------------------------------------------------------------
// <copyright file="EmployeeEPARepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.04">Created</change>
// </history>

namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Repositories.Interfaces;

    public class EmployeeEPARepository : IEmployeeEPARepository
    {
        public IEnumerable<EPAModel> GetEPA(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<EPAModel> epas = multi.Read<EPAModel, StatusModel, PeriodModel, Employee, DepartmentModel, Group, JobTitle, EPAModel>(
                    (epa, status, period, emp, dept, gr, job) =>
                    {
                        epa.Status = status;
                        epa.Period = period;
                        epa.Employee = emp;
                        epa.Department = dept;
                        epa.Group = gr;
                        epa.JobTitle = job;

                        return epa;
                    },
                    "StatusId, PeriodId, EmployeeId, DepartmentId, GroupId, JobTitleId");

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel, StatusModel, EPAProcessModel>(
                        (result, status) =>
                        {
                            result.Status = status;

                            return result;
                        },
                        "StatusId");

                    IEnumerable<EvaluatorModel> evaluators = multi.Read<EvaluatorModel>();

                    GroupEPAProcessByEPA(epas, processes);
                    GroupEvaluatorByEPA(epas, evaluators);

                    return epas;
                }
            }
        }

        public EPAModel GetEPAInfo(ICriteria criteria)
        {
            EPAModel epa = new EPAModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epa = multi.Read<EPAModel, StatusModel, PeriodModel, Employee, DepartmentModel, Group, JobTitle, EPAModel>(
                    (info, status, period, emp, dept, gr, job) =>
                    {
                        info.Status = status;
                        info.Period = period;
                        info.Employee = emp;
                        info.Department = dept;
                        info.Group = gr;
                        info.JobTitle = job;

                        return info;
                    },
                    "StatusId, PeriodId, EmployeeId, DepartmentId, GroupId, JobTitleId").FirstOrDefault();

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel, StatusModel, EPAProcessModel>(
                        (result, status) =>
                        {
                            result.Status = status;

                            return result;
                        },
                        "StatusId");

                    epa.KPIGroups = multi.Read<KPIGroupViewModel>().ToList();

                    IEnumerable<KPIViewModel> kpis = multi.Read<KPIViewModel>();

                    IEnumerable<EvaluateModel> evaluates = multi.Read<EvaluateModel>();

                    epa.Evaluators = multi.Read<EvaluatorModel>().ToList();

                    GroupEPAProcessByEPA(epa, processes);
                    GroupKPIByKPIGroup(epa.KPIGroups, kpis);

                    foreach (var kpiGroup in epa.KPIGroups)
                    {
                        GroupEvaluateOnEachKPI(kpiGroup.KPIs, evaluates);
                    }
                }
            }

            return epa;
        }

        private void GroupEPAProcessByEPA(IEnumerable<EPAModel> epas, IEnumerable<EPAProcessModel> processes)
        {
            foreach (var epa in epas)
            {
                foreach (var process in processes)
                {
                    if (process.EPAId == epa.EPAId
                        && process.ProcessId == 2)
                    {
                        epa.ProcessBySelf = process;
                    }
                    else if (process.EPAId == epa.EPAId
                        && process.ProcessId == 3)
                    {
                        epa.ProcessByEvaluators.Add(process);
                    }
                    else if (process.EPAId == epa.EPAId
                        && process.ProcessId == 4)
                    {
                        epa.ProcessByReviewers.Add(process);
                    }
                    else if (process.EPAId == epa.EPAId
                        && process.ProcessId == 5)
                    {
                        epa.ProcessByManagements.Add(process);
                    }
                }
            }
        }

        private void GroupEPAProcessByEPA(EPAModel epa, IEnumerable<EPAProcessModel> processes)
        {
            foreach (var process in processes)
            {
                if (process.EPAId == epa.EPAId
                    && process.ProcessId == 2)
                {
                    epa.ProcessBySelf = process;
                }
                else if (process.EPAId == epa.EPAId
                    && process.ProcessId == 3)
                {
                    epa.ProcessByEvaluators.Add(process);
                }
                else if (process.EPAId == epa.EPAId
                    && process.ProcessId == 4)
                {
                    epa.ProcessByReviewers.Add(process);
                }
                else if (process.EPAId == epa.EPAId
                        && process.ProcessId == 5)
                {
                    epa.ProcessByManagements.Add(process);
                }
            }
        }

        private void GroupEvaluatorByEPA(IEnumerable<EPAModel> epas, IEnumerable<EvaluatorModel> evaluators)
        {
            foreach (var epa in epas)
            {
                foreach (var evaluator in evaluators)
                {
                    if (evaluator.EPAId == epa.EPAId)
                    {
                        epa.Evaluators.Add(evaluator);
                    }
                }
            }
        }

        private void GroupKPIByKPIGroup(IList<KPIGroupViewModel> kpiGroups, IEnumerable<KPIViewModel> kpis)
        {
            foreach (var kpiGroup in kpiGroups)
            {
                foreach (var kpi in kpis)
                {
                    if (kpi.EPAId == kpiGroup.EPAId
                        && kpi.KPIGroupId == kpiGroup.KPIGroupId)
                    {
                        kpiGroup.KPIs.Add(kpi);
                    }
                }
            }
        }

        private void GroupEvaluateOnEachKPI(IEnumerable<KPIViewModel> kpis, IEnumerable<EvaluateModel> evaluates)
        {
            foreach (var kpi in kpis)
            {
                foreach (var evaluate in evaluates)
                {
                    if (evaluate.ProcessId == 2
                        && evaluate.EPAId == kpi.EPAId
                        && evaluate.KPIId == kpi.KPIId)
                    {
                        kpi.EvaluateBySelf = evaluate;
                    }
                    else if (evaluate.ProcessId == 3
                        && evaluate.EPAId == kpi.EPAId
                        && evaluate.KPIId == kpi.KPIId)
                    {
                        kpi.EvaluateByEvaluators.Add(evaluate);
                    }
                }
            }
        }
    }
}