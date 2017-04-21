namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Core.Model;
    using Criteria;
    using Model;
    using Model.Enitites;

    public interface ITargetTemplateService
    {
        TargetTemplate GetTargetTemplateById(GetTargetTemplateByIdCriteria criteria);

        IEnumerable<TargetTemplate> GetAllTargetTemplate(GetAllTargetTemplateCriteria criteria);

        CommandResult CreateTargetTemplate(CreateTargetTemplateCriteria criteria);

        CommandResult UpdateTargetTemplate(UpdateTargetTemplateCriteria criteria);

        CommandResult UpdateTargetTemplateBasicInfo(UpdateTargetTemplateBasicInfoCriteria criteria);

        CommandResult DeleteTargetTemplate(DeleteTargetTemplateCriteria criteria);

        CommandResult AddKPIGroupWeight(SetKPIGroupOfTargetTemplateCriteria criteria);

        IEnumerable<KPIGroupModel> GetKPIGroupOfTargetTemplate(GetKPIGroupOfTargetTemplateCriteria criteria);

        CommandResult UpdateStatusTargetTemplate(UpdateStatusTargetTemplateCriteria criteria);

        CommandResult DeleteKPIGroup(DeleteKPIGroupOfTargetTemplateCriteria criteria);

        IEnumerable<TargetTemplateKPIGroupModel> GetKPIOfTargetTemplate(GetKPIOfTargetTemplateCriteria criteria, bool onlyKPISelected);

        IEnumerable<RatingType> GetRatingType(GetRatingTypeCriteria criteria);
    }
}