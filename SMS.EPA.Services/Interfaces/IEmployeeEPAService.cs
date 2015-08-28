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
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Enitites;

    public interface IEmployeeEPAService
    {
        IEnumerable<EPAModel> GetEPAByEmployee(GetEPAByEmployeeCriteria criteria);

        EPAModel GetEPAInfoByEmployee(GetEPAInfoCriteria criteria);

        IEnumerable<EPAModel> GetEPAByEvaluator(GetEPAByEvaluatorCriteria criteria);

        EPAModel GetEPAInfoByEvaluator(GetEPAInfoByEvaluatorCriteria criteria);

        IEnumerable<EPAModel> GetEPAByReviewer(GetEPAByReviewerCriteria criteria);

        EPAModel GetEPAInfoByReviewer(GetEPAInfoByReviewerCriteria criteria);

        CommandResult UpdateEPAScore(UpdateEPAScoreCriteria criteria);

        CommandResult UpdateScoreByReviewerOrManagement(UpdateScoreByReviewerOrManagementCriteria criteria);

        IEnumerable<Employee> GetMember(GetMemberCriteria criteria);

        CommandResult UpdateStatusEPA(UpdateStatusEPACriteria criteria);

        string Encode(string plainText);

        string Decode(string cipherText);
    }
}