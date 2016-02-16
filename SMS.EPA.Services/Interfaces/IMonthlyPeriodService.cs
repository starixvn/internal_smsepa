// -----------------------------------------------------------------------
// <copyright file="IMonthlyPeriodService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2016.02.15">Created</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.MonthlyPeriod;
    using SMS.EPA.Model.Period;

    public interface IMonthlyPeriodService
    {
        IEnumerable<PeriodModel> GetMonthlyPeriod(GetMonthlyPeriodCriteria criteria);

        PeriodModel GetMonthlyPeriodById(GetMonthlyPeriodByIdCriteria criteria);

        CommandResult CreateMonthlyPeriod(CreateMonthlyPeriodCriteria criteria);

        CommandResult UpdateBasicInfoMonthlyPeriod(UpdateBasicInfoMonthlyPeriodCriteria criteria);
    }
}