// -----------------------------------------------------------------------
// <copyright file="IMemberEPAService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.04">Created</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria.MemberEPA;
    using SMS.EPA.Model.MemberEPA;

    public interface IMemberEPAService
    {
        IEnumerable<MemberEPAModel> GetEPAByEmployee(GetEPAByEmployeeCriteria criteria);
    }
}