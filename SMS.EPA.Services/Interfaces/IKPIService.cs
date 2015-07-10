// -----------------------------------------------------------------------
// <copyright file="IKPIService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.07">Created</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.KPI;
    using SMS.EPA.Model.KPI;

    public interface IKPIService
    {
        IEnumerable<KPIModel> GetKPI(GetKPICriteria criteria);

        CommandResult CreateKPI(CreateKPICriteria criteria);

        CommandResult UpdateKPI(UpdateKPICriteria criteria);

        CommandResult UpdateStatusKPI(UpdateStatusKPICriteria criteria);

        CommandResult UpdateRequiredEvaluateKPI(UpdateRequiredEvaluateKPICriteria criteria);

        CommandResult DeleteKPI(DeleteKPICriteria criteria);
    }
}