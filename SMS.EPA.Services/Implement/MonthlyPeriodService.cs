// -----------------------------------------------------------------------
// <copyright file="MonthlyPeriodService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2016.02.15">Created</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.MonthlyPeriod;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Services.Interfaces;

    public class MonthlyPeriodService : BaseService, IMonthlyPeriodService
    {
        public IEnumerable<PeriodModel> GetMonthlyPeriod(GetMonthlyPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<PeriodModel> periods = new List<PeriodModel>();

            if (criteria.IsValid())
            {
                periods = GetData<PeriodModel>(criteria);
            }

            return periods;
        }
    }
}