// -----------------------------------------------------------------------
// <copyright file="IKPIGroupRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.09">Created</change>
// </history>

namespace SMS.EPA.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.KPIGroup;

    public interface IKPIGroupRepository
    {
        IEnumerable<KPIGroupModel> GetKPIGroup(ICriteria criteria);

        KPIGroupModel GetKPIGroupById(ICriteria criteria);
    }
}