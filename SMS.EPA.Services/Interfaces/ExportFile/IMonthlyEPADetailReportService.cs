namespace SMS.EPA.Services.Interfaces
{
    using Model;

    public interface IMonthlyEPADetailReportService
    {
        void Export(EPADetailModel epaDetail);

        void Render(EPADetailModel epaDetail);
    }
}