namespace SMS.EPA.Services.Interfaces
{
    using SMS.EPA.Model.Report.EPADetail;
    using System.IO;

    public interface IMonthlyEPADetailReportService
    {
        MemoryStream Export(EPADetailModel epaDetail);

        void Render(EPADetailModel epaDetail);
    }
}
