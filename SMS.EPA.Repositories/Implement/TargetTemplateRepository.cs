// ----------------------------------------------------------------------
// <copyright file="TargetTemplateRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/13/2015 3:44:43 PM">Created.</change>
// </history>

namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.KPI;
    using SMS.EPA.Model.KPIGroup;
    using SMS.EPA.Model.TargetType;
    using SMS.EPA.Repositories.Interfaces;

    public class TargetTemplateRepository : ITargetTemplateRepository
    {
        public IEnumerable<KPIModel> GetKPIOfTargetTemplate(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<KPIModel, KPIGroupModel, TargetTypeModel, KPIModel>(
                    (kpi, gr, tar) =>
                    {
                        kpi.KPIGroups.Add(gr);
                        kpi.TargetType = tar;
                        return kpi;
                    },
                    "KPIGroupId,TargetTypeId",
                    criteria.GetSpParams());
            }
        }
    }
}
