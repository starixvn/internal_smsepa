namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Model.EPAHistory;

    public interface IEPAHistoryRepository
    {
        IList<EPAHistoryModel> GetEPAHistory(ICriteria criteria);

        IList<EPAHistoryModel> GetMonthlyEPAHistory(ICriteria criteria);
    }
}