// -----------------------------------------------------------------------
// <copyright file="CommonController.cs" company="Nexcel Solutions Viet Nam">
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
    using SMS.EPA.Model.TargetType;

    /// <summary>
    /// The Common controller.
    /// </summary>
    public class CommonController : BaseController
    {
        private readonly UtilitiesController _utilitiesController;

        public CommonController()
            : this(null)
        {
        }

        public CommonController(UtilitiesController utilitiesController)
        {
            _utilitiesController = utilitiesController ?? new UtilitiesController();
        }

        [HttpGet]
        public IEnumerable<TargetTypeModel> GetTargetType(bool? isActive = null)
        {
            return _utilitiesController.GetTargetType(isActive);
        }
    }
}