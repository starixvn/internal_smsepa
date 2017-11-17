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
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
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
            IList<KPIGroupModel> result = kpiGroups.Where(x => x.KPIId == kpiId)
                .Select(x => new KPIGroupModel
                {
                    KPIGroupId = x.KPIGroupId,
                    Name = x.Name
                }).ToList();

            return result;
        }
    }
}