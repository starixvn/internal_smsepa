// -----------------------------------------------------------------------
// <copyright file="KPIGroupRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.09">Created</change>
// </history>

namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.KPIGroup;
    using SMS.EPA.Model.TargetType;
    using SMS.EPA.Repositories.Interfaces;

    /// <summary>
    /// The implement of KPI Group Repository.
    /// </summary>
    public class KPIGroupRepository : IKPIGroupRepository
    {
        public IEnumerable<KPIGroupModel> GetKPIGroup(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<KPIGroupModel, TargetTypeModel, KPIGroupModel>(
                    (g, ty) =>
                    {
                        g.TargetType = ty;

                        return g;
                    },
                    "TargetTypeId",
                    criteria.GetSpParams());
            }
        }
    }
}