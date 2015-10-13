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
    using System.Linq;
    using Fanex.Common.Helper;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Core.Utilities;
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
                epaInfo.CurrentProcess = epaInfo.ProcessBySelf;

                foreach (EPAKPIGroupModel kpiGroup in epaInfo.KPIGroups)
                {
                    foreach (EPAKPIModel kpi in kpiGroup.KPIs)
                    {
                        kpi.CurrentEvaluation = kpi.EvaluationBySelf;
                    }
                }
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

                foreach (EPAModel memberEPA in memberEPAs)
                {
                    memberEPA.CurrentProcess = memberEPA.ProcessByEvaluators.FirstOrDefault();
                }
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
                epaInfo.CurrentProcess = epaInfo.ProcessByEvaluators.FirstOrDefault();

                foreach (EPAKPIGroupModel kpiGroup in epaInfo.KPIGroups)
                {
                    foreach (EPAKPIModel kpi in kpiGroup.KPIs)
                    {
                        kpi.CurrentEvaluation = kpi.EvaluationByEvaluators.FirstOrDefault();
                    }
                }
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

            foreach (EPAModel memberEPA in memberEPAs)
            {
                memberEPA.CurrentProcess = memberEPA.ProcessByReviewers.FirstOrDefault();
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
                epaInfo.CurrentProcess = epaInfo.ProcessByReviewers.FirstOrDefault();
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

            foreach (EPAModel memberEPA in memberEPAs)
            {
                memberEPA.CurrentProcess = memberEPA.ProcessByManagements.FirstOrDefault();
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
                epaInfo.CurrentProcess = epaInfo.ProcessByManagements.FirstOrDefault();
            }

            return epaInfo;
        }

        #endregion Management

        #region Rating By KPI

        public IList<Group> GetGroupByEvaluator(GetGroupByEvaluatorCriteria criteria)
        {
            IList<Group> groups = new List<Group>();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                groups = GetData<Group>(criteria).ToList();
            }

            return groups;
        }

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

        public IEnumerable<StandardGradeModel> GetStandardGrade(GetStandardGradeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<StandardGradeModel> standardGrades = new List<StandardGradeModel>();

            if (criteria.IsValid())
            {
                standardGrades = this.GetData<StandardGradeModel>(criteria);
            }

            return standardGrades;
        }

        public IEnumerable<EPASummaryImportModel> GetMemberEPASummaryImport(GetEPASummaryImportCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<EPASummaryImportModel> memberEPAs = new List<EPASummaryImportModel>();

            if (criteria.IsValid())
            {
                memberEPAs = this.GetData<EPASummaryImportModel>(criteria);
            }

            return memberEPAs;
        }

        public CommandResult UpdateEPASummaryImport(UpdateEPASummaryImportCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public string ConvertScoreToGrade(float score)
        {
            string grade = string.Empty;
            var criteria = new GetStandardGradeCriteria();
            IEnumerable<StandardGradeModel> standardGrades = GetStandardGrade(criteria);

            foreach (StandardGradeModel standardGrade in standardGrades)
            {
                if (score >= standardGrade.LeftScore && score <= standardGrade.RightScore)
                {
                    StandardGrade gradeEnum = GetGradeEnum(standardGrade.GradeId);
                    grade = Enumerations.GetDescription(gradeEnum);
                }
            }

            return grade;
        }

        public int CheckEmployeePermission(GetEmployeePermissionCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            int result = -1;

            if (criteria.IsValid())
            {
                result = GetDataEntity<int>(criteria);
            }

            return result;
        }

        private StandardGrade GetGradeEnum(int gradeId)
        {
            StandardGrade grade = StandardGrade.None;

            switch (gradeId)
            {
                case (int)StandardGrade.Excellent:
                    grade = StandardGrade.Excellent;
                    break;
                case (int)StandardGrade.Good:
                    grade = StandardGrade.Good;
                    break;
                case (int)StandardGrade.Standard:
                    grade = StandardGrade.Standard;
                    break;
                case (int)StandardGrade.UnderPerformance:
                    grade = StandardGrade.UnderPerformance;
                    break;
                case (int)StandardGrade.Fail:
                    grade = StandardGrade.Fail;
                    break;
            }

            return grade;
        }
    }
}