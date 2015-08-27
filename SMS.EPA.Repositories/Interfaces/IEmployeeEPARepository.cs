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
        IEnumerable<EPAModel> GetEPAByEmployee(ICriteria criteria);

        EPAModel GetEPAInfoByEmployee(ICriteria criteria);

        IEnumerable<EPAModel> GetEPAByEvaluator(ICriteria criteria);
    }
}