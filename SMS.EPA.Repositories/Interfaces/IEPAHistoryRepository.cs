namespace SMS.EPA.Repositories
{
    using Fanex.Data.Repository;
    using Model.EPAHistory;
    using System.Collections.Generic;

    public interface IEPAHistoryRepository
    {
        IList<EPAHistoryModel> GetEPAHistory(ICriteria criteria);

        IList<EPAHistoryModel> GetMonthlyEPAHistory(ICriteria criteria);
    }
}