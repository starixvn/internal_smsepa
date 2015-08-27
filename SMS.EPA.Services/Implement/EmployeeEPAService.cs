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

        #region Self

        public IEnumerable<EPAModel> GetEPAByEmployee(GetEPAByEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<EPAModel> myEPAs = new List<EPAModel>();

            if (criteria.IsValid())
            {
                myEPAs = _employeeEPARepository.GetEPAByEmployee(criteria);

                foreach (var myEPA in myEPAs)
                {
                    myEPA.Status.Name = Enumerations.GetDescription((Status)myEPA.Status.StatusId);
                    myEPA.FinalScore = !string.IsNullOrEmpty(myEPA.FinalScore) ? Decode(myEPA.FinalScore) : string.Empty;
                    myEPA.ProcessBySelf.Status.Name = Enumerations.GetDescription((Status)myEPA.ProcessBySelf.Status.StatusId);
                    myEPA.ProcessBySelf.TotalScore = !string.IsNullOrEmpty(myEPA.ProcessBySelf.TotalScore) ? Decode(myEPA.ProcessBySelf.TotalScore) : string.Empty;
                }
            }

            return myEPAs;
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
                    result.Status.Name = Enumerations.GetDescription((Status)result.Status.StatusId);
                    result.FinalScore = !string.IsNullOrEmpty(result.FinalScore) ? Decode(result.FinalScore) : string.Empty;
                    result.ProcessBySelf.TotalScore = !string.IsNullOrEmpty(result.ProcessBySelf.TotalScore) ? Decode(result.ProcessBySelf.TotalScore) : string.Empty;
                    result.ProcessBySelf.Status.Name = Enumerations.GetDescription((Status)result.ProcessBySelf.Status.StatusId);

                    foreach (var info in result.KPIGroups)
                    {
                        foreach (var kpi in info.KPIs)
                        {
                            kpi.EvaluateBySelf.Score = !string.IsNullOrEmpty(kpi.EvaluateBySelf.Score) ? Decode(kpi.EvaluateBySelf.Score).Replace("_" + kpi.KPIId, string.Empty) : string.Empty;
                        }
                    }
                }
            }

            return result;
        }

        #endregion Self

        #region Evaluator

        public IEnumerable<EPAModel> GetEPAByEvaluator(GetEPAByEvaluatorCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<EPAModel> memberEPAs = new List<EPAModel>();

            if (criteria.IsValid())
            {
                memberEPAs = _employeeEPARepository.GetEPAByEvaluator(criteria);

                foreach (var memberEPA in memberEPAs)
                {
                    memberEPA.Status.Name = Enumerations.GetDescription((Status)memberEPA.Status.StatusId);
                    memberEPA.FinalScore = !string.IsNullOrEmpty(memberEPA.FinalScore) ? Decode(memberEPA.FinalScore) : string.Empty;
                    memberEPA.ProcessBySelf.Status.Name = Enumerations.GetDescription((Status)memberEPA.ProcessBySelf.Status.StatusId);
                    memberEPA.ProcessBySelf.TotalScore = !string.IsNullOrEmpty(memberEPA.ProcessBySelf.TotalScore) ? Decode(memberEPA.ProcessBySelf.TotalScore) : string.Empty;
                    DecodeTotalScoreAndGetStatus(memberEPA.ProcessByEvaluators);
                }
            }

            return memberEPAs;
        }

        #endregion Evaluator

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

        private void DecodeTotalScoreAndGetStatus(IList<EPAProcessModel> epaProcesses)
        {
            foreach (var epaProcess in epaProcesses)
            {
                epaProcess.Status.Name = Enumerations.GetDescription((Status)epaProcess.Status.StatusId);
                epaProcess.TotalScore = !string.IsNullOrEmpty(epaProcess.TotalScore) ? Decode(epaProcess.TotalScore) : string.Empty;
            }
        }
    }
}