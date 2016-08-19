namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Criteria.ScoreRatingType;
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
            var criteria = new GetAllTargetTemplateCriteria
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive
            };
            IEnumerable<TargetTemplateModel> targetTemplates = targetTemplateService.GetAllTargetTemplate(criteria);

            return targetTemplates;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Read)]
        public TargetTemplateModel GetTargetTemplateById(int targetTemplateId)
        {
            var criteria = new GetTargetTemplateByIdCriteria
            {
                TargetTemplateId = targetTemplateId
            };
            TargetTemplateModel targetTemplate = targetTemplateService.GetTargetTemplateById(criteria);

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
        public CommandResult UpdateTargetTemplateBasicInfo(UpdateTargetTemplateBasicInfoCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = this.SessionService.EmployeeId;
            }

            CommandResult commandResult = targetTemplateService.UpdateTargetTemplateBasicInfo(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Host)]
        public CommandResult UpdateTargetTemplate(UpdateTargetTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = this.SessionService.EmployeeId;
            }

            CommandResult commandResult = targetTemplateService.UpdateTargetTemplate(criteria);
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
        public CommandResult AddKPIGroupWeight(SetKPIGroupOfTargetTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = this.SessionService.EmployeeId;
            }

            CommandResult commandResult = targetTemplateService.AddKPIGroupWeight(criteria);
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
            IEnumerable<TargetTemplateKPIGroupModel> kpis = targetTemplateService.GetKPIOfTargetTemplate(criteria, onlySelected);

            return kpis;
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