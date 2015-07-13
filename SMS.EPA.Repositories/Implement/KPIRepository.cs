// -----------------------------------------------------------------------
// <copyright file="KPIRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.07">Created</change>
// </history>

namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
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
            var result = new List<KPIGroupModel>();

            foreach (var kpiKPIGroup in kpiKPIGroups)
            {
                if (kpiKPIGroup.KPIId == kpiId)
                {
                    var kpiGroup = new KPIGroupModel()
                    {
                        KPIGroupId = kpiKPIGroup.KPIGroupId,
                        Name = kpiKPIGroup.Name
                    };

                    result.Add(kpiGroup);
                }
            }

            return result;
        }
    }
}