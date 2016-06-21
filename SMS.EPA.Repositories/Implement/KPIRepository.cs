namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.KPI;
    using SMS.EPA.Model.KPIGroup;
    using SMS.EPA.Repositories.Interfaces;

    public class KPIRepository : IKPIRepository
    {
        public IEnumerable<KPIModel> GetKPI(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<KPIModel> kpis = multi.Read<KPIModel>();
                    IEnumerable<RawKPIModel> kpiKPIGroups = multi.Read<RawKPIModel>();

                    foreach (var kpi in kpis)
                    {
                        kpi.KPIGroups = GroupKPIGroupByKPI(kpi.KPIId, kpiKPIGroups);
                    }

                    return kpis;
                }
            }
        }

        private IList<KPIGroupModel> GroupKPIGroupByKPI(int kpiId, IEnumerable<RawKPIModel> kpiKPIGroups)
        {
            IList<KPIGroupModel> result = kpiKPIGroups.Where(x => x.KPIId == kpiId)
                .Select(x => new KPIGroupModel
                {
                    KPIGroupId = x.KPIGroupId,
                    Name = x.Name
                }).ToList();

            return result;
        }
    }
}