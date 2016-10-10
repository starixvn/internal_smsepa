namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;

    public class FlowTemplateRepository : IFlowTemplateRepository
    {
        public IEnumerable<FlowTemplateModel> GetFlowTemplate(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<FlowTemplateModel, TargetType, FlowTemplateModel>(
                   (ft, tt) =>
                   {
                       ft.TargetType = tt;
                       return ft;
                   },
                   "TargetTypeId",
                   criteria.GetSpParams());
            }
        }

        public FlowTemplateModel GetFlowTemplateById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.QueryEntity<FlowTemplateModel, TargetType, FlowTemplateModel>(
                   (ft, tt) =>
                   {
                       ft.TargetType = tt;
                       return ft;
                   },
                   "TargetTypeId",
                   criteria.GetSpParams());
            }
        }
    }
}