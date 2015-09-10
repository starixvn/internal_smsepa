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
                    result.Epas = multi.Read<EPAModel, StatusModel, PeriodModel, Employee, DepartmentModel, Group, JobTitle, EPAModel>(
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

                    result.Processes = multi.Read<EPAProcessModel, StatusModel, EPAProcessModel>(
                        (process, status) =>
                        {
                            process.Status = status;

                            return process;
                        },
                        "StatusId");

                    result.Evaluators = multi.Read<EvaluatorModel>();

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
                    epaInfo.EPA = multi.Read<EPAModel, StatusModel, PeriodModel, Employee, DepartmentModel, Group, JobTitle, EPAModel>(
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

                    epaInfo.Evaluators = multi.Read<EvaluatorModel>().ToList();
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
                    epas = multi.Read<RatingByKPIModel, Employee, Group, JobTitle, TemporaryKPIGroupModel, RatingByKPIModel>(
                        (epa, emp, gr, job, kgr) =>
                        {
                            epa.Employee = emp;
                            epa.Group = gr;
                            epa.JobTitle = job;
                            epa.KPIGroup = kgr;

                            return epa;
                        },
                        "EmployeeId, GroupId, JobTitleId, KPIGroupId");

                    IEnumerable<TemporaryKPIModel> kpis = multi.Read<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                        (kpi, kgr) =>
                        {
                            kpi.KPIGroup = kgr;

                            return kpi;
                        },
                        "KPIGroupId");

                    GroupKPIByEPA(epas, kpis);
                }
            }

            return epas;
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