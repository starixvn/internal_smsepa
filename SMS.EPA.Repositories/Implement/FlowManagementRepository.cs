// -----------------------------------------------------------------------
// <copyright file="FlowManagementRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.17">Created</change>
// </history>

namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.FlowManagement;
    using SMS.EPA.Model.TargetType;
    using SMS.EPA.Repositories.Interfaces;

    public class FlowManagementRepository : IFlowManagementRepository
    {
        public IEnumerable<FlowTemplateModel> GetFlowTemplate(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<FlowTemplateModel, TargetTypeModel, FlowTemplateModel>(
                   (ft, tt) =>
                   {
                       ft.TargetType = tt;
                       return ft;
                   },
                   "TargetTypeId",
                   criteria.GetSpParams());
            }
        }

        public FlowTemplateModel GetFlowTemplateById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.QueryEntity<FlowTemplateModel, TargetTypeModel, FlowTemplateModel>(
                   (ft, tt) =>
                   {
                       ft.TargetType = tt;
                       return ft;
                   },
                   "TargetTypeId",
                   criteria.GetSpParams());
            }
        }

        public IEnumerable<FlowModel> GetFlowManagement(ICriteria criteria)
        {
            IEnumerable<FlowModel> epaFlow = null;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                epaFlow = db.Query<FlowModel, EmployeeModel, FlowModel>(
                   (f, e) =>
                   {
                       f.EvaluatorInfo = e;
                       return f;
                   },
                   "EmployeeId",
                   criteria.GetSpParams());
            }

            return epaFlow;
        }
    }
}