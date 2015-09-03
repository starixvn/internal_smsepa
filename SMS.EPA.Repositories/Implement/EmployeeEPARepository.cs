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

                    epaInfo.EPA.KPIGroups = multi.Read<EPAKpiGroupModel>().ToList();

                    epaInfo.KPIs = multi.Read<EPAKpiModel>();

                    epaInfo.Evaluations = multi.Read<EvaluationModel>();

                    epaInfo.Evaluators = multi.Read<EvaluatorModel>().ToList();
                }
            }

            return epaInfo;
        }
    }
}