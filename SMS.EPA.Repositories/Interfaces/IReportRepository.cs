namespace SMS.EPA.Repositories
{
    using Fanex.Data.Repository;
    using SMS.EPA.Model;

    public interface IReportRepository
    {
        EPASummaryModel GetEPASummary(ICriteria criteria);

        EPADetailModel GetEPADetail(ICriteria criteria);

        EPADetailModel GetMonthlyEPADetail(ICriteria criteria);

        EPASummaryModel GetMonthlyEPASummary(ICriteria criteria);
    }
}