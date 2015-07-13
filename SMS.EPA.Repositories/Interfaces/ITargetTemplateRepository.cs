// ----------------------------------------------------------------------
// <copyright file="ITargetTemplateRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/13/2015 3:42:41 PM">Created.</change>
// </history>

namespace SMS.EPA.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.KPI;

    public interface ITargetTemplateRepository
    {
        IEnumerable<KPIModel> GetKPIOfTargetTemplate(ICriteria criteria);
    }
}
