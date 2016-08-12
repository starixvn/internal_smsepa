namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.ScoreRatingType;
    using SMS.EPA.Criteria.TargetTemplate;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.ScoreRatingType;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class TargetTemplateController : BaseController
    {
        private readonly ITargetTemplateService targetTemplateService;
        private readonly IScoreRatingTypeService scoreRatingTypeService;

        public TargetTemplateController()
            : this(null, null)
        {
        }

        public TargetTemplateController(
            ITargetTemplateService targetTemplateService,
            IScoreRatingTypeService scoreRatingTypeService)
        {
            this.targetTemplateService = targetTemplateService ?? new TargetTemplateService();
            this.scoreRatingTypeService = scoreRatingTypeService ?? new ScoreRatingTypeService();
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

            targetTemplates = targetTemplateService.GetAllTargetTemplate(getAllTargetTemplateCriteria);
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

            targetTemplate = targetTemplateService.GetTargetTemplateById(getTargetTemplateByIdCriteria);
            return targetTemplate;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Host)]
        public CommandResult CreateTargetTemplate(CreateTargetTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = this.SessionService.EmployeeId;
            }

            CommandResult commandResult = targetTemplateService.CreateTargetTemplate(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Host)]
        public CommandResult UpdateStatusTargetTemplate(UpdateStatusTargetTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = this.SessionService.EmployeeId;
            }

            CommandResult commandResult = targetTemplateService.UpdateStatusTargetTemplate(criteria);
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
                    ModifiedBy = this.SessionService.EmployeeId,
                    Name = targetTemplate.Name,
                    TargetTypeId = targetTemplate.TargetTypeId,
                    ScoreRatingTypeId = targetTemplate.ScoreRatingTypeId,
                    RatingTypeId = targetTemplate.RatingTypeId,
                    IsActive = targetTemplate.IsActive,
                    Description = targetTemplate.Description
                };

                commandResult = targetTemplateService.UpdateTargetTemplateBasicInformation(updateTargetTemplateCriteria);
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
                    ModifiedBy = this.SessionService.EmployeeId,
                    KPIs = targetTemplate.ConvertKPIItemToXml(targetTemplate.KPIIds)
                };

                commandResult = targetTemplateService.UpdateTargetTemplate(updateTargetTemplateCriteria);
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
                ModifiedBy = this.SessionService.EmployeeId
            };

            CommandResult commandResult = targetTemplateService.DeleteTargetTemplate(criteria);
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
                    ModifiedBy = this.SessionService.EmployeeId
                };

                commandResult = targetTemplateService.AddKPIGroupWeight(criteria);
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

            IEnumerable<KPIGroupModel> groups = targetTemplateService.GetKPIGroupOfTargetTemplate(criteria);
            return groups;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Read)]
        public IEnumerable<TargetTemplateKPIGroupModel> GetKPIOfTargetTemplate(int targetTemplateId, bool onlySelected = false)
        {
            var criteria = new GetKPIOfTargetTemplateCriteria
            {
                TargetTemplateId = targetTemplateId
            };

            IEnumerable<TargetTemplateKPIGroupModel> kpiList = targetTemplateService.GetKPIOfTargetTemplate(criteria, onlySelected);
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
                ModifiedBy = this.SessionService.EmployeeId
            };

            CommandResult commandResult = targetTemplateService.DeleteKPIGroup(criteria);
            return commandResult;
        }

        public int GetUserPermissionOnTargetTemplate()
        {
            int permission = this.SessionService.GetPermission(EPAFunctionKey.TargetTemplate);
            return permission;
        }

        public IEnumerable<ScoreRatingTypeModel> GetScoreRatingType(int? scoreRatingTypeId = null, bool isActive = true)
        {
            var criteria = new GetScoreRatingTypeCriteria
            {
                ScoreRatingTypeId = scoreRatingTypeId,
                IsActive = isActive
            };

            IEnumerable<ScoreRatingTypeModel> scoreRatingTypes = scoreRatingTypeService.GetScoreRatingType(criteria);
            return scoreRatingTypes;
        }

        public IEnumerable<RatingType> GetRatingType()
        {
            var criteria = new GetRatingTypeCriteria();
            IEnumerable<RatingType> ratingTypes = targetTemplateService.GetRatingType(criteria);
            return ratingTypes;
        }
    }
}