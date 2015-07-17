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
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Model.KPI;
    using SMS.EPA.Model.KPIGroup;
    using SMS.EPA.Model.TargetType;
    using SMS.EPA.Web.ApiAuthorizeAttribute;

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

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPI, Policy = Policy.Read)]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Read)]
        public IEnumerable<KPIModel> GetKPI(
            int? targetTypeId = null,
            int? kpiGroupId = null,
            bool? isActive = null,
            bool? isRequiredEvaluate = null)
        {
            return _utilitiesController.GetKPI(targetTypeId, kpiGroupId, isActive, isRequiredEvaluate);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPI, Policy = Policy.Read)]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Read)]
        public IEnumerable<KPIGroupModel> GetKPIGroup(
            int? targetTypeId = null,
            bool? isActive = null)
        {
            return _utilitiesController.GetKPIGroup(targetTypeId, isActive);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPI, Policy = Policy.Read)]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Read)]
        public KPIGroupModel GetKPIGroupById(int kpiGroupId)
        {
            return _utilitiesController.GetKPIGroupById(kpiGroupId);
        }

        public List<object> GetSystemRoles(bool exceptNone)
        {
            var roles = new List<object>();

            foreach (SystemRole roleEnum in (SystemRole[])System.Enum.GetValues(typeof(SystemRole)))
            {
                var role = new
                {
                    RoleId = (int)roleEnum,
                    RoleCode = Enumerations.GetDescription(roleEnum)
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