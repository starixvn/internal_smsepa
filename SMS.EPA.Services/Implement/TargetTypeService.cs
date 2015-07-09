// -----------------------------------------------------------------------
// <copyright file="TargetTypeService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.09">Created</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Model.TargetType;
    using SMS.EPA.Services.Interfaces;

    /// <summary>
    /// The implement of Target Type Service.
    /// </summary>
    public class TargetTypeService : BaseService, ITargetTypeService
    {
        public IEnumerable<TargetTypeModel> GetTargetType(GetTargetTypeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<TargetTypeModel> kpis = new List<TargetTypeModel>();

            if (criteria.IsValid())
            {
                kpis = this.GetData<TargetTypeModel>(criteria);
            }

            return kpis;
        }
    }
}