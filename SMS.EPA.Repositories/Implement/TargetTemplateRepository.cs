namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;

    public class TargetTemplateRepository : ITargetTemplateRepository
    {
        public IEnumerable<KPIModel> GetKPIOfTargetTemplate(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<KPIModel, KPIGroupModel, KPIModel>(
                    (kpi, gr) =>
                    {
                        kpi.KPIGroups.Add(gr);
                        return kpi;
                    },
                    "KPIGroupId",
                    criteria);
            }
        }
    }
}