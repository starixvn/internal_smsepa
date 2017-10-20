namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria;
    using Model;

    public interface IManagementEPAService
    {
        IEnumerable<EPAModel> GetEPAByManagement(GetEPAByManagementCriteria criteria);

        EPAModel GetEPAInfoByManagement(GetEPAInfoByManagementCriteria criteria);
    }
}