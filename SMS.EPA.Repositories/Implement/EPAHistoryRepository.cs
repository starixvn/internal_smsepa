namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using Model.EPAHistory;

    public class EPAHistoryRepository : IEPAHistoryRepository
    {
        public IList<EPAHistoryModel> GetEPAHistory(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<EPAHistoryModel, EPAPeriodModel, EPAEmployeeInfo, EPAHistoryModel>(
                    (epa, pe, emp) =>
                    {
                        epa.Employee = emp;
                        epa.Period = pe;
                        return epa;
                    },
                    "PeriodId,EmployeeId",
                    criteria).ToList();
            }
        }

        public IList<EPAHistoryModel> GetMonthlyEPAHistory(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<EPAHistoryModel, EPAPeriodModel, EPAEmployeeInfo, EPAHistoryModel>(
                    (epa, pe, emp) =>
                    {
                        epa.Employee = emp;
                        epa.Period = pe;
                        return epa;
                    },
                    "PeriodId,EmployeeId",
                    criteria).ToList();
            }
        }
    }
}