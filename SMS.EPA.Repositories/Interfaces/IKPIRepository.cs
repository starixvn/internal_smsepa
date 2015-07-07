// -----------------------------------------------------------------------
// <copyright file="IKPIRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.07">Created</change>
// </history>

namespace SMS.EPA.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.KPI;

    /// <summary>
    /// The interface of KPI Repository.
    /// </summary>
    public interface IKPIRepository
    {
        IEnumerable<KPIModel> GetKPI(ICriteria criteria);
    }
}