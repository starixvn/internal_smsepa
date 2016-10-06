namespace SMS.EPA.AutoService.Repository
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Period;

    public class EvaluationRepository : IEvaluationRepository
    {
        public AutoEmployeePeriodModel GetEmployeeProbationEvaluation(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReaderWrapper multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<int> employees = multi.Read<int>();
                    IEnumerable<int> periods = multi.Read<int>();
                    var employeePeriod = new AutoEmployeePeriodModel();

                    if (periods != null && periods.Any() && employees != null && employees.Any())
                    {
                        employeePeriod.PeriodId = periods.First();
                        employeePeriod.EmployeeIds = string.Join(",", employees);
                    }

                    return employeePeriod;
                }
            }
        }
    }
}