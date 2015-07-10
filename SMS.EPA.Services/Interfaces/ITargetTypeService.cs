// -----------------------------------------------------------------------
// <copyright file="ITargetTypeService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.09">Created</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Model.TargetType;

    public interface ITargetTypeService
    {
        IEnumerable<TargetTypeModel> GetTargetType(GetTargetTypeCriteria criteria);
    }
}