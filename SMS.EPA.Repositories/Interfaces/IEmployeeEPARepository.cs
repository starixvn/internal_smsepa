// -----------------------------------------------------------------------
// <copyright file="IEmployeeEPARepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.04">Created</change>
// </history>

namespace SMS.EPA.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.EmployeeEPA;

    public interface IEmployeeEPARepository
    {
        IEnumerable<EmployeeEPAModel> GetEPAByEmployee(ICriteria criteria);

        EmployeeEPAInfoModel GetEPAInfo(ICriteria criteria);

        IEnumerable<MemberEPAModel> GetEPAByEvaluator(ICriteria criteria);

        MemberEPAInfoModel GetEPAInfoByEvaluator(ICriteria criteria);
    }
}