// -----------------------------------------------------------------------
// <copyright file="IMemberEPARepository.cs" company="Nexcel Solutions Viet Nam">
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
    using SMS.EPA.Model.MemberEPA;

    public interface IMemberEPARepository
    {
        IEnumerable<MemberEPAModel> GetEPAByEmployee(ICriteria criteria);
    }
}