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
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.TargetTemplate;
    using SMS.EPA.Model.KPIGroup;
    using SMS.EPA.Model.TargetTemplate;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class TargetTemplateController : BaseController
    {
        private readonly ITargetTemplateService _targetTemplateService;

        public TargetTemplateController()
            : this(null)
        {
        }

        public TargetTemplateController(
            ITargetTemplateService targetTemplateService)
        {
            this._targetTemplateService = targetTemplateService ?? new TargetTemplateService();
        }

        public IEnumerable<TargetTemplateModel> GetAllTargetTemplate(int? targetTypeId = null, bool? isActive = null)
        {
            IEnumerable<TargetTemplateModel> targetTemplates = null;
            var getAllTargetTemplateCriteria = new GetAllTargetTemplateCriteria
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive
            };

            targetTemplates = this._targetTemplateService.GetAllTargetTemplate(getAllTargetTemplateCriteria);
            return targetTemplates;
        }

        public TargetTemplateModel GetTargetTemplateById(int targetTemplateId)
        {
            TargetTemplateModel targetTemplate = null;
            var getTargetTemplateByIdCriteria = new GetTargetTemplateByIdCriteria
            {
                TargetTemplateId = targetTemplateId
            };

            targetTemplate = this._targetTemplateService.GetTargetTemplateById(getTargetTemplateByIdCriteria);
            return targetTemplate;
        }

        [HttpPost]
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
                    Description = targetTemplate.Description,
                    ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId,
                    KPIs = targetTemplate.ConvertKPIItemToXml
                };

                commandResult = _targetTemplateService.UpdateTargetTemplate(updateTargetTemplateCriteria);
            }

            return commandResult;
        }

        [HttpDelete]
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

        public IEnumerable<KPIGroupModel> GetKPIGroupOfTargetTemplate(int targetTemplateId)
        {
            var criteria = new GetKPIGroupOfTargetTemplateCriteria
            {
                TargetTemplateId = targetTemplateId
            };

            IEnumerable<KPIGroupModel> groups = _targetTemplateService.GetKPIGroupOfTargetTemplate(criteria);
            return groups;
        }

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
    }
}