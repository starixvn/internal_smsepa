// -----------------------------------------------------------------------
// <copyright file="IKPIGroupService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.08">Created</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.KPIGroup;
    using SMS.EPA.Model.KPIGroup;

    /// <summary>
    /// The interface of KPI Group Service.
    /// </summary>
    public interface IKPIGroupService
    {
        IEnumerable<KPIGroupModel> GetKPIGroup(GetKPIGroupCriteria criteria);

        CommandResult InsertKPIGroup(InsertKPIGroupCriteria criteria);
    }
}