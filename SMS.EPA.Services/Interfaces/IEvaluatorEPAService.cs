namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Core.Model;
    using Criteria;
    using Model;

    public interface IEvaluatorEPAService
    {
        IEnumerable<EPAModel> GetEPAByEvaluator(GetEPAByEvaluatorCriteria criteria);

        EPAModel GetEPAInfoByEvaluator(GetEPAInfoByEvaluatorCriteria criteria);

        CommandResult UpdateEPAScoreByEvaluator(UpdateEPAScoreCriteria criteria);
    }
}