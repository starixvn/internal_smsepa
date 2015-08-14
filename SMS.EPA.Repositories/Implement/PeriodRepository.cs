// ----------------------------------------------------------------------
// <copyright file="PeriodRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/13/2015">Created.</change>
// </history>

namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Model.TargetTemplate;
    using SMS.EPA.Model.TargetType;
    using SMS.EPA.Repositories.Interfaces;

    public class PeriodRepository : IPeriodRepository
    {
        public IEnumerable<PeriodModel> GetPeriod(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<PeriodModel, TargetTypeModel, PeriodModel>(
                    (pe, tar) =>
                    {
                        pe.TargetType = tar;
                        return pe;
                    },
                    "TargetTypeId",
                    criteria.GetSpParams());
            }
        }

        public PeriodModel GetPeriodById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<PeriodModel, TargetTypeModel, PeriodModel>(
                    (pe, tar) =>
                    {
                        pe.TargetType = tar;
                        return pe;
                    },
                    "TargetTypeId",
                    criteria.GetSpParams()).First();
            }
        }

        public RawPeriodEmployeeSettingModel GetPeriodEmployeeSettingById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<PeriodEmployeeSettingModel> employees = 
                        multi.Read<PeriodEmployeeSettingModel, Group, JobTitle, TargetTemplateModel, PeriodEmployeeSettingModel>(
                            (pe, gr, job, tar) =>
                            {
                                pe.Group = gr;
                                pe.JobTitle = job;
                                pe.TargetTemplate = tar;
                                return pe;
                            },
                            "GroupId, JobTitleId, TargetTemplateId");

                    IEnumerable<RawPeriodFlowSettingModel> flowSettings = 
                        multi.Read<RawPeriodFlowSettingModel, EmployeeModel, RawPeriodFlowSettingModel>(
                            (fl, eva) =>
                            {
                                fl.EvaluatorInfo = eva;
                                return fl;
                            },
                            "EvaluatorId");

                    var periodEmployeeSetting = new RawPeriodEmployeeSettingModel
                    {
                        Employees = employees,
                        FlowSettings = flowSettings
                    };

                    return periodEmployeeSetting;
                }
            }
        }
    }
}
