// ----------------------------------------------------------------------
// <copyright file="PeriodService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/13/2015">Created.</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class PeriodService : BaseService, IPeriodService
    {
        private readonly IPeriodRepository _periodRepository;

        public PeriodService()
            : this(null)
        {
        }

        public PeriodService(IPeriodRepository periodRepository)
        {
            _periodRepository = periodRepository ?? new PeriodRepository();
        }

        public IEnumerable<PeriodModel> GetPeriod(GetPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<PeriodModel> periods = new List<PeriodModel>();

            if (criteria.IsValid())
            {
                periods = _periodRepository.GetPeriod(criteria);
            }

            return periods;
        }

        public CommandResult CreatePeriod(CreatePeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateBasicInformationPeriod(UpdateBasicInformationPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public PeriodModel GetPeriodById(GetPeriodByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            PeriodModel period = new PeriodModel();

            if (criteria.IsValid())
            {
                period = _periodRepository.GetPeriodById(criteria);
            }

            return period;
        }

        public CommandResult UpdateStatusPeriod(UpdateStatusPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeletePeriod(DeletePeriodCriteria criteria)
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
