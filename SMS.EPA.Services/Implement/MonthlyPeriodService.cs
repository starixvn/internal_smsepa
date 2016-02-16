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
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.MonthlyPeriod;
    using SMS.EPA.Model.FlowManagement;
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

        public PeriodModel GetMonthlyPeriodById(GetMonthlyPeriodByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            PeriodModel period = new PeriodModel();

            if (criteria.IsValid())
            {
                period = GetDataEntity<PeriodModel>(criteria);
            }

            return period;
        }

        public CommandResult CreateMonthlyPeriod(CreateMonthlyPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateBasicInfoMonthlyPeriod(UpdateBasicInfoMonthlyPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public IEnumerable<FlowModel> GetProcessSettingByPeriodId(GetProcessSettingMonthlyPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<FlowModel> processSettings = new List<FlowModel>();

            if (criteria.IsValid())
            {
                processSettings = GetData<FlowModel>(criteria);
            }

            return processSettings;
        }

        public CommandResult UpdateProcessSettingMonthlyPeriod(UpdateProcessSettingMonthlyPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteMonthlyPeriod(DeleteMonthlyPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }
    }
}