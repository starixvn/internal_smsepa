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

    /// <summary>
    /// The implement of KPI Group Service.
    /// </summary>
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

        public CommandResult InsertKPIGroup(InsertKPIGroupCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }
    }
}