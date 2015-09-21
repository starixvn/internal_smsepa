// -----------------------------------------------------------------------
// <copyright file="IEmployeeEPAService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.04">Created</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Criteria.EmployeeEPA.RatingByKPI;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA.RatingByKPI;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.TargetTemplate;

    public interface IEmployeeEPAService
    {
        #region Self

        IEnumerable<EPAModel> GetEPAByEmployee(GetEPAByEmployeeCriteria criteria);

        EPAModel GetEPAInfoByEmployee(GetEPAInfoByEmployeeCriteria criteria);

        #endregion Self

        #region Evaluator

        IEnumerable<EPAModel> GetEPAByEvaluator(GetEPAByEvaluatorCriteria criteria);

        EPAModel GetEPAInfoByEvaluator(GetEPAInfoByEvaluatorCriteria criteria);

        #endregion Evaluator

        #region Reviewer

        IEnumerable<EPAModel> GetEPAByReviewer(GetEPAByReviewerCriteria criteria);

        EPAModel GetEPAInfoByReviewer(GetEPAInfoByReviewerCriteria criteria);

        #endregion Reviewer

        #region Management

        IEnumerable<EPAModel> GetEPAByManagement(GetEPAByManagementCriteria criteria);

        EPAModel GetEPAInfoByManagement(GetEPAInfoByManagementCriteria criteria);

        #endregion Management

        #region Rating By KPI

        IEnumerable<TargetTemplateModel> GetTargetTemplateByEvaluator(GetTargetTemplateByEvaluatorCriteria criteria);

        IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(GetKPIByTargetTemplateCriteria criteria);

        IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(GetEmployeeEPAByKPICriteria criteria);

        CommandResult UpdateKPIScore(UpdateKPIScoreCriteria criteria);

        #endregion Rating By KPI

        EPAModel GetEPAInfoById(GetEPAInfoByIdCriteria criteria);

        CommandResult UpdateEPAScore(UpdateEPAScoreCriteria criteria);

        CommandResult UpdateScoreByReviewerOrManagement(UpdateScoreByReviewerOrManagementCriteria criteria);

        CommandResult AdjustScoreByReviewerOrManagement(AdjustScoreByReviewerOrManagementCriteria criteria);

        IEnumerable<Employee> GetMember(GetMemberCriteria criteria);

        string Encode(string plainText);

        string Decode(string cipherText);
    }
}