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
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA.RatingByKPI;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories.Interfaces;

    public class EmployeeEPARepository : IEmployeeEPARepository
    {
        public IEnumerable<EPAModel> GetEPA(ICriteria criteria)
        {
            IEnumerable<EPAModel> epas = new List<EPAModel>();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epas = multi.Read<EPAModel, StatusModel, EPAPeriodModel, EPAModel>(
                    (epa, status, period) =>
                    {
                        epa.Status = status;
                        epa.Period = period;

                        return epa;
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

                    IEnumerable<EPAProcessModel> evaluators = multi.Read<EPAProcessModel>();

                    GroupEmployeeByEPA(epas, employees);
                    GroupEPAProcessByEPA(epas, processes);
                    GroupEvaluatorByEPA(epas, evaluators);

                    return epas;
                }
            }
        }

        public EPAModel GetEPAInfo(ICriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaInfo = multi.Read<EPAModel, StatusModel, EPAPeriodModel, EPAModel>(
                    (info, status, period) =>
                    {
                        info.Status = status;
                        info.Period = period;

                        return info;
                    },
                    "StatusId, PeriodId").FirstOrDefault();

                    epaInfo.Employee = multi.Read<EPAEmployeeModel, DepartmentModel, Group, JobTitle, EPAEmployeeModel>(
                        (epa, dept, gr, job) =>
                        {
                            epa.Department = dept;
                            epa.Group = gr;
                            epa.JobTitle = job;

                            return epa;
                        },
                        "DepartmentId, GroupId, JobTitleId").FirstOrDefault();

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel, StatusModel, EPAProcessModel>(
                        (result, status) =>
                        {
                            result.Status = status;

                            return result;
                        },
                        "StatusId");

                    epaInfo.KPIGroups = multi.Read<EPAKPIGroupModel>().ToList();

                    IEnumerable<EPAKPIModel> kpis = multi.Read<EPAKPIModel>();

                    IEnumerable<EvaluationModel> evaluations = multi.Read<EvaluationModel>();

                    IList<EPAProcessModel> evaluators = multi.Read<EPAProcessModel>().ToList();

                    GroupEvaluatorByProcess(epaInfo, evaluators);
                    GroupEPAProcessByEPA(epaInfo, processes);
                    GroupKPIByKPIGroup(epaInfo.KPIGroups, kpis);

                    foreach (EPAKPIGroupModel kpiGroup in epaInfo.KPIGroups)
                    {
                        GroupEvaluationOnEachKPI(kpiGroup.KPIs, evaluations);
                    }
                }
            }

            return epaInfo;
        }

        #region Rating By KPI

        public IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                    (kpi, gr) =>
                    {
                        kpi.KPIGroup = gr;

                        return kpi;
                    },
                    "KPIGroupId",
                    criteria.GetSpParams());
            }
        }

        public IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(ICriteria criteria)
        {
            IEnumerable<RatingByKPIModel> epas = new List<RatingByKPIModel>();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epas = multi.Read<RatingByKPIModel, TemporaryKPIGroupModel, RatingByKPIModel>(
                        (epa, kgr) =>
                        {
                            epa.KPIGroup = kgr;

                            return epa;
                        },
                        "KPIGroupId");

                    IEnumerable<EPAEmployeeModel> employees = multi.Read<EPAEmployeeModel, DepartmentModel, Group, JobTitle, EPAEmployeeModel>(
                        (epa, dept, gr, job) =>
                        {
                            epa.Department = dept;
                            epa.Group = gr;
                            epa.JobTitle = job;

                            return epa;
                        },
                        "DepartmentId, GroupId, JobTitleId");

                    IEnumerable<TemporaryKPIModel> kpis = multi.Read<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                        (kpi, kgr) =>
                        {
                            kpi.KPIGroup = kgr;

                            return kpi;
                        },
                        "KPIGroupId");

                    GroupEmployeeByEPA(epas, employees);
                    GroupKPIByEPA(epas, kpis);
                }
            }

            return epas;
        }

        #endregion Rating By KPI

        private void GroupEmployeeByEPA(IEnumerable<EPAModel> epas, IEnumerable<EPAEmployeeModel> employees)
        {
            foreach (EPAModel epa in epas)
            {
                foreach (EPAEmployeeModel employee in employees)
                {
                    if (employee.EPAId.Equals(epa.EPAId))
                    {
                        epa.Employee = employee;
                    }
                }
            }
        }

        private void GroupEPAProcessByEPA(IEnumerable<EPAModel> epas, IEnumerable<EPAProcessModel> processes)
        {
            foreach (EPAModel epa in epas)
            {
                foreach (EPAProcessModel process in processes)
                {
                    if (process.EPAId.Equals(epa.EPAId))
                    {
                        switch (process.ProcessId)
                        {
                            case (int)ProcessType.Self:
                                epa.ProcessBySelf = process;
                                break;

                            case (int)ProcessType.Evaluator:
                                epa.ProcessByEvaluators.Add(process);
                                break;

                            case (int)ProcessType.Reviewer:
                                epa.ProcessByReviewers.Add(process);
                                break;

                            case (int)ProcessType.Management:
                                epa.ProcessByManagements.Add(process);
                                break;
                        }
                    }
                }
            }
        }

        private void GroupEPAProcessByEPA(EPAModel epa, IEnumerable<EPAProcessModel> processes)
        {
            foreach (EPAProcessModel process in processes)
            {
                switch (process.ProcessId)
                {
                    case (int)ProcessType.Self:
                        epa.ProcessBySelf = process;
                        break;

                    case (int)ProcessType.Evaluator:
                        epa.ProcessByEvaluators.Add(process);
                        break;

                    case (int)ProcessType.Reviewer:
                        epa.ProcessByReviewers.Add(process);
                        break;

                    case (int)ProcessType.Management:
                        epa.ProcessByManagements.Add(process);
                        break;
                }
            }
        }

        private void GroupEvaluatorByEPA(IEnumerable<EPAModel> epas, IEnumerable<EPAProcessModel> evaluators)
        {
            foreach (EPAModel epa in epas)
            {
                foreach (EPAProcessModel evaluator in evaluators)
                {
                    if (evaluator.EPAId.Equals(epa.EPAId))
                    {
                        GroupEvaluatorByProcess(epa, evaluator);
                    }
                }
            }
        }

        private void GroupEvaluatorByProcess(EPAModel epa, EPAProcessModel evaluator)
        {
            switch (evaluator.ProcessId)
            {
                case (int)ProcessType.HumanResource:
                    epa.HumanResources.Add(evaluator);
                    break;

                case (int)ProcessType.Evaluator:
                    epa.Evaluators.Add(evaluator);
                    break;

                case (int)ProcessType.Reviewer:
                    epa.Reviewers.Add(evaluator);
                    break;

                case (int)ProcessType.Management:
                    epa.Managements.Add(evaluator);
                    break;
            }
        }

        private void GroupEvaluatorByProcess(EPAModel epa, IEnumerable<EPAProcessModel> evaluators)
        {
            foreach (EPAProcessModel evaluator in evaluators)
            {
                if (evaluator.EPAId.Equals(epa.EPAId))
                {
                    GroupEvaluatorByProcess(epa, evaluator);
                }
            }
        }

        private void GroupKPIByKPIGroup(IList<EPAKPIGroupModel> kpiGroups, IEnumerable<EPAKPIModel> kpis)
        {
            foreach (EPAKPIGroupModel kpiGroup in kpiGroups)
            {
                foreach (EPAKPIModel kpi in kpis)
                {
                    if (kpi.EPAId.Equals(kpiGroup.EPAId) && kpi.KPIGroupId.Equals(kpiGroup.KPIGroupId))
                    {
                        kpiGroup.KPIs.Add(kpi);
                    }
                }
            }
        }

        private void GroupEvaluationOnEachKPI(IEnumerable<EPAKPIModel> kpis, IEnumerable<EvaluationModel> evaluations)
        {
            foreach (EPAKPIModel kpi in kpis)
            {
                foreach (EvaluationModel evaluation in evaluations)
                {
                    if (evaluation.EPAId.Equals(kpi.EPAId) && evaluation.KPIId.Equals(kpi.KPIId))
                    {
                        switch (evaluation.ProcessId)
                        {
                            case (int)ProcessType.Self:
                                kpi.EvaluationBySelf = evaluation;
                                break;

                            case (int)ProcessType.Evaluator:
                                kpi.EvaluationByEvaluators.Add(evaluation);
                                break;
                        }
                    }
                }
            }
        }

        private void GroupEmployeeByEPA(IEnumerable<RatingByKPIModel> epas, IEnumerable<EPAEmployeeModel> employees)
        {
            foreach (RatingByKPIModel epa in epas)
            {
                foreach (EPAEmployeeModel employee in employees)
                {
                    if (employee.EPAId.Equals(epa.EPAId))
                    {
                        epa.Employee = employee;
                    }
                }
            }
        }

        private void GroupKPIByEPA(IEnumerable<RatingByKPIModel> epas, IEnumerable<TemporaryKPIModel> kpis)
        {
            foreach (RatingByKPIModel epa in epas)
            {
                foreach (TemporaryKPIModel kpi in kpis)
                {
                    if (kpi.EPAId.Equals(epa.EPAId) && kpi.KPIGroup.KPIGroupId.Equals(epa.KPIGroup.KPIGroupId))
                    {
                        epa.KPIGroup.KPIs.Add(kpi);
                    }
                }
            }
        }
    }
}