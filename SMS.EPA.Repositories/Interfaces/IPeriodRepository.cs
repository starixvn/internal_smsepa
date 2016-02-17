// ----------------------------------------------------------------------
// <copyright file="IPeriodRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/13/2015">Created.</change>
// </history>

namespace SMS.EPA.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Period;

    public interface IPeriodRepository
    {
        IEnumerable<PeriodModel> GetPeriod(ICriteria criteria);

        PeriodModel GetPeriodById(ICriteria criteria);

        IEnumerable<PeriodEmployeeSettingModel> GetPeriodEmployeeSettingById(ICriteria criteria);

        IEnumerable<PeriodFlowModel> GetEmployeeSettingByPeriodId(ICriteria criteria);
    }
}