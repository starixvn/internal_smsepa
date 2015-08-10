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
    using SMS.EPA.Core.Services.Interfaces;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class EmployeeEPAService : BaseService, IEmployeeEPAService
    {
        private readonly IEmployeeEPARepository _memberEPARepository;
        private readonly IEncodeService _encodeService;

        public EmployeeEPAService()
            : this(null, null)
        {
        }

        public EmployeeEPAService(
            IEmployeeEPARepository memberEPARepository,
            IEncodeService encodeService)
        {
            _memberEPARepository = memberEPARepository ?? new EmployeeEPARepository();
            _encodeService = encodeService ?? new EncodeService();
        }

        public IEnumerable<EmployeeEPAModel> GetEPAByEmployee(GetEPAByEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<EmployeeEPAModel> employeeEPAs = new List<EmployeeEPAModel>();

            if (criteria.IsValid())
            {
                employeeEPAs = _memberEPARepository.GetEPAByEmployee(criteria);

                foreach (var employeeEPA in employeeEPAs)
                {
                    employeeEPA.Status.Name = Enumerations.GetDescription((Status)employeeEPA.Status.StatusId);
                    employeeEPA.Score = !string.IsNullOrEmpty(employeeEPA.Score) ? Decode(employeeEPA.Score) : string.Empty;
                }
            }

            return employeeEPAs;
        }

        public string Encode(string plainText)
        {
            return _encodeService.Encode(plainText);
        }

        public string Decode(string cipherText)
        {
            return _encodeService.Decode(cipherText);
        }
    }
}