// ----------------------------------------------------------------------
// <copyright file="ITargetEmployeeSettingService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/17/2015">Created.</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria.TargetEmployeeSetting;
    using SMS.EPA.Model.TargetEmployeeSetting;

    public interface ITargetEmployeeSettingService
    {
        IList<TargetEmployeeModel> GetTargetEmployeeSetting(GetAllTargetEmployeeSettingCriteria criteria);
    }
}
