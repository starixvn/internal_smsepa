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
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.TargetType;
    using SMS.EPA.Repositories.Interfaces;

    public class EmployeeEPARepository : IEmployeeEPARepository
    {
        public IEnumerable<EPAModel> GetEPAByEmployee(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<EPAModel> epas = multi.Read<EPAModel, Employee, TargetTypeModel, DepartmentViewModel, Group, EPAModel>(
                    (me, emp, target, dept, group) =>
                    {
                        me.Employee = emp;
                        me.TargetType = target;
                        me.Department = dept;
                        me.Group = group;

                        return me;
                    },
                    "EmployeeId, TargetTypeId, DepartmentId, GroupId");

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel, StatusModel, Employee, EPAProcessModel>(
                        (pc, status, eva) =>
                        {
                            pc.Status = status;
                            pc.Evaluator = eva;

                            return pc;
                        },
                        "StatusId, EmployeeId");

                    IEnumerable<EvaluatorModel> evaluators = multi.Read<EvaluatorModel>();

                    epas = GroupEPAProcessByEPA(epas, processes);
                    epas = GroupEvaluatorByEPA(epas, evaluators);

                    return epas;
                }
            }
        }

        public EPAModel GetEPAInfoByEmployee(ICriteria criteria)
        {
            EPAModel epa = new EPAModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epa = multi.Read<EPAModel, Employee, TargetTypeModel, DepartmentViewModel, Group, JobTitle, EPAModel>(
                    (me, emp, target, dept, group, job) =>
                    {
                        me.Employee = emp;
                        me.TargetType = target;
                        me.Department = dept;
                        me.Group = group;
                        me.JobTitle = job;

                        return me;
                    },
                    "EmployeeId, TargetTypeId, DepartmentId, GroupId, JobTitleId").FirstOrDefault();

                    epa.ProcessBySelf = multi.Read<EPAProcessModel, StatusModel, Employee, EPAProcessModel>(
                        (pc, status, eva) =>
                        {
                            pc.Status = status;
                            pc.Evaluator = eva;

                            return pc;
                        },
                        "StatusId, EmployeeId").FirstOrDefault();

                    epa.EPAInfoBySelf = multi.Read<EPAInfoModel, Employee, KPIGroupViewModel, EPAInfoModel>(
                        (info, eva, gr) =>
                        {
                            info.Evaluator = eva;
                            info.KPIGroup = gr;

                            return info;
                        },
                        "EmployeeId, KPIGroupId").ToList();

                    IList<KPIViewModel> kpis = multi.Read<KPIViewModel>().ToList();

                    epa.Evaluators = multi.Read<EvaluatorModel>().ToList();

                    GroupKPIByKPIGroup(epa.EPAInfoBySelf, kpis);
                }
            }

            return epa;
        }

        public IEnumerable<EPAModel> GroupEPAProcessByEPA(IEnumerable<EPAModel> epas, IEnumerable<EPAProcessModel> processes)
        {
            foreach (var epa in epas)
            {
                foreach (var process in processes)
                {
                    if (process.EPAId == epa.EPAId)
                    {
                        epa.ProcessBySelf = process;
                    }
                }
            }

            return epas;
        }

        public IEnumerable<EPAModel> GroupEvaluatorByEPA(IEnumerable<EPAModel> epas, IEnumerable<EvaluatorModel> evaluators)
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

            return epas;
        }

        public void GroupKPIByKPIGroup(IList<EPAInfoModel> epaInfos, IList<KPIViewModel> kpis)
        {
            foreach (var epaInfo in epaInfos)
            {
                foreach (var kpi in kpis)
                {
                    if (kpi.EPAId == epaInfo.EPAId
                        && kpi.ProcessId == epaInfo.ProcessId
                        && kpi.KPIGroupId == epaInfo.KPIGroup.KPIGroupId
                        && kpi.EvaluatorId == epaInfo.Evaluator.EmployeeId)
                    {
                        epaInfo.KPIGroup.KPIs.Add(kpi);
                    }
                }
            }
        }
    }
}