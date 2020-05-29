namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Feedback;
    using SMS.EPA.Model.Report.Feedback;

    public interface IFeedbackReportRepository
    {
        IEnumerable<FeedbackReport> GetData(ICriteria criteria);
    }

    public class FeedbackReportRepository : IFeedbackReportRepository
    {
        public IEnumerable<FeedbackReport> GetData(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var employees = multi.Read<FeedbackReport>();
                    var feedbacks = multi.Read<FeedbackReportItem, FeedbackEmployee, FeedbackReportItem>(
                       (fb, evaluator) =>
                       {
                           fb.Evaluator = evaluator;
                           return fb;
                       },
                       "EvaluatorId");

                    foreach (FeedbackReport employee in employees)
                    {
                        employee.Feedbacks = feedbacks.Where(x => x.EmployeeId == employee.EmployeeId);
                    }

                    return employees;
                }
            }
        }
    }
}