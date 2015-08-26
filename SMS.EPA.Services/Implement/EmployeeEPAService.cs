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
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Core.Services.Interfaces;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class EmployeeEPAService : BaseService, IEmployeeEPAService
    {
        private readonly IEmployeeEPARepository _employeeEPARepository;
        private readonly IEncodeService _encodeService;

        public EmployeeEPAService()
            : this(null, null)
        {
        }

        public EmployeeEPAService(
            IEmployeeEPARepository employeeEPARepository,
            IEncodeService encodeService)
        {
            _employeeEPARepository = employeeEPARepository ?? new EmployeeEPARepository();
            _encodeService = encodeService ?? new EncodeService();
        }

        public IEnumerable<EPAModel> GetEPAByEmployee(GetEPAByEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<EPAModel> employeeEPAs = new List<EPAModel>();

            if (criteria.IsValid())
            {
                employeeEPAs = _employeeEPARepository.GetEPAByEmployee(criteria);

                foreach (var employeeEPA in employeeEPAs)
                {
                    employeeEPA.ProcessBySelf.Status.Name = Enumerations.GetDescription((Status)employeeEPA.ProcessBySelf.Status.StatusId);
                    employeeEPA.FinalScore = !string.IsNullOrEmpty(employeeEPA.FinalScore) ? Decode(employeeEPA.FinalScore) : string.Empty;
                    employeeEPA.ProcessBySelf.TotalScore = !string.IsNullOrEmpty(employeeEPA.ProcessBySelf.TotalScore) ? Decode(employeeEPA.ProcessBySelf.TotalScore) : string.Empty;
                }
            }

            return employeeEPAs;
        }

        public EPAModel GetEPAInfoByEmployee(GetEPAInfoCriteria criteria)
        {
            EPAModel result = new EPAModel();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                result = _employeeEPARepository.GetEPAInfoByEmployee(criteria);

                if (result != null)
                {
                    result.ProcessBySelf.Status.Name = Enumerations.GetDescription((Status)result.ProcessBySelf.Status.StatusId);
                    result.FinalScore = !string.IsNullOrEmpty(result.FinalScore) ? Decode(result.FinalScore) : string.Empty;
                    result.ProcessBySelf.TotalScore = !string.IsNullOrEmpty(result.ProcessBySelf.TotalScore) ? Decode(result.ProcessBySelf.TotalScore) : string.Empty;

                    foreach (var info in result.EPAInfoBySelf)
                    {
                        foreach (var kpi in info.KPIGroup.KPIs)
                        {
                            kpi.Score = !string.IsNullOrEmpty(kpi.Score) ? Decode(kpi.Score).Replace("_" + kpi.KPIId, string.Empty) : string.Empty;
                        }
                    }
                }
            }

            return result;
        }

        public CommandResult UpdateEPAScore(UpdateEPAScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public IEnumerable<Employee> GetMember(GetMemberCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<Employee> employees = new List<Employee>();

            if (criteria.IsValid())
            {
                employees = this.GetData<Employee>(criteria);
            }

            return employees;
        }

        public CommandResult UpdateStatusEPA(UpdateStatusEPACriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
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