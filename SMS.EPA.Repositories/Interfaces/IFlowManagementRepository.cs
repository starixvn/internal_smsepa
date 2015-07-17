// -----------------------------------------------------------------------
// <copyright file="IFlowManagementRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.17">Created</change>
// </history>

namespace SMS.EPA.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.FlowManagement;

    public interface IFlowManagementRepository
    {
        IEnumerable<EPAFlowModel> GetFlowManagement(ICriteria criteria);
    }
}