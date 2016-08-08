namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.ScoreRatingType;
    using SMS.EPA.Criteria.TargetTemplate;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
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
        public CommandResult UpdateTargetTemplateBasicInformation(TargetTemplateModel targetTemplate)
        {
            CommandResult commandResult = new CommandResult();

            if (ModelState.IsValid && targetTemplate != null)
            {
                var updateTargetTemplateCriteria = new UpdateTargetTemplateBasicInformationCriteria()
                {
                    TargetTemplateId = targetTemplate.TargetTemplateId,
                    ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId,
                    Name = targetTemplate.Name,
                    TargetTypeId = targetTemplate.TargetTypeId,
                    ScoreRatingTypeId = targetTemplate.ScoreRatingTypeId,
                    RatingTypeId = targetTemplate.RatingTypeId,
                    IsActive = targetTemplate.IsActive,
                    Description = targetTemplate.Description
                };

                commandResult = _targetTemplateService.UpdateTargetTemplateBasicInformation(updateTargetTemplateCriteria);
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Host)]
        public CommandResult UpdateTargetTemplate(TargetTemplateModel targetTemplate)
        {
            CommandResult commandResult = new CommandResult();

            if (ModelState.IsValid && targetTemplate != null)
            {
                var updateTargetTemplateCriteria = new UpdateTargetTemplateCriteria()
                {
                    TargetTemplateId = targetTemplate.TargetTemplateId,
                    ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId,
                    KPIs = targetTemplate.ConvertKPIItemToXml(targetTemplate.KPIIds)
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
            CommandResult commandResult = new CommandResult();
            if (model != null)
            {
                var criteria = new SetKPIGroupOfTargetTemplateCriteria
                {
                    TargetTemplateId = model.TargetTemplateId,
                    KPIGroups = model.ConvertKPIGroupWeightToXml,
                    ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
                };

                commandResult = _targetTemplateService.AddKPIGroupWeight(criteria);
            }

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
        public IEnumerable<TargetTemplateKPIGroupModel> GetKPIOfTargetTemplate(int targetTemplateId, bool onlySelected = false)
        {
            var criteria = new GetKPIOfTargetTemplateCriteria
            {
                TargetTemplateId = targetTemplateId
            };

            IEnumerable<TargetTemplateKPIGroupModel> kpiList = _targetTemplateService.GetKPIOfTargetTemplate(criteria, onlySelected);
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

        public IEnumerable<RatingType> GetRatingType()
        {
            var criteria = new GetRatingTypeCriteria();
            IEnumerable<RatingType> ratingTypes = _targetTemplateService.GetRatingType(criteria);
            return ratingTypes;
        }
    }
}