namespace SMS.EPA.Services.Interfaces
{
    using Model;

    public interface IEPADetailReportService
    {
        void Export(EPADetailModel epaDetail);

        void Render(EPADetailModel epaDetail);
    }
}