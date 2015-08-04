// -----------------------------------------------------------------------
// <copyright file="MemberEPAService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.04">Created</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Criteria.MemberEPA;
    using SMS.EPA.Model.MemberEPA;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class MemberEPAService : BaseService, IMemberEPAService
    {
        private readonly IMemberEPARepository _memberEPARepository;

        public MemberEPAService()
            : this(null)
        {
        }

        public MemberEPAService(IMemberEPARepository memberEPARepository)
        {
            _memberEPARepository = memberEPARepository ?? new MemberEPARepository();
        }

        public IEnumerable<MemberEPAModel> GetEPAByEmployee(GetEPAByEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var employeeEPAs = new List<MemberEPAModel>();

            if (criteria.IsValid())
            {
                employeeEPAs = (List<MemberEPAModel>)_memberEPARepository.GetEPAByEmployee(criteria);

                foreach (var employeeEPA in employeeEPAs)
                {
                    employeeEPA.Status.Name = Enumerations.GetDescription((Status)employeeEPA.Status.StatusId);
                }
            }

            return employeeEPAs;
        }
    }
}