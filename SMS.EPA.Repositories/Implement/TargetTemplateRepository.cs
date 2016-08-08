namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;
    using SMS.EPA.Repositories.Interfaces;

    public class TargetTemplateRepository : ITargetTemplateRepository
    {
        public IEnumerable<KPIModel> GetKPIOfTargetTemplate(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<KPIModel, KPIGroupModel, KPIModel>(
                    (kpi, gr) =>
                    {
                        kpi.KPIGroups.Add(gr);
                        return kpi;
                    },
                    "KPIGroupId",
                    criteria.GetSpParams());
            }
        }
    }
}