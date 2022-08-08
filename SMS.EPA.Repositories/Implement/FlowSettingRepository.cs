namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;

    public class FlowSettingRepository : IFlowSettingRepository
    {
        public IEnumerable<FlowModel> GetFlowSettingByFlowTemplateId(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<FlowModel, FlowTemplateModel, EvaluatorInfoViewModel, FlowModel>(
                   (f, ft, e) =>
                   {
                       f.FlowTemplate = ft;
                       f.EvaluatorInfo = e;
                       return f;
                   },
                   "FlowTemplateId, EmployeeId",
                   criteria);
            }
        }
    }
}