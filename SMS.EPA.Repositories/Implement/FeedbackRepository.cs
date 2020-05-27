namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Feedback;

    public class FeedbackRepository : IFeedbackRepository
    {
        public IEnumerable<Feedback> GetByActor(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<Feedback, FeedbackEmployee, Feedback>(
                   (fb, employee) =>
                   {
                       fb.Employee = employee;
                       return fb;
                   },
                   "EmployeeId",
                   criteria.GetSpParams());
            }
        }

        public Feedback GetById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var feedback = multi.Read<Feedback, FeedbackEmployee, Feedback>(
                       (fb, employee) =>
                       {
                           fb.Employee = employee;
                           return fb;
                       },
                       "EmployeeId").FirstOrDefault();
                    if (feedback != null)
                    {
                        feedback.KPIGroups = multi.Read<FeedbackKPIGroup>();
                        var kpis = multi.Read<FeedbackKPI>();

                        foreach (FeedbackKPIGroup kpigr in feedback.KPIGroups)
                        {
                            kpigr.KPIs = kpis.Where(x => x.KPIGroupId == kpigr.KPIGroupId).OrderBy(x => x.DisplayOrder);
                        }
                    }

                    return feedback;
                }
            }
        }
    }
}