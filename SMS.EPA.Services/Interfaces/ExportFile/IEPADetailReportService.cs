namespace SMS.EPA.Services.Interfaces
{
    using System.IO;
    using SMS.EPA.Model;

    public interface IEPADetailReportService
    {
        MemoryStream Export(EPADetailModel epaDetail);

        void Render(EPADetailModel epaDetail);
    }
}