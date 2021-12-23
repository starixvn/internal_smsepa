namespace SMS.EPA.Services
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