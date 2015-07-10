// -----------------------------------------------------------------------
// <copyright file="KPIGroupService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.08">Created</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.KPIGroup;
    using SMS.EPA.Model.KPIGroup;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class KPIGroupService : BaseService, IKPIGroupService
    {
        private readonly IKPIGroupRepository _kpiGroupRepository;

        public KPIGroupService()
            : this(null)
        {
        }

        public KPIGroupService(IKPIGroupRepository kpiGroupRepository)
        {
            _kpiGroupRepository = kpiGroupRepository ?? new KPIGroupRepository();
        }

        public IEnumerable<KPIGroupModel> GetKPIGroup(GetKPIGroupCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<KPIGroupModel> kpiGroups = new List<KPIGroupModel>();

            if (criteria.IsValid())
            {
                kpiGroups = _kpiGroupRepository.GetKPIGroup(criteria);
            }

            return kpiGroups;
        }

        public KPIGroupModel GetKPIGroupById(GetKPIGroupByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            KPIGroupModel kpiGroup = new KPIGroupModel();

            if (criteria.IsValid())
            {
                kpiGroup = _kpiGroupRepository.GetKPIGroupById(criteria);
            }

            return kpiGroup;
        }

        public CommandResult CreateKPIGroup(CreateKPIGroupCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateKPIGroup(UpdateKPIGroupCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateStatusKPIGroup(UpdateStatusKPIGroupCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteKPIGroup(DeleteKPIGroupCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }
    }
}