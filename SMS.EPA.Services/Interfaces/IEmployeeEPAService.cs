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
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA;

    public interface IEmployeeEPAService
    {
        IEnumerable<EmployeeEPAModel> GetEPAByEmployee(GetEPAByEmployeeCriteria criteria);

        EmployeeEPAInfoModel GetEPAInfo(GetEPAInfoCriteria criteria);
    }
}