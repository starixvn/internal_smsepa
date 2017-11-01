namespace SMS.EPA.Repositories
{
    using Fanex.Data.Repository;
    using Model;

    public interface IReportRepository
    {
        EPASummaryModel GetEPASummary(ICriteria criteria);

        EPADetailModel GetEPADetail(ICriteria criteria);

        EPASummaryModel GetMonthlyEPASummary(ICriteria criteria);

        EPADetailModel GetMonthlyEPADetail(ICriteria criteria);
    }
}