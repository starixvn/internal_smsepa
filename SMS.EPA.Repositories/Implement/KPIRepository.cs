namespace SMS.EPA.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using System.Collections.Generic;
    using System.Linq;

    public class KPIRepository : IKPIRepository
    {
        public IEnumerable<KPIModel> GetKPI(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria))
                {
                    IEnumerable<KPIModel> kpis = multi.Read<KPIModel>();
                    IEnumerable<dynamic> kpiGroups = multi.Read<dynamic>();

                    foreach (var kpi in kpis)
                    {
                        kpi.KPIGroups = GroupKPIGroupByKPI(kpi.KPIId, kpiGroups);
                    }

                    return kpis;
                }
            }
        }

        private IList<KPIGroupModel> GroupKPIGroupByKPI(int kpiId, IEnumerable<dynamic> kpiGroups)
        {
            return kpiGroups.Where(x => x.KPIId == kpiId)
                .Select(x => new KPIGroupModel
                {
                    KPIGroupId = x.KPIGroupId,
                    Name = x.Name
                }).ToList();
        }
    }
}