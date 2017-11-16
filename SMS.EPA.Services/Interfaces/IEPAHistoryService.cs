namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.EPAHistory;
    using Model.EPAHistory;

    public interface IEPAHistoryService
    {
        IList<EPAHistoryModel> GetEPAHistory(GetEPAHistoryCriteria criteria);

        IList<EPAHistoryModel> GetMonthlyEPAHistory(GetMonthlyEPAHistoryCriteria criteria);
    }
}