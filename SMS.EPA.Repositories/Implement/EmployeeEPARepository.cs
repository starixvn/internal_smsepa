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
    using SMS.EPA.Model.EmployeeEPA.RatingByKPI;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Repositories.Interfaces;

    public class EmployeeEPARepository : IEmployeeEPARepository
    {
        public RawEPAModel GetEPA(ICriteria criteria)
        {
            RawEPAModel result = new RawEPAModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    result.Epas = multi.Read<EPAModel, StatusModel, PeriodModel, EPAModel>(
                    (epa, status, period) =>
                    {
                        epa.Status = status;
                        epa.Period = period;

                        return epa;
                    },
                    "StatusId, PeriodId");

                    result.Employees = multi.Read<EPAEmployeeModel, DepartmentModel, Group, JobTitle, EPAEmployeeModel>(
                        (epa, dept, gr, job) =>
                        {
                            epa.Department = dept;
                            epa.Group = gr;
                            epa.JobTitle = job;

                            return epa;
                        },
                        "DepartmentId, GroupId, JobTitleId");

                    result.Processes = multi.Read<EPAProcessModel, StatusModel, EPAProcessModel>(
                        (process, status) =>
                        {
                            process.Status = status;

                            return process;
                        },
                        "StatusId");

                    result.Evaluators = multi.Read<EPAProcessModel>();

                    return result;
                }
            }
        }

        public RawEPAInfoModel GetEPAInfo(ICriteria criteria)
        {
            RawEPAInfoModel epaInfo = new RawEPAInfoModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaInfo.EPA = multi.Read<EPAModel, StatusModel, PeriodModel, EPAModel>(
                    (info, status, period) =>
                    {
                        info.Status = status;
                        info.Period = period;

                        return info;
                    },
                    "StatusId, PeriodId").FirstOrDefault();

                    epaInfo.EPA.Employee = multi.Read<EPAEmployeeModel, DepartmentModel, Group, JobTitle, EPAEmployeeModel>(
                        (epa, dept, gr, job) =>
                        {
                            epa.Department = dept;
                            epa.Group = gr;
                            epa.JobTitle = job;

                            return epa;
                        },
                        "DepartmentId, GroupId, JobTitleId").FirstOrDefault();

                    epaInfo.Processes = multi.Read<EPAProcessModel, StatusModel, EPAProcessModel>(
                        (result, status) =>
                        {
                            result.Status = status;

                            return result;
                        },
                        "StatusId");

                    epaInfo.EPA.KPIGroups = multi.Read<EPAKPIGroupModel>().ToList();

                    epaInfo.KPIs = multi.Read<EPAKPIModel>();

                    epaInfo.Evaluations = multi.Read<EvaluationModel>();

                    epaInfo.Evaluators = multi.Read<EPAProcessModel>().ToList();
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
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
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

        private void GroupEmployeeByEPA(IEnumerable<RatingByKPIModel> epas, IEnumerable<EPAEmployeeModel> employees)
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

        private void GroupKPIByEPA(IEnumerable<RatingByKPIModel> epas, IEnumerable<TemporaryKPIModel> kpis)
        {
            foreach (var epa in epas)
            {
                foreach (var kpi in kpis)
                {
                    if (kpi.EPAId == epa.EPAId && kpi.KPIGroup.KPIGroupId == epa.KPIGroup.KPIGroupId)
                    {
                        epa.KPIGroup.KPIs.Add(kpi);
                    }
                }
            }
        }

        #endregion Rating By KPI
    }
}