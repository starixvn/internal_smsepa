namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Core.Model;
    using Criteria;
    using Model;

    public interface ISelfEvaluationService
    {
        IEnumerable<EPAModel> GetEPAByEmployee(GetEPAByEmployeeCriteria criteria);

        EPAModel GetEPAInfoByEmployee(GetEPAInfoByEmployeeCriteria criteria);

        CommandResult UpdateEPAScoreByEmployee(UpdateEPAScoreCriteria criteria);
    }
}