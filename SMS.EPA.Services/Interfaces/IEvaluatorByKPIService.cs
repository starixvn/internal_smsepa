namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Core.Model;
    using Criteria;
    using Model;
    using Model.Enitites;

    public interface IEvaluatorByKPIService
    {
        IEnumerable<EPAPeriodModel> GetPeriodByEvaluator(GetPeriodByEvaluatorCriteria criteria);

        IEnumerable<TargetTemplate> GetTargetTemplateByEvaluator(GetTargetTemplateByEvaluatorCriteria criteria);

        IEnumerable<TargetTemplate> GetTargetTemplateToView(GetTargetTemplateToViewCriteria criteria);

        IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(GetKPIByTargetTemplateCriteria criteria);

        IEnumerable<EPAPeriodModel> GetPeriodToView(GetPeriodToViewCriteria criteria);

        IEnumerable<TemporaryKPIModel> GetKPIToView(GetKPIToViewCriteria criteria);

        IEnumerable<RatingByKPIModel> GetEmployeeEpabyKPI(GetEmployeeEPAByKPICriteria criteria);

        IEnumerable<RatingByKPIModel> GetEmployeeEPAToView(GetEmployeeEPAToViewCriteria criteria);

        IEnumerable<Employee> GetEvaluatorsByTargetTemplate(GetEvaluatorsByTargetTemplateCriteria criteria);

        CommandResult UpdateKPIScore(UpdateKPIScoreCriteria criteria);
    }
}