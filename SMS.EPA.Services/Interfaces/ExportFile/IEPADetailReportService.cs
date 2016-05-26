namespace SMS.EPA.Services.Interfaces
{
    using SMS.EPA.Model.Report.EPADetail;
    using System.IO;

    public interface IEPADetailReportService
    {
        MemoryStream Export(EPADetailModel epaDetail);

        void Render(EPADetailModel epaDetail);
    }
}
