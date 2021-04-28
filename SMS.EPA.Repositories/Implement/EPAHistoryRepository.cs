namespace SMS.EPA.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using Model.EPAHistory;
    using System.Collections.Generic;
    using System.Linq;

    public class EPAHistoryRepository : IEPAHistoryRepository
    {
        public IList<EPAHistoryModel> GetEPAHistory(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IList<EPAHistoryModel> epas = db.Query<EPAHistoryModel, EPAPeriodModel, EPAEmployeeInfo, EPAHistoryModel>(
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

        public IList<EPAHistoryModel> GetMonthlyEPAHistory(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IList<EPAHistoryModel> epas = db.Query<EPAHistoryModel, EPAPeriodModel, EPAEmployeeInfo, EPAHistoryModel>(
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