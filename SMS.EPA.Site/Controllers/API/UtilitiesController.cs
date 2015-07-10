// -----------------------------------------------------------------------
// <copyright file="UtilitiesController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.09">Created</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Model.TargetType;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class UtilitiesController
    {
        private readonly ITargetTypeService _targetTypeService;

        public UtilitiesController()
            : this(null)
        {
        }

        public UtilitiesController(ITargetTypeService targetTypeService)
        {
            _targetTypeService = targetTypeService ?? new TargetTypeService();
        }

        [NonAction]
        public IEnumerable<TargetTypeModel> GetTargetType(bool? isActive = null)
        {
            var criteria = new GetTargetTypeCriteria
            {
                IsActive = isActive
            };

            return _targetTypeService.GetTargetType(criteria);
        }
    }
}