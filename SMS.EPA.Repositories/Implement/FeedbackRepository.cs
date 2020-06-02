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
                    var feedback = multi.Read<Feedback, FeedbackEmployee, FeedbackEmployee, Feedback>(
                       (fb, employee, evaluator) =>
                       {
                           fb.Employee = employee;
                           fb.Evaluator = evaluator;
                           return fb;
                       },
                       "EmployeeId, EvaluatorId").FirstOrDefault();
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

        public IEnumerable<FeedbackSetting> GetSetting(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var settings = multi.Read<FeedbackSetting, FeedbackEmployee, FeedbackSetting>(
                       (fb, employee) =>
                       {
                           fb.Employee = employee;
                           return fb;
                       },
                       "EmployeeId");
                    var evaluators = multi.Read<dynamic>();

                    foreach (FeedbackSetting setting in settings)
                    {
                        setting.Evaluators = evaluators.Where(x => x.EmployeeId == setting.Employee.EmployeeId)
                            .Select(x => new FeedbackEmployee
                            {
                                EmployeeId = x.EvaluatorId,
                                Code = x.Code,
                                Nickname = x.Nickname,
                                FullName = x.FullName
                            });
                    }

                    return settings;
                }
            }
        }
    }
}