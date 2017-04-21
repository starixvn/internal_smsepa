namespace SMS.EPA.Services.Interfaces
{
    using System.IO;
    using Model;

    public interface IMonthlyEPADetailReportService
    {
        MemoryStream Export(EPADetailModel epaDetail);

        void Render(EPADetailModel epaDetail);
    }
}