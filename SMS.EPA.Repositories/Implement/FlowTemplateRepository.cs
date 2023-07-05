namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;

    public class FlowTemplateRepository : IFlowTemplateRepository
    {
        public IEnumerable<FlowTemplateModel> GetFlowTemplate(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<FlowTemplateModel, TargetTypeModel, FlowTemplateModel>(
                   (ft, tt) =>
                   {
                       ft.TargetType = tt;
                       return ft;
                   },
                   "TargetTypeId",
                   criteria);
            }
        }

        public FlowTemplateModel GetFlowTemplateById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.QueryEntity<FlowTemplateModel, TargetTypeModel, FlowTemplateModel>(
                   (ft, tt) =>
                   {
                       ft.TargetType = tt;
                       return ft;
                   },
                   "TargetTypeId",
                   criteria);
            }
        }
    }
}