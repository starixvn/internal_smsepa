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
    using SMS.EPA.Repositories.Interfaces;

    public class FlowManagementRepository : IFlowManagementRepository
    {
        public IEnumerable<EPAFlowModel> GetFlowManagement(ICriteria criteria)
        {
            IEnumerable<EPAFlowModel> epaFlow = null;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                epaFlow = db.Query<EPAFlowModel, EmployeeModel, EPAFlowModel>(
                   (f, e) =>
                   {
                       f.EvaluatorInfo = e;
                       return f;
                   },
                   "EvaluatorId",
                   criteria.GetSpParams());
            }

            return epaFlow;
        }
    }
}