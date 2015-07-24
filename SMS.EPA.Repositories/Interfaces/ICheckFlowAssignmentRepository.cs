// ----------------------------------------------------------------------
// <copyright file="ICheckFlowAssignmentRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/24/2015">Created.</change>
// </history>

namespace SMS.EPA.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using SMS.EPA.Model.CheckFlowAssignment;

    public interface ICheckFlowAssignmentRepository
    {
        RawEmployeeFlowTargetSettingModel GetEmployeeFlowTargetSetting(ICriteria criteria);
    }
}
