// ----------------------------------------------------------------------
// <copyright file="EPAHistoryRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="10/19/2015">Created.</change>
// </history>

namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.EPAHistory;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Repositories.Interfaces;

    public class EPAHistoryRepository : IEPAHistoryRepository
    {
        public IList<EPAHistoryModel> GetEPAHistory(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IList<EPAHistoryModel> epas = db.Query<EPAHistoryModel, PeriodModel, EmployeeModel, EPAHistoryModel>(
                    (epa, pe, emp) =>
                    {
                        epa.Employee = emp;
                        epa.Period = pe;
                        return epa;
                    },
                    "PeriodId,EmployeeId",
                    criteria.GetSpParams()).ToList();

                return epas;
            }
        }
    }
}
