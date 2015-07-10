// ----------------------------------------------------------------------
// <copyright file="TargetTemplateService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/10/2015 12:34:40 PM">Created.</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.TargetTemplate;
    using SMS.EPA.Model.KPIGroup;
    using SMS.EPA.Model.TargetTemplate;
    using SMS.EPA.Services.Interfaces;

    public class TargetTemplateService : BaseService, ITargetTemplateService
    {
        public TargetTemplateModel GetTargetTemplateById(GetTargetTemplateByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            TargetTemplateModel targetTemplate = null;
            if (criteria.IsValid())
            {
                targetTemplate = this.GetDataEntity<TargetTemplateModel>(criteria);
            }

            return targetTemplate;
        }

        public IEnumerable<TargetTemplateModel> GetAllTargetTemplate(GetAllTargetTemplateCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<TargetTemplateModel> targetTemplates = null;
            if (criteria.IsValid())
            {
                targetTemplates = GetData<TargetTemplateModel>(criteria);
            }

            return targetTemplates;
        }

        public CommandResult CreateTargetTemplate(CreateTargetTemplateCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = this.Repository.Insert(criteria);
            }

            return result;
        }

        public CommandResult UpdateTargetTemplate(UpdateTargetTemplateCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = this.Repository.Update(criteria);
            }

            return result;
        }

        public CommandResult DeleteTargetTemplate(DeleteTargetTemplateCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = this.Repository.Delete(criteria);
            }

            return result;
        }

        public CommandResult AddKPIGroupWeight(SetKPIGroupOfTargetTemplateCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = this.Repository.Insert(criteria);
            }

            return result;
        }

        public IEnumerable<KPIGroupModel> GetSelectedKPIGroup(GetKPIGroupOfTargetTemplateCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<KPIGroupModel> groups = null;
            if (criteria.IsValid())
            {
                groups = GetData<KPIGroupModel>(criteria);
            }

            return groups;
        }

        public CommandResult UpdateStatusTargetTemplate(UpdateStatusTargetTemplateCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;

            if (criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            return result;
        }

        public CommandResult RemoveKPIGroup(DeleteKPIGroupOfTargetTemplateCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Delete(criteria);
            }

            return result;
        }
    }
}
