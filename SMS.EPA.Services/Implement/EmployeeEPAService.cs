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
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Criteria.EmployeeEPA.RatingByKPI;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA.RatingByKPI;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.TargetTemplate;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class EmployeeEPAService : BaseService, IEmployeeEPAService
    {
        private readonly IEmployeeEPARepository _employeeEPARepository;

        public EmployeeEPAService()
            : this(null)
        {
        }

        public EmployeeEPAService(IEmployeeEPARepository employeeEPARepository)
        {
            _employeeEPARepository = employeeEPARepository ?? new EmployeeEPARepository();
        }

        #region Self

        public IEnumerable<EPAModel> GetEPAByEmployee(GetEPAByEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<EPAModel> epas = new List<EPAModel>();

            if (criteria.IsValid())
            {
                epas = _employeeEPARepository.GetEPA(criteria);
            }

            return epas;
        }

        public EPAModel GetEPAInfoByEmployee(GetEPAInfoByEmployeeCriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                epaInfo = _employeeEPARepository.GetEPAInfo(criteria);
            }

            return epaInfo;
        }

        #endregion Self

        #region Evaluator

        public IEnumerable<EPAModel> GetEPAByEvaluator(GetEPAByEvaluatorCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<EPAModel> memberEPAs = new List<EPAModel>();

            if (criteria.IsValid())
            {
                memberEPAs = _employeeEPARepository.GetEPA(criteria);
            }

            return memberEPAs;
        }

        public EPAModel GetEPAInfoByEvaluator(GetEPAInfoByEvaluatorCriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                epaInfo = _employeeEPARepository.GetEPAInfo(criteria);
            }

            return epaInfo;
        }

        #endregion Evaluator

        #region Reviewer

        public IEnumerable<EPAModel> GetEPAByReviewer(GetEPAByReviewerCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<EPAModel> memberEPAs = new List<EPAModel>();

            if (criteria.IsValid())
            {
                memberEPAs = _employeeEPARepository.GetEPA(criteria);
            }

            return memberEPAs;
        }

        public EPAModel GetEPAInfoByReviewer(GetEPAInfoByReviewerCriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                epaInfo = _employeeEPARepository.GetEPAInfo(criteria);
            }

            return epaInfo;
        }

        #endregion Reviewer

        #region Management

        public IEnumerable<EPAModel> GetEPAByManagement(GetEPAByManagementCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<EPAModel> memberEPAs = new List<EPAModel>();

            if (criteria.IsValid())
            {
                memberEPAs = _employeeEPARepository.GetEPA(criteria);
            }

            return memberEPAs;
        }

        public EPAModel GetEPAInfoByManagement(GetEPAInfoByManagementCriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                epaInfo = _employeeEPARepository.GetEPAInfo(criteria);
            }

            return epaInfo;
        }

        #endregion Management

        #region Rating By KPI

        public IEnumerable<TargetTemplateModel> GetTargetTemplateByEvaluator(GetTargetTemplateByEvaluatorCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<TargetTemplateModel> targetTemplates = new List<TargetTemplateModel>();

            if (criteria.IsValid())
            {
                targetTemplates = this.GetData<TargetTemplateModel>(criteria);
            }

            return targetTemplates;
        }

        public IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(GetKPIByTargetTemplateCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<TemporaryKPIModel> kpis = new List<TemporaryKPIModel>();

            if (criteria.IsValid())
            {
                kpis = _employeeEPARepository.GetKPIByTargetTemplate(criteria);
            }

            return kpis;
        }

        public IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(GetEmployeeEPAByKPICriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<RatingByKPIModel> epas = new List<RatingByKPIModel>();

            if (criteria.IsValid())
            {
                epas = _employeeEPARepository.GetEmployeeEPAByKPI(criteria);
            }

            return epas;
        }

        public CommandResult UpdateKPIScore(UpdateKPIScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        #endregion Rating By KPI

        public EPAModel GetEPAInfoById(GetEPAInfoByIdCriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                epaInfo = _employeeEPARepository.GetEPAInfo(criteria);
            }

            return epaInfo;
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

        public CommandResult AdjustScoreByReviewerOrManagement(AdjustScoreByReviewerOrManagementCriteria criteria)
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
    }
}