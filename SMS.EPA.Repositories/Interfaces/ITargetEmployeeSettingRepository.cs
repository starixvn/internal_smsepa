// ----------------------------------------------------------------------
// <copyright file="ITargetEmployeeSettingRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/17/2015">Created.</change>
// </history>

namespace SMS.EPA.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using SMS.EPA.Model.TargetEmployeeSetting;

    public interface ITargetEmployeeSettingRepository
    {
        RawTargetEmployeeSettingModel GetTargetEmployeeSetting(ICriteria criteria);
    }
}
