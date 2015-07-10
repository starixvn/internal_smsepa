// ----------------------------------------------------------------------
// <copyright file="ITargetTemplateService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/10/2015 11:26:44 AM">Created and clone from NexEPA.</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.TargetTemplate;
    using SMS.EPA.Model.KPIGroup;
    using SMS.EPA.Model.TargetTemplate;

    public interface ITargetTemplateService
    {
        TargetTemplateModel GetTargetTemplateById(GetTargetTemplateByIdCriteria criteria);

        IEnumerable<TargetTemplateModel> GetAllTargetTemplate(GetAllTargetTemplateCriteria criteria);

        CommandResult CreateTargetTemplate(CreateTargetTemplateCriteria criteria);

        CommandResult UpdateTargetTemplate(UpdateTargetTemplateCriteria criteria);

        CommandResult DeleteTargetTemplate(DeleteTargetTemplateCriteria criteria);

        CommandResult AddKPIGroupWeight(SetKPIGroupOfTargetTemplateCriteria criteria);

        IEnumerable<KPIGroupModel> GetSelectedKPIGroup(GetKPIGroupOfTargetTemplateCriteria criteria);

        CommandResult UpdateStatusTargetTemplate(UpdateStatusTargetTemplateCriteria criteria);

        CommandResult RemoveKPIGroup(DeleteKPIGroupOfTargetTemplateCriteria criteria);
    }
}
