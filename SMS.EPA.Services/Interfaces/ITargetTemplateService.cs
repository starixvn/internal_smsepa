namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;

    public interface ITargetTemplateService
    {
        TargetTemplateModel GetTargetTemplateById(GetTargetTemplateByIdCriteria criteria);

        IEnumerable<TargetTemplateModel> GetAllTargetTemplate(GetAllTargetTemplateCriteria criteria);

        CommandResult CreateTargetTemplate(CreateTargetTemplateCriteria criteria);

        CommandResult UpdateTargetTemplate(UpdateTargetTemplateCriteria criteria);

        CommandResult UpdateTargetTemplateBasicInformation(UpdateTargetTemplateBasicInformationCriteria criteria);

        CommandResult DeleteTargetTemplate(DeleteTargetTemplateCriteria criteria);

        CommandResult AddKPIGroupWeight(SetKPIGroupOfTargetTemplateCriteria criteria);

        IEnumerable<KPIGroupModel> GetKPIGroupOfTargetTemplate(GetKPIGroupOfTargetTemplateCriteria criteria);

        CommandResult UpdateStatusTargetTemplate(UpdateStatusTargetTemplateCriteria criteria);

        CommandResult DeleteKPIGroup(DeleteKPIGroupOfTargetTemplateCriteria criteria);

        IEnumerable<TargetTemplateKPIGroupModel> GetKPIOfTargetTemplate(GetKPIOfTargetTemplateCriteria criteria, bool onlyKPISelected);

        IEnumerable<RatingType> GetRatingType(GetRatingTypeCriteria criteria);
    }
}