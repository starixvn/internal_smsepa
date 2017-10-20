namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria;
    using Model;

    public interface ISelfEvaluationService
    {
        IEnumerable<EPAModel> GetEPAByEmployee(GetEPAByEmployeeCriteria criteria);

        EPAModel GetEPAInfoByEmployee(GetEPAInfoByEmployeeCriteria criteria);
    }
}