// ----------------------------------------------------------------------
// <copyright file="TargetTemplateController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/10/2015 12:39:44 PM">Created and clone from NexEPA.</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.ScoreRatingType;
    using SMS.EPA.Criteria.TargetTemplate;
    using SMS.EPA.Model.KPIGroup;
    using SMS.EPA.Model.ScoreRatingType;
    using SMS.EPA.Model.TargetTemplate;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Web.ApiAuthorizeAttribute;
    using SMS.EPA.Web.Services;

    public class TargetTemplateController : BaseController
    {
        private readonly ITargetTemplateService _targetTemplateService;
        private readonly IWebCommonService _webCommonService;
        private readonly IScoreRatingTypeService _scoreRatingTypeService;

        public TargetTemplateController()
            : this(null, null, null)
        {
        }

        public TargetTemplateController(
            ITargetTemplateService targetTemplateService,
            IWebCommonService webCommonService,
            IScoreRatingTypeService scoreRatingTypeService)
        {
            _targetTemplateService = targetTemplateService ?? new TargetTemplateService();
            _webCommonService = webCommonService ?? new WebCommonService();
            _scoreRatingTypeService = scoreRatingTypeService ?? new ScoreRatingTypeService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Read)]
        public IEnumerable<TargetTemplateModel> GetAllTargetTemplate(int? targetTypeId = null, bool? isActive = null)
        {
            IEnumerable<TargetTemplateModel> targetTemplates = null;
            var getAllTargetTemplateCriteria = new GetAllTargetTemplateCriteria
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive
            };

            targetTemplates = _targetTemplateService.GetAllTargetTemplate(getAllTargetTemplateCriteria);
            return targetTemplates;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Read)]
        public TargetTemplateModel GetTargetTemplateById(int targetTemplateId)
        {
            TargetTemplateModel targetTemplate = null;
            var getTargetTemplateByIdCriteria = new GetTargetTemplateByIdCriteria
            {
                TargetTemplateId = targetTemplateId
            };

            targetTemplate = _targetTemplateService.GetTargetTemplateById(getTargetTemplateByIdCriteria);
            return targetTemplate;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Host)]
        public CommandResult CreateTargetTemplate(CreateTargetTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _targetTemplateService.CreateTargetTemplate(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Host)]
        public CommandResult UpdateStatusTargetTemplate(UpdateStatusTargetTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _targetTemplateService.UpdateStatusTargetTemplate(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Host)]
        public CommandResult UpdateTargetTemplate(TargetTemplateModel targetTemplate)
        {
            CommandResult commandResult = new CommandResult();

            if (ModelState.IsValid)
            {
                var updateTargetTemplateCriteria = new UpdateTargetTemplateCriteria()
                {
                    TargetTemplateId = targetTemplate.TargetTemplateId,
                    Name = targetTemplate.Name,
                    TargetTypeId = targetTemplate.TargetTypeId,
                    ScoreRatingTypeId = targetTemplate.ScoreRatingTypeId,
                    IsActive = targetTemplate.IsActive,
                    Description = targetTemplate.Description,
                    ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId,
                    KPIs = targetTemplate.ConvertKPIItemToXml
                };

                commandResult = _targetTemplateService.UpdateTargetTemplate(updateTargetTemplateCriteria);
            }

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Host)]
        public CommandResult DeleteTargetTemplate(int targetTemplateId)
        {
            var criteria = new DeleteTargetTemplateCriteria
            {
                TargetTemplateId = targetTemplateId,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            CommandResult commandResult = _targetTemplateService.DeleteTargetTemplate(criteria);
            return commandResult;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Host)]
        public CommandResult AddKPIGroupWeight(TargetKPIGroupWeightModel model)
        {
            var criteria = new SetKPIGroupOfTargetTemplateCriteria
            {
                TargetTemplateId = model.TargetTemplateId,
                KPIGroups = model.ConvertKPIGroupWeightToXml,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            CommandResult commandResult = _targetTemplateService.AddKPIGroupWeight(criteria);
            return commandResult;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Read)]
        public IEnumerable<KPIGroupModel> GetKPIGroupOfTargetTemplate(int targetTemplateId)
        {
            var criteria = new GetKPIGroupOfTargetTemplateCriteria
            {
                TargetTemplateId = targetTemplateId
            };

            IEnumerable<KPIGroupModel> groups = _targetTemplateService.GetKPIGroupOfTargetTemplate(criteria);
            return groups;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Read)]
        public IEnumerable<TargetTemplateKPIGroupModel> GetKPIOfTargetTemplate(int targetTemplateId)
        {
            var criteria = new GetKPIOfTargetTemplateCriteria
            {
                TargetTemplateId = targetTemplateId
            };

            IEnumerable<TargetTemplateKPIGroupModel> kpiList = _targetTemplateService.GetKPIOfTargetTemplate(criteria);
            return kpiList;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Host)]
        public CommandResult DeleteKPIGroupOfTargetTemplate(int targetTemplateId, int kpiGroupId)
        {
            var criteria = new DeleteKPIGroupOfTargetTemplateCriteria
            {
                TargetTemplateId = targetTemplateId,
                KPIGroupId = kpiGroupId,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            CommandResult commandResult = _targetTemplateService.DeleteKPIGroup(criteria);
            return commandResult;
        }

        public int GetUserPermissionOnTargetTemplate()
        {
            int permission = _webCommonService.GetPermission(EPAFunctionKey.TargetTemplate);
            return permission;
        }

        public IEnumerable<ScoreRatingTypeModel> GetScoreRatingType(int? scoreRatingTypeId = null, bool isActive = true)
        {
            var criteria = new GetScoreRatingTypeCriteria
            {
                ScoreRatingTypeId = scoreRatingTypeId,
                IsActive = isActive
            };

            IEnumerable<ScoreRatingTypeModel> scoreRatingTypes = _scoreRatingTypeService.GetScoreRatingType(criteria);
            return scoreRatingTypes;
        }
    }
}