namespace SMS.EPA.Site.Controllers.API
{
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Criteria;
    using Model;
    using Model.Enitites;
    using Services.Implement;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Http;

    public class TargetTemplateController : BaseController
    {
        private readonly ITargetTemplateService _targetTemplateService;
        private readonly IScoreRatingTypeService _scoreRatingTypeService;

        public TargetTemplateController()
            : this(null, null)
        {
        }

        public TargetTemplateController(
            ITargetTemplateService targetTemplateService,
            IScoreRatingTypeService scoreRatingTypeService)
        {
            _targetTemplateService = targetTemplateService ?? new TargetTemplateService();
            _scoreRatingTypeService = scoreRatingTypeService ?? new ScoreRatingTypeService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Read)]
        public IEnumerable<TargetTemplate> GetAllTargetTemplate(int? targetTypeId = null, bool? isActive = null)
        {
            var criteria = new GetAllTargetTemplateCriteria
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive
            };
            IEnumerable<TargetTemplate> targetTemplates = _targetTemplateService.GetAllTargetTemplate(criteria);

            return targetTemplates;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Read)]
        public TargetTemplate GetTargetTemplateById(int targetTemplateId)
        {
            var criteria = new GetTargetTemplateByIdCriteria
            {
                TargetTemplateId = targetTemplateId
            };
            TargetTemplate targetTemplate = _targetTemplateService.GetTargetTemplateById(criteria);

            return targetTemplate;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Host)]
        public CommandResult CreateTargetTemplate(CreateTargetTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = SessionService.EmployeeId;
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
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = _targetTemplateService.UpdateStatusTargetTemplate(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Host)]
        public CommandResult UpdateTargetTemplateBasicInfo(UpdateTargetTemplateBasicInfoCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = _targetTemplateService.UpdateTargetTemplateBasicInfo(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Host)]
        public CommandResult UpdateTargetTemplate(UpdateTargetTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = _targetTemplateService.UpdateTargetTemplate(criteria);
            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Host)]
        public CommandResult DeleteTargetTemplate(int targetTemplateId)
        {
            var criteria = new DeleteTargetTemplateCriteria
            {
                TargetTemplateId = targetTemplateId,
                ModifiedBy = SessionService.EmployeeId
            };

            CommandResult commandResult = _targetTemplateService.DeleteTargetTemplate(criteria);
            return commandResult;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetTemplate, Policy = Policy.Host)]
        public CommandResult AddKPIGroupWeight(SetKPIGroupOfTargetTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

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
        public IEnumerable<TargetTemplateKPIGroupModel> GetKPIOfTargetTemplate(int targetTemplateId, bool onlySelected = false)
        {
            var criteria = new GetKPIOfTargetTemplateCriteria
            {
                TargetTemplateId = targetTemplateId
            };
            IEnumerable<TargetTemplateKPIGroupModel> kpis = _targetTemplateService.GetKPIOfTargetTemplate(criteria, onlySelected);

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
                ModifiedBy = SessionService.EmployeeId
            };
            CommandResult commandResult = _targetTemplateService.DeleteKPIGroup(criteria);

            return commandResult;
        }

        public int GetUserPermissionOnTargetTemplate()
        {
            int permission = SessionService.GetPermission(EPAFunctionKey.TargetTemplate);
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