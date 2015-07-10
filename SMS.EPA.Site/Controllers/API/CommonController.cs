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
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Model.TargetType;

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

        /// <summary>
        /// Get all system role.
        /// </summary>
        /// <param name="exceptNone">Except none or not.</param>
        /// <returns>The list system role.</returns>
        public List<object> GetSystemRoles(bool exceptNone)
        {
            var roles = new List<object>();

            foreach (SystemRole roleEnum in (SystemRole[])System.Enum.GetValues(typeof(SystemRole)))
            {
                var role = new
                {
                    RoleId = (int)roleEnum,
                    RoleName = Enumerations.GetDescription(roleEnum)
                };

                if (exceptNone && roleEnum != 0)
                {
                    roles.Add(role);
                }
                else if (!exceptNone)
                {
                    roles.Add(role);
                }
            }

            return roles;
        }
    }
}